import { Component, OnInit, OnDestroy, inject, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  NgIf,
  NgFor,
  NgClass,
  AsyncPipe,
  Location,
  JsonPipe,
  NgTemplateOutlet,
} from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable, tap } from 'rxjs';

// Services
import { CategoryService } from '../../../store_v2/category/category.services';
import { FormService } from '../services/form.service';
import { CoreService } from '../../../sdk/services/coreService';

// Models
import {
  IAUTH,
  UserRole,
  USER_ROLE_ENTITY,
} from '../../../store_v2/auth/model';
import { ICONFIG } from '../../../store_v2/configs/model';
import {
  ICATEGORY,
  CATEGORY_QUERY_OBJECT,
  CategorySettings,
  Initial_Category_Settings,
  CategoryModel,
  CategoryData,
  Initial_Category_Entity,
  CategoryReponse,
} from '../../../store_v2/category/model';

// Selectors
import {
  renderNotify,
  renderSelector,
} from '../../../store_v2/core/notify/notify.reducers';
import { selectAllAuth } from '../../../store_v2/auth/auth.reducer';
import { selectAllConfigs } from '../../../store_v2/configs/config.reducer';
import {
  loadCategories,
  selectCategoriesLoading,
  selectIsCategorySelected,
  toggleCategorySelection,
  addCategoriesSuccess,
  selectNavList,
  selectCategoryChildren,
  selectFilteredNavList,
  updateCategorySuccess,
  actionCategories,
} from '../../../store_v2/category/category.reducer';

// Components
import { NoRecordFoundComponent } from '../../../sdk/components/norecord/norecord.component';
import { LoaderComponent } from '../../../sdk/components/loader/loader.component';
import { DynamicModalFormComponent } from '../../../sdk/components/reactiveform/dynamic-modal-form';
import { Initial_User_Entity, UserModel } from '../../../store_v2/user/model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogViewComponent } from '../../../sdk/components/modal/dialog.component';

/**
 * MainCategoryComponent - Category management component
 *
 * Features:
 * - Displays and manages category hierarchy
 * - Handles category CRUD operations
 * - Integrates with NgRx store for state management
 * - Supports filtering and navigation
 * - Provides role-based access control
 */
@Component({
  selector: 'app-main-category',
  templateUrl: './main.html',
  standalone: true,
  styleUrls: ['./category.css'],
  imports: [
    NgIf,
    NgFor,
    NgClass,
    NgTemplateOutlet,
    FormsModule,
    AsyncPipe,
    //JsonPipe,
    NoRecordFoundComponent,
    LoaderComponent,
    DynamicModalFormComponent,
  ],
  providers: [FormService, CategoryService],
})
export class MainCategoryComponent implements OnInit, OnDestroy {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly store = inject(Store);
  private readonly formService = inject(FormService);
  private readonly categoryService = inject(CategoryService);
  private readonly coreService = inject(CoreService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly location = inject(Location);
  private readonly modalService = inject(NgbModal);

  
  @Input() Type: number = 0;

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showLoader = false;
  isAuthenticated = false;
  searchTerm = '';
  private readonly LABEL_MAP = {
    7: { singular: 'Document', plural: 'Documents' },
    default: { singular: 'Category', plural: 'Categories' },
  };
  label = '';
  labels = '';
  parentIds: number[] = [];
  parentID = 0;
  submitButtonText = 'Save Changes';
  controls: any[] = [];

  /* ---------------------------- STORE OBSERVABLES ---------------------------- */
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);
  loading$: Observable<boolean> = this.store.select(selectCategoriesLoading);
  // navList$: Observable<CategoryModel[]> = this.store.select(selectNavList);
  filteredNavList$: Observable<CategoryModel[]> = this.getChildren(
    this.parentID
  );

  /* ---------------------------- MODELS & CONFIGURATIONS ---------------------------- */
  User: UserModel = { ...Initial_User_Entity };
  pageRole: UserRole = USER_ROLE_ENTITY;
  configs: ICONFIG[] = [];
  navQuery: ICATEGORY = { ...CATEGORY_QUERY_OBJECT };
  selectedDocument: CategoryModel = { ...Initial_Category_Entity };

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */

  ngOnInit(): void {
    this.initializeAuthSubscription();
  }

  ngOnDestroy(): void {
    // Clean up subscriptions if needed
  }

  /* ---------------------------- INITIALIZATION METHODS ---------------------------- */

  /**
   * Initializes authentication state subscription
   */
  private initializeAuthSubscription(): void {
    this.auth$ = this.store.select(selectAllAuth).pipe(
      tap((auth: IAUTH[]) => {
        if (auth.length > 0) {
          this.isAuthenticated = auth[0].isAuthenticated;
          this.User = auth[0].User || { ...Initial_User_Entity };
          this.pageRole = this.coreService.processRole(auth[0].Role || []);
          this.loadAppConfigs();
        }
      })
    );
    this.auth$.subscribe();
  }

  /**
   * Loads application configurations from store
   */
  private loadAppConfigs(): void {
    this.config$ = this.store.select(selectAllConfigs).pipe(
      tap((config: ICONFIG[]) => {
        if (config.length > 0) {
          this.configs = config;
          this.initializeCategoryData();
        }
      })
    );
    this.config$.subscribe();
  }

  /**
   * Initializes category data based on route parameters
   */
  private initializeCategoryData(): void {
    this.route.params.subscribe((params) => {
      this.coreService.bindParams(this.navQuery, params);
      if (this.Type > 0) {
        // type returned via input
        this.navQuery.type = this.Type;
      }
      if (Number(this.navQuery.type) !== 0) {
        this.navQuery.loadall = true;
        this.store.dispatch(loadCategories({ Query: this.navQuery }));
      }
      this.initializeLabel(this.navQuery.type);
    });
  }

  /**
   * Sets the appropriate labels based on the given type
   * @param type The type identifier that determines which labels to use
   */
  initializeLabel(type: number): void {
    const labelSet =
      this.LABEL_MAP[type as keyof typeof this.LABEL_MAP] ||
      this.LABEL_MAP.default;
    this.label = labelSet.singular;
    this.labels = labelSet.plural;
  }
  /* ---------------------------- CATEGORY METHODS ---------------------------- */

  /**
   * Initializes a new category document
   * @param parentId Parent category ID
   * @returns New category model
   */
  private initializeDocument(parentId: number): CategoryModel {
    return {
      ...Initial_Category_Entity,
      type: this.navQuery.type,
      parentid: parentId,
    };
  }

  /**
   * Initializes form controls for category editing
   */
  private initializeControls(): void {
    this.controls = this.formService.generateControls(
      this.selectedDocument,
      this.configs
    );
  }

  /**
   * Filters categories by search term
   * @param categories Category array to filter
   * @param filterBy Search term
   * @returns Filtered categories
   */
  /*filterByTerm(categories: CategoryModel[], filterBy: string): CategoryModel[] {
    return categories.filter((category) =>
      category.category_data.title
        .toLowerCase()
        .includes(filterBy.toLowerCase())
    );
  }*/

  /* ---------------------------- EVENT HANDLERS ---------------------------- */

  /**
   * Handles add document action
   * @param event Mouse event
   */
  addDocument(event: MouseEvent): void {
    this.selectedDocument = this.initializeDocument(this.parentID);
    this.selectedDocument.culture_categories = []
    this.submitButtonText = 'Add Category';
    this.initializeControls();
    event.stopPropagation();
  }

  /**
   * Handles back navigation
   * @param event Mouse event
   */
  navigateBack(event: MouseEvent): void {
    event.stopPropagation();
    if (this.parentIds.length === 0) {
      return; // No parents to navigate back to
    }

    // clear controls
    this.controls = [];
    // Create new array without mutating the original
    const newParentIds = [...this.parentIds];
    const parentId = newParentIds.pop(); // Safely removes and returns last element

    if (parentId != null) {
      // Checks for both undefined and null
      this.parentID = parentId;
      this.parentIds = newParentIds; // Update the array immutably
      this.filteredNavList$ = this.getChildren(this.parentID);
    }
  }

  /**
   * Toggles category menu items
   * @param category Selected category
   * @param event Mouse event
   */
  toggleMenuItems(category: CategoryModel, event: MouseEvent): void {
    event.stopPropagation();
    event.preventDefault();

    // clear controls
    this.controls = [];

    this.parentIds.push(category.parentid);
    this.parentID = category.id;
    this.filteredNavList$ = this.getChildren(category.id);
  }

  /**
   * Handles category selection
   * @param category Selected category
   * @param event Mouse event
   */
  selectItem(
    list: CategoryModel[],
    category: CategoryModel,
    event: MouseEvent
  ): void {
    this.selectedDocument = category;
    this.store.dispatch(toggleCategorySelection({ id: category.id }));
    this.loadDocument(this.selectedDocument.id);
    event.stopPropagation();
  }

  loadDocument(id: number) {
    this.showLoader = true;
    this.categoryService
      .GetInfo({
        ...CATEGORY_QUERY_OBJECT,
        id,
      })
      .pipe()
      .subscribe((response: CategoryReponse) => {
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

            this.initializeControls();
          }

          this.showLoader = false;
        }
      });
  }

  fllterCategories(event: any) {
    this.filteredNavList$ = this.searchByTerm(this.searchTerm);
  }

  /**
   * Handles record deletion
   * @param Record - Blog record to delete
   * @param event - Click event
   */
  deleteRecord(Record: CategoryModel, event: Event): void {
    this.openConfirmationDialog(
      'Delete Record',
      'Are you sure you want to delete selected record',
      () => {
        this.controls = []
        this.store.dispatch(
          actionCategories({
            Entities: [Record],
            actionstatus: 'delete',
          })
        );
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
   * Submits category form
   * @param payload Category data
   */
  submitForm(payload: CategoryModel): void {
    let CultureCategories = payload.culture_categories;
    if (
      payload.culture_categories === undefined ||
      CultureCategories === null ||
      CultureCategories.length === 0
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
      (this.selectedDocument as any)[prop] =
        payload[prop as keyof CategoryModel];
    }

    this.selectedDocument.category_data = {
       id: 0,
       categoryid: this.selectedDocument.id,
       culture: 'en',
       title: payload.title,
       sub_title: '',
       description: ''
    }

    // console.log('Form submitted:', this.selectedDocument);
    this.showLoader = true;
    this.categoryService
      .ProcessRecord(this.selectedDocument)
      .pipe()
      .subscribe((response: CategoryReponse) => {
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
                addCategoriesSuccess({ category: this.selectedDocument })
              );
            } else {
              // update record

              this.store.dispatch(
                updateCategorySuccess({ category: this.selectedDocument })
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

          this.showLoader = false;
        }
      });
  }

  // Get children of specific category
  getChildren(parentId: number): Observable<CategoryModel[]> {
    return this.store.select(selectCategoryChildren(parentId));
  }

  // Filter categories by specific search term
  searchByTerm(searchTerm: string): Observable<CategoryModel[]> {
    return this.store.select(selectFilteredNavList(searchTerm));
  }

  /**
   * Navigates back
   * @param event - Click event
   */
  back(event: Event): void {
    event.stopPropagation();
    this.location.back();
  }
  /* ---------------------------- UTILITY METHODS ---------------------------- */

  /**
   * TrackBy function for ngFor optimization
   * @param index Item index
   * @param item Category item
   * @returns Unique identifier
   */
  getKey(index: number, item: CategoryModel): string {
    return item.id.toString();
  }
}
