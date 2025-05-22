import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import {
  Observable,
  filter,
  distinctUntilChanged,
  takeUntil,
  Subject,
  tap,
} from 'rxjs';
import { Store } from '@ngrx/store';
import {
  NgIf,
  NgFor,
  NgSwitch,
  NgSwitchCase,
  AsyncPipe,
  DatePipe,
  Location,
} from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

// Services
import { CoreService } from '../../sdk/services/coreService';
import { UserService } from '../../store_v2/user/user.services';
import { FormService } from './main.formservice';

// Models
import { IAUTH, UserRole, USER_ROLE_ENTITY } from '../../store_v2/auth/model';
import {
  IUSER,
  USER_QUERY_OBJECT,
  UserModel,
  Initial_User_Entity,
  UserReponse,
} from '../../store_v2/user/model';
import { ICONFIG } from '../../store_v2/configs/model';

// Selectors
import { renderNotify } from '../../store_v2/core/notify/notify.reducers';
import { selectAllAuth } from '../../store_v2/auth/auth.reducer';
import { selectAllConfigs } from '../../store_v2/configs/config.reducer';
import {
  actionUsers,
  addUsersSuccess,
  loadUsers,
  selectAllUsers,
  selectUsersLoading,
  selectRecords,
} from '../../store_v2/user/user.reducer';

// Components
import { ConfirmDialogViewComponent } from '../../sdk/components/modal/dialog.component';
import { PaginationComponent } from '../../sdk/components/pagination/pagination.component';
import { NoRecordFoundComponent } from '../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../sdk/components/reactiveform/dynamic-modal-form';
import { AppConfig } from '../../configs/app.configs';

/**
 * Main Users Component - Handles user management including listing, creation,
 * updating, and deletion of user records.
 */
@Component({
  selector: 'app-main-user',
  templateUrl: './main.html',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    NgSwitch,
    NgSwitchCase,
    AsyncPipe,
    DatePipe,
    RouterModule,
    FormsModule,
    NoRecordFoundComponent,
    LoaderComponent,
    DynamicModalFormComponent,
    PaginationComponent,
  ],
  providers: [FormService, UserService],
})
export class MainUserComponent implements OnInit, OnDestroy {
  // Dependency injections
  private readonly store = inject(Store);
  private readonly formService = inject(FormService);
  private readonly userService = inject(UserService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly coreService = inject(CoreService);
  private readonly titleService = inject(Title);
  public readonly appConfig = inject(AppConfig);
  private readonly modalService = inject(NgbModal);
  private readonly location = inject(Location);

  // Observable streams
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);
  loading$: Observable<boolean> = this.store.select(selectUsersLoading);
  data$: Observable<UserModel[]> = this.store.select(selectAllUsers);
  records$: Observable<number> = this.store.select(selectRecords);

  // Component state
  navQuery: IUSER = { ...USER_QUERY_OBJECT }; // Navigation query parameters
  isAuthenticated: boolean = false;
  User: UserModel = { ...Initial_User_Entity };

  selectedDocument: UserModel = { ...Initial_User_Entity };
  configs: ICONFIG[] = [];
  pageRole: UserRole = USER_ROLE_ENTITY;
  ProfileView: number = 0;
  // UI state
  navButton: string = 'Find Records';
  pageIndex: string = '_INDEX_';
  errorMessage: string = '';
  pageInit: boolean = false;
  showLoader: boolean = false;
  NoRecordText: string = 'No Records Found!';
  submitButtonText: string = 'Submit';
  headingTitle: string = 'Create Account';
  controls: any[] = [];

  // Cleanup subject for component destruction
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.initializeAuthSubscription();
  }

  /**
   * Initializes authentication state subscription
   * Handles user authentication status and loads app config when authenticated
   */
  private initializeAuthSubscription(): void {
    this.auth$
      .pipe(
        takeUntil(this.destroy$),
        tap((auth: IAUTH[]) => {
          if (auth.length > 0 && auth[0].isAuthenticated) {
            this.isAuthenticated = true;
            this.User = auth[0].User ?? { ...Initial_User_Entity };
            this.pageRole = this.coreService.processRole(auth[0].Role || []);
            this.loadAppConfigs();
          }
        })
      )
      .subscribe();
  }

  /**
   * Loads application configurations from store
   */
  private loadAppConfigs(): void {
    this.config$
      .pipe(
        takeUntil(this.destroy$),
        tap((config: ICONFIG[]) => {
          if (config.length > 0) {
            this.configs = config;
            this.navQuery.pagesize =
              this.configs[0].configs.settings.general.pageSize;
            this.initializeRouteListener();
          }
        })
      )
      .subscribe();
  }

  /**
   * Initializes route change listener
   * Handles route changes and updates component state accordingly
   */
  private initializeRouteListener(): void {
    /*this.router.events.pipe(
      takeUntil(this.destroy$),
      filter(event => event instanceof NavigationEnd),
      distinctUntilChanged()
    ).subscribe(() => {
      this.initializeRouteData();
    });*/

    this.route.params.pipe(takeUntil(this.destroy$)).subscribe((params) => {
      this.coreService.bindParams(this.navQuery, params);
      this.initializeRouteData();
    });
  }

  /**
   * Initializes route data based on current route
   */
  private initializeRouteData(): void {
    const routeData =
      this.route.snapshot.firstChild?.data || this.route.snapshot.data;

    if (routeData['key']) {
      this.pageIndex = routeData['key'];
    }

    if (routeData['title']) {
      this.titleService.setTitle(
        `${routeData['title']} - ${this.appConfig.getConfig('title')}`
      );
    }

    this.initializePage();
    this.pageInit = true;
  }

  /**
   * Initializes page based on current route index
   */
  private initializePage(): void {
    switch (this.pageIndex) {
      case '_INDEX_':
        this.initializeManageListings();
        break;
      case '_CREATE_':
      case '_UPDATE_':
        this.initializeCreateUpdate();
        break;
      case '_PROFILE_':
        this.initializeProfile();
        break;
      case '_CHANGE_PASS_':
        this.initUpdatePass();
        break;
      case '_CHANGE_EMAIL_':
        this.initUpdateEmail();
        break;
      case '_CHANGE_USER_ROLE_':
        this.initUpdateUserRole();
        break;
      case '_CHANGE_AVATAR_':
        this.initUpdateAvatar();
        break;

      default:
        this.handleUnknownPage();
        break;
    }
  }

  /**
   * Initializes user listings page
   */
  private initializeManageListings(): void {
    this.store.dispatch(loadUsers({ Query: this.navQuery }));
  }

  /**
   * Initializes create/update user page
   */
  private initializeCreateUpdate(): void {
    console.log('userid : ', this.navQuery.id);

    if (this.pageIndex === '_UPDATE_' && !this.navQuery.id) {
      this.errorMessage = 'Missing record information';
      return;
    }

    this.headingTitle =
      this.pageIndex === '_UPDATE_' ? 'Update Information' : 'Create Account';

    if (this.pageIndex === '_UPDATE_') {
      if (this.navQuery.id) {
        this.loadDocument(this.navQuery.id);
      } else {
        this.errorMessage = 'Invalid record ID';
      }
    } else {
      this.initializeControls();
    }
  }

  /**
   * Initializes change password page
   */
  private initUpdatePass(): void {
    console.log('userid : ', this.navQuery.id);

    this.headingTitle = 'Change Password';

    this.loadDocument(this.navQuery.id);
  }

  /**
   * Initializes change email page
   */
  private initUpdateEmail(): void {
    console.log('userid : ', this.navQuery.id);
    this.headingTitle = 'Change Email';
    this.loadDocument(this.navQuery.id);
  }

  /**
   * Initializes update user role
   */
  private initUpdateUserRole(): void {
    console.log('userid : ', this.navQuery.id);
    this.headingTitle = 'Change User Role';
    this.loadDocument(this.navQuery.id);
  }

  /**
   * Initializes update user role
   */
  private initUpdateAvatar(): void {
    console.log('userid : ', this.navQuery.id);
    this.headingTitle = 'Update Avatar';
    this.loadDocument(this.navQuery.id);
  }

  /**
   * Initializes user profile view page
   */
  private initializeProfile(): void {
    this.headingTitle = 'User Profile';
    this.loadDocument(this.navQuery.id);
  }

  /**
   * Handles unknown page routes
   */
  private handleUnknownPage(): void {
    this.errorMessage = `Unknown page: ${this.pageIndex}`;
  }

  /**
   * Loads user document by ID for update or profile view
   * @param id - User ID
   */
  private loadDocument(id: any): void {
    this.showLoader = true;
    this.userService
      .GetInfo({ ...USER_QUERY_OBJECT, id })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else if (response.record) {
            this.handleDocumentResponse(response.record);
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to load document', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Handles successful document load response
   * @param record - Loaded user record
   */
  private handleDocumentResponse(record: UserModel): void {
    this.selectedDocument = record;
    if (this.pageIndex === '_CHANGE_EMAIL_') {
      this.initializeChangeEmailControls();
    } else if (this.pageIndex === '_CHANGE_PASS_') {
      this.initializeChangePasswordControls();
    } else if (this.pageIndex === '_CHANGE_AVATAR_') {
      this.initializeUpdateAvatarControls();
    } else if (this.pageIndex === '_CHANGE_USER_ROLE_') {
      this.initializeUpdateUserRoleControls();
    } else if (this.pageIndex === '_UPDATE_') {
      this.initializeUpdateControls();
    } else if (this.pageIndex === '_PROFILE_') {
    } else {
      this.initializeControls();
    }
  }

  /**
   * Initializes form controls for create account
   */
  private initializeControls(): void {
    this.controls = this.formService.GenerateControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Initializes form controls for user editing
   */
  private initializeUpdateControls(): void {
    this.controls = this.formService.UpdateProfile(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Initializes change password control
   */
  private initializeChangePasswordControls(): void {
    this.controls = this.formService.ChangePasswordControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Initializes change email control
   */
  private initializeChangeEmailControls(): void {
    this.controls = this.formService.ChangeEmailControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Initializes update user role
   */
  private initializeUpdateUserRoleControls(): void {
    this.controls = this.formService.UpdateUserRoleControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Initializes update user role
   */
  private initializeUpdateAvatarControls(): void {
    this.controls = this.formService.UpdateAvatarControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Submits user form (create or update)
   * @param payload - User data to submit
   */
  submitForm(payload: UserModel): void {
    if (!this.validateForm(payload)) return;

    this.prepareDocumentForSubmission(payload);

    if (this.pageIndex === '_CHANGE_EMAIL_') {
      this.changeEmailSubmission();
    } else if (this.pageIndex === '_CHANGE_PASS_') {
      this.changePasswordSubmission();
    } else if (this.pageIndex === '_CHANGE_AVATAR_') {
      this.updateAvatarSubmission();
    } else if (this.pageIndex === '_CHANGE_USER_ROLE_') {
      this.changeUserRoleSubmission();
    } else {
      this.processUserSubmission();
    }
  }

  /**
   * Validates user form before submission
   * @param payload - User data to validate
   * @returns True if valid, false otherwise
   */
  private validateForm(payload: UserModel): boolean {
    return true;
  }

  /**
   * Prepares user document for submission
   * @param payload - User data to merge into document
   */
  private prepareDocumentForSubmission(payload: UserModel): void {
    Object.assign(this.selectedDocument, payload);
  }

  /**
   * Processes user submission to the server
   */
  private processUserSubmission(): void {
    this.showLoader = true;
    this.userService
      .ProcessRecord(this.selectedDocument)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else {
            this.handleSuccessfulSubmission(response);
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to submit blog', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Processes user submission to the server
   */
  private updateAvatarSubmission(): void {
    this.showLoader = true;
    this.userService
      .UpdateAvatar(this.selectedDocument)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else {
            this.showNotification(
              response.message ?? 'Operation successful',
              'success'
            );
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to update avatar', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Processes user submission to the server
   */
  private changePasswordSubmission(): void {
    this.showLoader = true;
    this.userService
      .ChangePassword(this.selectedDocument)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else {
            this.showNotification(
              response.message ?? 'Operation successful',
              'success'
            );
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to update password', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Processes user submission to the server
   */
  private changeEmailSubmission(): void {
    this.showLoader = true;
    this.userService
      .ChangeEmail(this.selectedDocument)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else {
            this.showNotification(
              response.message ?? 'Operation successful',
              'success'
            );
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to update email', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Processes user submission to the server
   */
  private changeUserRoleSubmission(): void {
    this.showLoader = true;
    this.userService
      .ChangeUserRole(this.selectedDocument)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: UserReponse) => {
          if (response.status === 'error') {
            this.showNotification(
              response.message ?? 'Unknown error',
              'danger'
            );
          } else {
            this.showNotification(
              response.message ?? 'Operation successful',
              'success'
            );
          }
          this.showLoader = false;
        },
        error: (err) => {
          this.showNotification('Failed to update user role', 'danger');
          this.showLoader = false;
        },
      });
  }

  /**
   * Handles successful user submission
   * @param response - Server response
   */
  private handleSuccessfulSubmission(response: UserReponse): void {
    const status = this.selectedDocument.id === '' ? 'added' : 'updated';

    if (response.record) {
      this.selectedDocument = response.record;
      this.store.dispatch(addUsersSuccess({ User: this.selectedDocument }));
    }

    this.showNotification(`Record ${status} successfully!`, 'success');
    this.router.navigate(['/users']);
  }

  /**
   * Displays notification to user
   * @param message - Notification message
   * @param type - Notification type (e.g., 'success', 'danger')
   */
  private showNotification(message: string, type: string): void {
    this.store.dispatch(
      renderNotify({
        title: message,
        text: '',
        css: `bg-${type}`,
      })
    );
  }

  /**
   * Handles search action
   * @param event - Search event
   */
  searchRecords(event: any): void {
    this.navQuery.pagenumber = 1;
    this.store.dispatch(loadUsers({ Query: this.navQuery }));
  }

  /**
   * Handles pagination selection
   * @param value - Selected page number
   */
  paginationSelection(value: number): void {
    this.navQuery.pagenumber = value;
    this.store.dispatch(loadUsers({ Query: this.navQuery }));
  }

  /**
   * Handles toolbar actions
   * @param action - Action to perform
   */
  toolbarAction(action: string): void {
    switch (action) {
      case 'add':
        this.router.navigate(['/users/create-account']);
        break;
    }
  }

  /**
   * Handles record deletion
   * @param Record - User record to delete
   * @param event - Click event
   */
  deleteRecord(Record: UserModel, event: Event): void {
    this.openConfirmationDialog(
      'Delete Record',
      'Are you sure you want to delete selected record',
      () => {
        this.store.dispatch(
          actionUsers({
            Entities: [Record],
            actionstatus: 'delete',
          })
        );
      },
      event
    );
  }

  /**
   * Handles navigation actions with confirmation
   */
  navAction(
    actionstatus: string,
    action_title: string,
    action_message: string,
    event: Event
  ): void {
    this.openConfirmationDialog(
      action_title,
      action_message,
      () => {

        if (actionstatus === 'enable') {
          this.selectedDocument.isEnabled = 1;
          this.selectedDocument.emailConfirmed = true;
        } else if (actionstatus === 'disable') {
          this.selectedDocument.isEnabled = 0;
        }
        
        this.store.dispatch(
          actionUsers({
            Entities: [this.selectedDocument],
            actionstatus,
          })
        );

        if (actionstatus === 'delete') {
          this.router.navigate(['/users']);
        }
      },
      event
    );
  }

  /**
   * Opens confirmation dialog
   * @param title - Dialog title
   * @param message - Dialog message
   * @param onConfirm - Callback for confirmation
   * @param event - Original event
   */
  private openConfirmationDialog(
    title: string,
    message: string,
    onConfirm: () => void,
    event: Event
  ): void {
    event.stopPropagation();

    const modalRef = this.modalService.open(ConfirmDialogViewComponent, {
      backdrop: false,
    });

    modalRef.componentInstance.Info = { title, message };

    modalRef.result.then(
      () => onConfirm(),
      () => console.log('Dialog dismissed')
    );
  }

  /**
   * Navigates to edit page
   * @param event - Click event
   */
  edit(event: Event): void {
    event.stopPropagation();
    this.router.navigate([`/users/update/${this.selectedDocument.id}`]);
  }

  updateAvatar(event: Event): void {}

  /**
   * Navigates back
   * @param event - Click event
   */
  back(event: Event): void {
    event.stopPropagation();
    this.location.back();
  }

  /**
   * TrackBy function for ngFor
   */
  getKey(index: number, item: any): string {
    return item.id;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
