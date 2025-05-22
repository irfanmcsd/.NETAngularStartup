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
import { FormService } from '../users/main.formservice';

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
  addUsersSuccess,
  selectAllUsers,
  selectUsersLoading,
  selectRecords,
} from '../../store_v2/user/user.reducer';

// Components
import { NoRecordFoundComponent } from '../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../sdk/components/reactiveform/dynamic-modal-form';
import { AppConfig } from '../../configs/app.configs';

/**
 * Main Users Component - Handles user management including listing, creation,
 * updating, and deletion of user records.
 */
@Component({
  templateUrl: './userprofile.html',
  standalone: true,
  imports: [
    NgIf,
    RouterModule,
    FormsModule,
    LoaderComponent,
    DynamicModalFormComponent,
    NoRecordFoundComponent
  ],
  providers: [FormService, UserService],
})
export class UserProfileComponent implements OnInit, OnDestroy {
  // Dependency injections
  private readonly store = inject(Store);
  private readonly formService = inject(FormService);
  private readonly userService = inject(UserService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly coreService = inject(CoreService);
  private readonly titleService = inject(Title);
  public readonly appConfig = inject(AppConfig);
  private readonly location = inject(Location);

  // Observable streams
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);
 

  // Component state
  navQuery: IUSER = { ...USER_QUERY_OBJECT }; // Navigation query parameters
  isAuthenticated: boolean = false;
  User: UserModel = { ...Initial_User_Entity };

  selectedDocument: UserModel = { ...Initial_User_Entity };
  configs: ICONFIG[] = [];
  pageRole: UserRole = USER_ROLE_ENTITY;
  ProfileView: number = 0;
  // UI state
  pageIndex: string = '_INDEX_';
  errorMessage: string = '';
  pageInit: boolean = false;
  showLoader: boolean = false;
  submitButtonText: string = 'Save Changes';
  headingTitle: string = 'User Profile';
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
    this.initializeCreateUpdate();
  }

  /**
   * Initializes create/update user page
   */
  private initializeCreateUpdate(): void {
    console.log('userid : ', this.User.id);

    this.headingTitle = 'User Profile';

    if (this.User.id) {
      this.loadDocument(this.User.id);
    } else {
      this.errorMessage = 'Invalid record ID';
    }
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
    this.initializeUpdateControls();
  }

  /**
   * Initializes form controls for user editing
   */
  private initializeUpdateControls(): void {
    this.controls = this.formService.UpdateUserProfile(
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
    this.processUserSubmission();
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
   * Handles successful user submission
   * @param response - Server response
   */
  private handleSuccessfulSubmission(response: UserReponse): void {
    const status = this.selectedDocument.id === '' ? 'added' : 'updated';

    if (response.record) {
      this.selectedDocument = response.record;
      // this.store.dispatch(addUsersSuccess({ User: this.selectedDocument }));
    }

    this.showNotification(`Profile ${status} successfully!`, 'success');
    //this.router.navigate(['/users']);
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
