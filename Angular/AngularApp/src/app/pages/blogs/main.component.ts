import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { Observable, filter, distinctUntilChanged, takeUntil, Subject, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import {
  NgIf,
  NgFor,
  NgSwitch,
  NgSwitchCase,
  AsyncPipe,
  DatePipe,
  JsonPipe,
  Location
} from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

// Services
import { CoreService } from '../../sdk/services/coreService';
import { BlogsService } from '../../store_v2/blog/blog.services';
import { FormService } from './main.formservice';

// Models
import {
  IAUTH,
  UserRole,
  USER_ROLE_ENTITY,
} from '../../store_v2/auth/model';
import {
  IBLOGS,
  BLOG_QUERY_OBJECT,
  BlogSettings,
  Initial_Blog_Settings,
  BlogModel,
  Initial_Blog_Entity,
  BlogResponse,
} from '../../store_v2/blog/model';
import { UserModel, Initial_User_Entity } from '../../store_v2/user/model';
import { ICONFIG } from '../../store_v2/configs/model';

// Selectors
import {
  renderNotify,
} from '../../store_v2/core/notify/notify.reducers';
import { selectAllAuth } from '../../store_v2/auth/auth.reducer';
import { selectAllConfigs } from '../../store_v2/configs/config.reducer';
import {
  actionBlogs,
  addBlogsSuccess,
  loadBlogs,
  selectAllBlogs,
  selectBlogsLoading,
  selectRecords,
} from '../../store_v2/blog/blog.reducer';

// Components
import { ConfirmDialogViewComponent } from '../../sdk/components/modal/dialog.component';
import { PaginationComponent } from '../../sdk/components/pagination/pagination.component';
import { NoRecordFoundComponent } from '../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../sdk/components/reactiveform/dynamic-modal-form';
import { AppConfig } from '../../configs/app.configs';

/**
 * Main Blogs Component - Handles blog management including listing, creation, 
 * updating, and deletion of blog posts.
 */
@Component({
  selector: 'app-main-blogs',
  templateUrl: './main.html',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    NgSwitch,
    NgSwitchCase,
    AsyncPipe,
    DatePipe,
    JsonPipe,
    RouterModule,
    FormsModule,
    NoRecordFoundComponent,
    LoaderComponent,
    DynamicModalFormComponent,
    PaginationComponent,
  ],
  providers: [FormService, BlogsService],
})
export class MainBlogsComponent implements OnInit, OnDestroy {
  // Dependency injections
  private readonly store = inject(Store);
  private readonly formService = inject(FormService);
  private readonly blogService = inject(BlogsService);
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
  loading$: Observable<boolean> = this.store.select(selectBlogsLoading);
  data$: Observable<BlogModel[]> = this.store.select(selectAllBlogs);
  records$: Observable<number> = this.store.select(selectRecords);

  // Component state
  navQuery: IBLOGS = { ...BLOG_QUERY_OBJECT }; // Navigation query parameters
  isAuthenticated: boolean = false;
  User: UserModel = { ...Initial_User_Entity };
  Settings: BlogSettings = { ...Initial_Blog_Settings };
  selectedDocument: BlogModel = { ...Initial_Blog_Entity };
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
    this.store.dispatch(loadBlogs({ Query: this.navQuery }));
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
    this.blogService.GetInfo({ ...BLOG_QUERY_OBJECT, id }).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response: BlogResponse) => {
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
  private handleDocumentResponse(record: BlogModel): void {
    this.selectedDocument = record;

    if (this.pageIndex === '_PROFILE_') {
      this.selectedDocument.enc_id = this.coreService.encrypt(this.selectedDocument.id);
      if (this.selectedDocument.tags) {
        this.Tags = this.coreService.ProcessTags(this.selectedDocument.tags.split(','));
      }
    } else {
      this.initializeControls();
    }
  }

  /**
   * Initializes form controls for blog editing
   */
  private initializeControls(): void {
    this.controls = this.formService.generateControls(this.selectedDocument, this.configs);
  }

  /**
   * Submits blog form (create or update)
   * @param payload - Blog data to submit
   */
  submitForm(payload: BlogModel): void {
    if (!this.validateForm(payload)) return;

    this.prepareDocumentForSubmission(payload);
    this.processBlogSubmission();
  }

  /**
   * Validates blog form before submission
   * @param payload - Blog data to validate
   * @returns True if valid, false otherwise
   */
  private validateForm(payload: BlogModel): boolean {
    if (!payload.blog_culture_data?.length) {
      this.showNotification('Please add one or more culture', 'danger');
      return false;
    }

    if (!this.selectedDocument.categorylist?.length) {
      this.showNotification('Please select one or more categories', 'danger');
      return false;
    }

    return true;
  }

  /**
   * Prepares blog document for submission
   * @param payload - Blog data to merge into document
   */
  private prepareDocumentForSubmission(payload: BlogModel): void {
    Object.assign(this.selectedDocument, payload);
    this.selectedDocument.userid = this.User.id;
    this.selectedDocument.categories = this.selectedDocument.categorylist || [];
    this.selectedDocument.categorylist = null; // Reset to avoid API parsing issues
    this.selectedDocument.isdraft = parseInt(this.selectedDocument.isdraft.toString());
  }

  /**
   * Processes blog submission to the server
   */
  private processBlogSubmission(): void {
    this.showLoader = true;
    this.blogService.ProcessRecord(this.selectedDocument).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response: BlogResponse) => {
        if (response.status === 'error') {
          this.showNotification(response.message ?? 'Unknown error', 'danger');
        } else {
          this.handleSuccessfulSubmission(response);
        }
        this.showLoader = false;
      },
      error: (err) => {
        this.showNotification('Failed to submit blog', 'danger');
        this.showLoader = false;
      }
    });
  }

  /**
   * Handles successful blog submission
   * @param response - Server response
   */
  private handleSuccessfulSubmission(response: BlogResponse): void {
    const status = this.selectedDocument.id === 0 ? 'added' : 'updated';
    
    if (response.record) {
      this.selectedDocument = response.record;
      this.store.dispatch(addBlogsSuccess({ blogs: this.selectedDocument }));
    }

    this.showNotification(`Record ${status} successfully!`, 'success');
    this.router.navigate(['/blogs']);
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
    this.store.dispatch(loadBlogs({ Query: this.navQuery }));
  }

  /**
   * Handles pagination selection
   * @param value - Selected page number
   */
  paginationSelection(value: number): void {
    this.navQuery.pagenumber = value;
    this.store.dispatch(loadBlogs({ Query: this.navQuery }));
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
  deleteRecord(Record: BlogModel, event: Event): void {
    this.openConfirmationDialog(
      'Delete Record',
      'Are you sure you want to delete selected record',
      () => {
        this.store.dispatch(actionBlogs({ 
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
        this.store.dispatch(actionBlogs({ 
          Entities: [this.selectedDocument], 
          actionstatus 
        }));
        
        if (actionstatus === 'delete') {
          this.router.navigate(['/blogs']);
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
   * Navigates to edit page
   * @param event - Click event
   */
  edit(event: Event): void {
    event.stopPropagation();
    this.router.navigate([`/blogs/update-post/${this.selectedDocument.enc_id}`]);
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

/* for testing purpose

Improve angular javascript code and write detail comments for code below 
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { Observable, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import {
  NgIf,
  NgFor,
  NgSwitch,
  NgSwitchCase,
  AsyncPipe,
  JsonPipe,
  DatePipe,
  Location
} from '@angular/common';
import { RouterModule } from '@angular/router';
// inject services
//import { FormService } from '../services/form.service';
import { CoreService } from '../../sdk/services/coreService';

// Models
import {
  IAUTH,
  UserRole,
  USER_ROLE_ENTITY,
} from '../../store_v2/auth/model';
import {
  IBLOGS,
  BLOG_QUERY_OBJECT,
  BlogSettings,
  Initial_Blog_Settings,
  BlogModel,
  Initial_Blog_Entity,
  BlogResponse,
} from '../../store_v2/blog/model';
import { UserModel, Initial_User_Entity } from '../../store_v2/user/model';
import { ICONFIG } from '../../store_v2/configs/model';

// Selectors
import {
  renderNotify,
  renderSelector,
} from '../../store_v2/core/notify/notify.reducers';
import { selectAllAuth } from '../../store_v2/auth/auth.reducer';
import { selectAllConfigs } from '../../store_v2/configs/config.reducer';
import {
  actionBlogs,
  addBlogsSuccess,
  loadBlogs,
  selectAllBlogs,
  selectBlogsLoading,
  selectRecords,
} from '../../store_v2/blog/blog.reducer';

// Services
import { BlogsService } from '../../store_v2/blog/blog.services';
import { FormService } from './main.formservice';

// Components
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogViewComponent } from '../../sdk/components/modal/dialog.component';
//import { ListComponent } from './List/list.component';
// import { ToolbarListStatsComponent } from '../../../sdk/components/liststats/liststats.component';
import { PaginationComponent } from '../../sdk/components/pagination/pagination.component';
import { NoRecordFoundComponent } from '../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../sdk/components/reactiveform/dynamic-modal-form';
import { AppConfig } from '../../configs/app.configs';


@Component({
  selector: 'app-main-blogs',
  templateUrl: './main.html',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    NgSwitch,
    NgSwitchCase,
    AsyncPipe,
    //JsonPipe,
    DatePipe,
    RouterModule,
    FormsModule,
    NoRecordFoundComponent,
    LoaderComponent,
    DynamicModalFormComponent,
    PaginationComponent,
  ],
  providers: [FormService, BlogsService],
})
export class MainBlogsComponent implements OnInit, OnDestroy {
  private store = inject(Store);
  private readonly formService = inject(FormService);
  private readonly blogService = inject(BlogsService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private coreService = inject(CoreService);
  private titleService = inject(Title);
  private appConfig = inject(AppConfig);
  private modalService = inject(NgbModal);
  private location = inject(Location)
  constructor() {}

  // Authentication state
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);

  // App config state
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);

  // List loading state
  loading$: Observable<any> = this.store.select(selectBlogsLoading);

  // List Data state
  data$: Observable<BlogModel[]> = this.store.select(selectAllBlogs);

  // Total Records
  records$ = this.store.select(selectRecords);

  // Blog query
  navQuery: IBLOGS = Object.assign({}, BLOG_QUERY_OBJECT);

  isAuthenticated: boolean = false;
  User: UserModel = Object.assign({}, Initial_User_Entity);

  // settings
  Settings: BlogSettings = Object.assign({}, Initial_Blog_Settings);

  // document info // incase of update post / profile view
  selectedDocument: BlogModel = { ...Initial_Blog_Entity };

  // nav filter controls
  navButton: string = 'Find Records';
  pageIndex: string = '_INDEX_';
  errorMessage: string = '';
  pageInit: boolean = false;
  showLoader: boolean = false;
  NoRecordText: string = 'No Records Found!';
  // Reactive form controls
  controls: any[] = [];
  submitButtonText: string = 'Submit';
  headingTitle: string = 'Add Post';
  Tags: any[] = []
  // App configurations -> fetch from store
  configs: ICONFIG[] = [];

  // Logged user role / permissions -. fetch from store
  pageRole: UserRole = USER_ROLE_ENTITY;

  ngOnInit(): void {
    this.auth$ = this.store.select(selectAllAuth).pipe(
      tap((auth: any) => {
        //console.log('Selected auth:', auth);
        if (auth.length > 0) {
          this.isAuthenticated = auth[0].isAuthenticated;
          this.User = auth[0].User;

          this.pageRole = this.coreService.processRole(auth[0].Role);

          // load app config data
          this.LoadAppConfigs();
        }
      })
    );
    this.auth$.subscribe();
  }

  LoadAppConfigs() {
    this.config$ = this.store.select(selectAllConfigs).pipe(
      tap((config: ICONFIG[]) => {
        if (config.length > 0) {
          this.configs = config;
          this.navQuery.pagesize =
            this.configs[0].configs.settings.general.pageSize;
          this.Initialize();
        }
      })
    );
    this.config$.subscribe();
  }

  Initialize() {
    // Subscribe to route changes
    

      this.route.params.subscribe((params) => {
        // Filter query parameters based on route params
        this.coreService.bindParams(this.navQuery, params);
        // Get initial route data
        this.initializeRouteData();
      });
    }
  
    private initializeRouteData(): void {
      // Get the current route path
      //let currentRoute = this.router.url.split('?')[0]; // Remove query params if any
  
      // Get the route data
      let routeData: any =
        this.route.snapshot.firstChild?.data || this.route.snapshot.data;
  
      if (
        routeData.key !== undefined &&
        routeData.key !== null &&
        routeData.key !== ''
      ) {
        this.pageIndex = routeData.key;
      }
  
      if (
        routeData.title !== undefined &&
        routeData.title !== null &&
        routeData.title !== ''
      ) {
        this.titleService.setTitle(
          routeData.title + ' - ' + this.appConfig.getConfig('title')
        );
      }
  
      this.initializePage();
      this.pageInit = true;
    }
  
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
  
    initializeManageListings() {
      this.store.dispatch(loadBlogs({ Query: this.navQuery }));
    }
  
    initializeCreateUpdate() {
      // decrypt id received
      if (this.navQuery.id !== 0) {
        this.navQuery.id = parseInt(
          this.coreService.decrypt(this.navQuery.id?.toString()),
          10
        );
      }
  
      if (this.pageIndex === '_UPDATE__' && this.navQuery.id === 0) {
        this.errorMessage = 'Missing record information';
        return;
      }
  
      if (this.pageIndex === '_UPDATE_') {
        this.headingTitle = 'Update Post';
        if (this.navQuery.id !== undefined) {
          this.loadDocument(this.navQuery.id);
        } else {
          this.errorMessage = 'Invalid record ID';
        }
      } else {
        this.headingTitle = 'Create Post';
        this.initializeControls();
      }
    }
  
    initializeProfile() {
      // decrypt id received
      if (this.navQuery.id !== 0) {
        this.navQuery.id = parseInt(
          this.coreService.decrypt(this.navQuery.id?.toString()),
          10
        );
      }
      if (
        this.navQuery.id === undefined ||
        this.navQuery.id === null ||
        this.navQuery.id === 0
      ) {
        this.errorMessage = 'Invalid record ID';
        return;
      }
  
      this.loadDocument(this.navQuery.id);
    }
  
    handleUnknownPage() {
      this.errorMessage = `Unknown page: ${this.pageIndex}`;
    }
  
    // For loading document information (Update or Profile View)
    loadDocument(id: number) {
      this.showLoader = true;
      this.blogService
        .GetInfo({
          ...BLOG_QUERY_OBJECT,
          id,
        })
        .pipe()
        .subscribe((response: BlogResponse) => {
          if (response.status === 'error') {
            this.store.dispatch(
              renderNotify({
                title: response.message ?? 'Unknown error',
                text: '',
                css: 'bg-danger',
              })
            );
          } else {
            if (response.record) {
              this.selectedDocument = response.record;
  
              if (this.pageIndex === '_PROFILE_') {
                this.selectedDocument.enc_id = this.coreService.encrypt(this.selectedDocument.id)
                // tags
                if (this.selectedDocument.tags !== undefined && this.selectedDocument.tags !== null && this.selectedDocument.tags !== "") {
                  this.Tags = this.coreService.ProcessTags(
                    this.selectedDocument.tags.split(",")
                  );
                } 
              } else {
                // Add or Update Post
                this.initializeControls();
              }
            }
  
            this.showLoader = false;
          }
        });
    }
  
    private initializeControls(): void {
      this.controls = this.formService.generateControls(
        this.selectedDocument,
        this.configs
      );
    }
  
    submitForm(payload: BlogModel): void {
      let CultureData = payload.blog_culture_data;
      if (
        payload.blog_culture_data === undefined ||
        CultureData === null ||
        CultureData.length === 0
      ) {
        this.store.dispatch(
          renderNotify({
            title: 'Please add one or more culture',
            text: '',
            css: 'bg-danger',
          })
        );
        return;
      }
  
      for (let prop in payload) {
        (this.selectedDocument as any)[prop] = payload[prop as keyof BlogModel];
      }
  
      // console.log('selected document: ', this.selectedDocument.categorylist);
  
      this.selectedDocument.userid = this.User.id;
  
      if (
        this.selectedDocument.categorylist === undefined ||
        this.selectedDocument.categorylist === null ||
        this.selectedDocument.categorylist.length === 0
      ) {
        this.store.dispatch(
          renderNotify({
            title: 'Please select one or more categories',
            text: '',
            css: 'bg-danger',
          })
        );
        return;
      }
  
      if (
        this.selectedDocument.categorylist !== null &&
        this.selectedDocument.categorylist.length > 0
      ) {
        this.selectedDocument.categories = this.selectedDocument.categorylist;
      }
      // _reset categorylist -> avoid parsing error in api
      this.selectedDocument.categorylist = null;
      this.selectedDocument.isdraft = parseInt(
        this.selectedDocument.isdraft.toString()
      );
  
      this.showLoader = true;
      this.blogService
        .ProcessRecord(this.selectedDocument)
        .pipe()
        .subscribe((response: BlogResponse) => {
          if (response.status === 'error') {
            this.store.dispatch(
              renderNotify({
                title: response.message ?? 'Unknown error',
                text: '',
                css: 'bg-danger',
              })
            );
          } else {
            let _status = 'updated';
            if (this.selectedDocument.id === 0) {
              _status = 'added';
            }
  
            if (response.record) {
              if (this.selectedDocument.id === 0) {
                // add record
                this.selectedDocument.id = response.record.id;
                this.store.dispatch(
                  addBlogsSuccess({ blogs: this.selectedDocument })
                );
              } else {
                // update record
                this.store.dispatch(
                  addBlogsSuccess({ blogs: this.selectedDocument })
                );
              }
  
              this.selectedDocument = response.record;
            }
  
            this.store.dispatch(
              renderNotify({
                title: 'Record ' + _status + ' successfully!',
                text: '',
                css: 'bg-success',
              })
            );
  
            // redirect to blogs page
            this.router.navigate(['/blogs']);
  
            this.showLoader = false;
          }
        });
    }
  
    searchRecords(event: any) {
      // reset page number to 1
      this.navQuery.pagenumber = 1;
      this.store.dispatch(loadBlogs({ Query: this.navQuery }));
    }
  
    paginationSelection(value: number) {
      this.navQuery.pagenumber = value;
      this.store.dispatch(loadBlogs({ Query: this.navQuery }));
    }
  
    toolbarAction(action: any) {
      switch (action) {
        case 'add':
          this.router.navigate(['/blogs/create-post']);
          break;
  
        case 'categories':
          this.router.navigate(['/categories/2']);
          break;
      }
    }
  
    deleteRecord(Record: BlogModel, event: any) {
      const _options: NgbModalOptions = {
        backdrop: false,
      };
      const modalRef = this.modalService.open(
        ConfirmDialogViewComponent,
        _options
      );
  
      modalRef.componentInstance.Info = {
        title: 'Delete Record',
        message: 'Are you sure you want to delete selected reocrd',
      };
  
      modalRef.result.then(
        (result) => {
          this.store.dispatch(
            actionBlogs({ Entities: [Record], actionstatus: 'delete' })
          );
        },
        (dismissed) => {
          console.log('closed');
        }
      );
      event.stopPropagation();
    }
  
    navAction(actionstatus: string, action_title: string, action_message: string, message: string, event: any) {
      const _options: NgbModalOptions = {
        backdrop: false,
      };
      const modalRef = this.modalService.open(
        ConfirmDialogViewComponent,
        _options
      );
  
      modalRef.componentInstance.Info = {
        title:  action_title, // 'Delete Record',
        message:  action_message, // 'Are you sure you want to delete selected reocrd',
      };
  
      modalRef.result.then(
        (result) => {
        
          this.store.dispatch(
            actionBlogs({ Entities: [this.selectedDocument], actionstatus })
          );
      
          if (actionstatus === 'delete') {
             this.router.navigate(['/blogs']);
          } 
  
        },
        (dismissed) => {
          console.log('closed');
        }
      );
      event.stopPropagation();
    }
  
    edit(event: any) {
      event.stopPropagation();
      this.router.navigate(['/blogs/update-post/' + this.selectedDocument.enc_id]);
    }
  
    back(event: any) {
      this.location.back();
      event.stopPropagation();
    }
  
    getKey(index: number, item: any): string {
      return item.id;
    }
  
    ngOnDestroy(): void {
     
    }
  }

  */