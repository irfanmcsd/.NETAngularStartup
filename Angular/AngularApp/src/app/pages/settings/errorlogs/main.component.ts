import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { Observable, takeUntil, Subject, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import {
  NgIf,
  NgFor,
  NgSwitch,
  NgSwitchCase,
  AsyncPipe,
  DatePipe,
  Location
} from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

// Services
import { CoreService } from '../../../sdk/services/coreService';
import { ErrorLogService } from '../../../store_v2/errorlog/log.services';

// Models
import {
  IAUTH,
  UserRole,
  USER_ROLE_ENTITY,
} from '../../../store_v2/auth/model';
import {
  ILOG,
  LOG_QUERY_OBJECT,
  Initial_Log_Entity,
  ErrorLogReponse,
  ErrorLogModel,
} from '../../../store_v2/errorlog/model';
import { UserModel, Initial_User_Entity } from '../../../store_v2/user/model';
import { ICONFIG } from '../../../store_v2/configs/model';

// Selectors
import {
  renderNotify,
} from '../../../store_v2/core/notify/notify.reducers';
import { selectAllAuth } from '../../../store_v2/auth/auth.reducer';
import { selectAllConfigs } from '../../../store_v2/configs/config.reducer';
import {
  actionErrorLogs,
  loadErrorLogs,
  selectAllErrorLogs,
  selectErrorLogsLoading,
  selectRecords,
} from '../../../store_v2/errorlog/log.reducer';

// Components
import { ConfirmDialogViewComponent } from '../../../sdk/components/modal/dialog.component';
import { PaginationComponent } from '../../../sdk/components/pagination/pagination.component';
import { NoRecordFoundComponent } from '../../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../../sdk/components/reactiveform/dynamic-modal-form';
import { AppConfig } from '../../../configs/app.configs';

/**
 * Main Blogs Component - Handles blog management including listing, creation, 
 * updating, and deletion of blog posts.
 */
@Component({
  selector: 'app-main-log',
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
  providers: [ErrorLogService],
})
export class MainUsersComponent implements OnInit, OnDestroy {
  // Dependency injections
  private readonly store = inject(Store);
  private readonly logService = inject(ErrorLogService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly coreService = inject(CoreService);
  private readonly titleService = inject(Title);
  private readonly appConfig = inject(AppConfig);
  private readonly modalService = inject(NgbModal);
  private readonly location = inject(Location);

  // Observable streams
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);
  loading$: Observable<boolean> = this.store.select(selectErrorLogsLoading);
  data$: Observable<ErrorLogModel[]> = this.store.select(selectAllErrorLogs);
  records$: Observable<number> = this.store.select(selectRecords);

  // Component state
  navQuery: ILOG = { ...LOG_QUERY_OBJECT }; // Navigation query parameters
  isAuthenticated: boolean = false;
  User: UserModel = { ...Initial_User_Entity };
  
  selectedDocument: ErrorLogModel = { ...Initial_Log_Entity };
  Tags: any[] = [];
  configs: ICONFIG[] = [];
  pageRole: UserRole = USER_ROLE_ENTITY;

  // UI state
  navButton: string = 'Find Records';
  pageIndex: string = '_INDEX_';
  errorMessage: string = '';
  pageInit: boolean = false;
  showLoader: boolean = false;
  NoRecordText: string = 'No Records Found!';
  submitButtonText: string = 'Submit';
  headingTitle: string = 'Add Post';
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
    this.auth$.pipe(
      takeUntil(this.destroy$),
      tap((auth: IAUTH[]) => {
        if (auth.length > 0 && auth[0].isAuthenticated) {
          this.isAuthenticated = true;
          this.User = auth[0].User ?? { ...Initial_User_Entity };
          this.pageRole = this.coreService.processRole(auth[0].Role || []);
          this.loadAppConfigs();
        }
      })
    ).subscribe();
  }

  /**
   * Loads application configurations from store
   */
  private loadAppConfigs(): void {
    this.config$.pipe(
      takeUntil(this.destroy$),
      tap((config: ICONFIG[]) => {
        if (config.length > 0) {
          this.configs = config;
          this.navQuery.pagesize = this.configs[0].configs.settings.general.pageSize;
          this.initializeRouteListener();
        }
      })
    ).subscribe();
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

    this.route.params.pipe(
      takeUntil(this.destroy$)
    ).subscribe(params => {
      this.coreService.bindParams(this.navQuery, params);
      this.initializeRouteData();
    });
  }

  

  /**
   * Initializes route data based on current route
   */
  private initializeRouteData(): void {
    const routeData = this.route.snapshot.firstChild?.data || this.route.snapshot.data;

    if (routeData['key']) {
      this.pageIndex = routeData['key'];
    }

    if (routeData['title']) {
      this.titleService.setTitle(`${routeData['title']} - ${this.appConfig.getConfig('title')}`);
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
      default:
        this.handleUnknownPage();
        break;
    }
  }

  /**
   * Initializes blog listings page
   */
  private initializeManageListings(): void {
    this.store.dispatch(loadErrorLogs({ Query: this.navQuery }));
  }

  /**
   * Initializes create/update blog page
   */
  private initializeCreateUpdate(): void {
      
    if (this.navQuery.id !== 0) {
      this.navQuery.id = parseInt(this.coreService.decrypt(this.navQuery.id?.toString()), 10);
    }

    if (this.pageIndex === '_UPDATE_' && !this.navQuery.id) {
      this.errorMessage = 'Missing record information';
      return;
    }

    this.headingTitle = this.pageIndex === '_UPDATE_' ? 'Update Post' : 'Create Post';

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
   * Initializes blog profile view page
   */
  private initializeProfile(): void {
    if (this.navQuery.id !== 0) {
      this.navQuery.id = parseInt(this.coreService.decrypt(this.navQuery.id?.toString()), 10);
    }

    if (!this.navQuery.id) {
      this.errorMessage = 'Invalid record ID';
      return;
    }

    this.loadDocument(this.navQuery.id);
  }

  /**
   * Handles unknown page routes
   */
  private handleUnknownPage(): void {
    this.errorMessage = `Unknown page: ${this.pageIndex}`;
  }

  /**
   * Loads blog document by ID for update or profile view
   * @param id - Blog post ID
   */
  private loadDocument(id: number): void {
    this.showLoader = true;
    this.logService.GetInfo({ ...LOG_QUERY_OBJECT, id }).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response: ErrorLogReponse) => {
        if (response.status === 'error') {
          this.showNotification(response.message ?? 'Unknown error', 'danger');
        } else if (response.record) {
          this.handleDocumentResponse(response.record);
        }
        this.showLoader = false;
      },
      error: (err) => {
        this.showNotification('Failed to load document', 'danger');
        this.showLoader = false;
      }
    });
  }

  /**
   * Handles successful document load response
   * @param record - Loaded blog record
   */
  private handleDocumentResponse(record: ErrorLogModel): void {
    this.selectedDocument = record;

    if (this.pageIndex === '_PROFILE_') {
      
    } else {
      this.initializeControls();
    }
  }

  /**
   * Initializes form controls for blog editing
   */
  private initializeControls(): void {
    // this.controls = this.formService.generateControls(this.selectedDocument, this.configs);
  }
 

  /**
   * Displays notification to user
   * @param message - Notification message
   * @param type - Notification type (e.g., 'success', 'danger')
   */
  private showNotification(message: string, type: string): void {
    this.store.dispatch(renderNotify({
      title: message,
      text: '',
      css: `bg-${type}`
    }));
  }

  /**
   * Handles search action
   * @param event - Search event
   */
  searchRecords(event: any): void {
    this.navQuery.pagenumber = 1;
    this.store.dispatch(loadErrorLogs({ Query: this.navQuery }));
  }

  /**
   * Handles pagination selection
   * @param value - Selected page number
   */
  paginationSelection(value: number): void {
    this.navQuery.pagenumber = value;
    this.store.dispatch(loadErrorLogs({ Query: this.navQuery }));
  }

  /**
   * Handles toolbar actions
   * @param action - Action to perform
   */
  toolbarAction(action: string): void {
    switch (action) {
      case 'add':
        this.router.navigate(['/blogs/create-post']);
        break;
      case 'categories':
        this.router.navigate(['/categories/2']);
        break;
    }
  }

  /**
   * Handles record deletion
   * @param Record - Blog record to delete
   * @param event - Click event
   */
  deleteRecord(Record: ErrorLogModel, event: Event): void {
    this.openConfirmationDialog(
      'Delete Record',
      'Are you sure you want to delete selected record',
      () => {
        this.store.dispatch(actionErrorLogs({ 
          Entities: [Record], 
          actionstatus: 'delete' 
        }));
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
        this.store.dispatch(actionErrorLogs({ 
          Entities: [this.selectedDocument], 
          actionstatus 
        }));
        
        if (actionstatus === 'delete') {
          this.router.navigate(['/logs']);
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
      backdrop: false
    });

    modalRef.componentInstance.Info = { title, message };

    modalRef.result.then(
      () => onConfirm(),
      () => console.log('Dialog dismissed')
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
