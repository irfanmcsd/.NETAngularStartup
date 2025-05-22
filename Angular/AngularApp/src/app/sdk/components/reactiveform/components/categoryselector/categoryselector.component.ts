import {
  Component,
  Input,
  Output,
  OnChanges,
  EventEmitter,
  inject
} from "@angular/core";

// Models and Services
import {
  ICATEGORY,
  CATEGORY_QUERY_OBJECT,
  CategoryReponse
} from '../../../../../store_v2/category/model';
import { CategoryService } from '../../../../../store_v2/category/category.services';
import { FetchColumnOptions } from '../../../../../configs/query.types';

// Angular Modules
import { FormsModule } from "@angular/forms";
import { NgIf, NgFor, NgClass } from "@angular/common";

// Components
import { LoaderComponent } from "../../../loader/loader.component";

/**
 * CategorySelectorComponent - A hierarchical category selection component
 * 
 * Features:
 * - Loads categories based on parent-child relationships
 * - Supports multi-level category selection
 * - Implements search/filter functionality
 * - Manages loading states during data fetch
 * - Emits selected categories to parent components
 */
@Component({
  selector: "app-category-selector",
  templateUrl: "./categoryselector.html",
  styleUrls: ["./categoryselector.css"],
  providers: [CategoryService],
  standalone: true,
  imports: [
    NgIf,
    LoaderComponent,
    NgFor,
    FormsModule,
    NgClass,
  ],
})
export class CategorySelectorComponent implements OnChanges {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly categoryDataService = inject(CategoryService);

  /* ---------------------------- INPUT/OUTPUT PROPERTIES ---------------------------- */
  @Input() options: any = {};              // General component options
  @Input() categoryOptions: any = {};      // Category-specific options
  @Output() onChange = new EventEmitter<any>(); // Emits selected categories

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showLoader: boolean = false;             // Loading indicator
  categories: any[] = [];                 // All loaded categories
  filterBy: string = '';                  // Current search filter text
  categoryselection: any[] = [];          // Hierarchical category structure
  selectedCategories: any[] = [];         // Currently selected categories
  filterargs: any = { title: '' };        // Filter arguments for search

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnChanges - Handles input changes
   * @param changes - SimpleChanges object containing changed inputs
   */
  ngOnChanges(changes: any): void {
    if (changes.categoryOptions !== undefined) {
      this.resetCategorySelection();
      this.loadCategories(-1, this.categoryOptions.type);
    }
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */
  
  /**
   * onSearchChange - Updates filter when search text changes
   */
  onSearchChange(): void {
    this.filterargs.title = this.filterBy;
  }

  /**
   * onCategorySelect - Handles category selection and emits changes
   */
  onCategorySelect(): void {
    this.selectedCategories = [];
    
    // Collect all selected categories
    for (let box of this.categoryselection) {
      for (let cat of box.categories) {
        if (cat.selected) {
          this.selectedCategories.push({ 
            id: cat.id,
            term: cat.term,
            sub_term: cat.sub_term,
            title: cat.title
          });
        }
      }
    }

    this.onChange.emit(this.selectedCategories);
  }

  /* ---------------------------- CATEGORY LOADING METHODS ---------------------------- */
  
  /**
   * loadCategories - Loads categories from the server
   * @param parentid - Parent category ID (-1 for root)
   * @param _type - Category type filter (default: 100)
   */
  loadCategories(parentid: number, _type: number = 100): void {
    this.categories = []; // Reset before loading

    let Query: ICATEGORY = Object.assign({}, CATEGORY_QUERY_OBJECT);
    Query.parentid = parentid;
    Query.order = 'category.priority desc';
    Query.loadall = true;
    Query.column_options = FetchColumnOptions.Dropdown;
    Query.iscache = true;
    Query.type = _type;
   
    this.showLoader = true;
    this.categoryDataService
      .LoadRecords(Query)
      .pipe()
      .subscribe((data: CategoryReponse) => {

        if ((data.posts ?? []).length > 0) {
          this.categories = data.posts ?? [];
          this.filterCategories(0); // Start with root categories
        }
        this.showLoader = false;
      });
  }

  /* ---------------------------- CATEGORY FILTERING METHODS ---------------------------- */
  
  /**
   * filter - Handles category selection and filters child categories
   * @param categories - Array of categories to filter
   * @param category - Selected category
   * @param index - Current level in hierarchy
   * @param event - Mouse event
   */
  filter(categories: any[], category: any, index: number, event: MouseEvent): void {
    // Update selection state
    categories.forEach(cat => {
      cat.selected = (cat.id === category.id);
    });
  
    // Clean up deeper levels if they exist
    if (this.categoryselection.length > index + 1) {
      this.categoryselection.splice(index + 1);
    }

    // Load child categories if needed
    if (category.hasChilds) {
      this.filterCategories(category.id);
    }
  
    this.onCategorySelect();
    event.stopPropagation();
  }

  /**
   * filterCategories - Filters categories by parent ID and builds hierarchy
   * @param parentid - Parent category ID to filter by
   */
  filterCategories(parentid: number): void {
    const categories = this.categories
      .filter(category => category.parentid === parentid);
     
    if (categories.length > 0) {
      // Check for child categories
      categories.forEach(category => {
        category.hasChilds = this.categories.some(cat => cat.parentid === category.id);
      });
      
      this.categoryselection.push(this.initChilds(categories));
    }
  }

  /* ---------------------------- UTILITY METHODS ---------------------------- */
  
  /**
   * initChilds - Initializes a category group object
   * @param categories - Categories to include in the group
   * @returns Category group object
   */
  private initChilds(categories: any[]): any {
    return {
      filterBy: '',
      categories: categories
    };
  }

  /**
   * resetCategorySelection - Resets the category selection state
   */
  private resetCategorySelection(): void {
    this.categoryselection = [];
  }

  /**
   * filterByTerm - Filters categories by search term
   * @param categories - Categories to filter
   * @param filterBy - Search term
   * @returns Filtered categories
   */
  filterByTerm(categories: any[], filterBy: string): any[] {
    return categories.filter(category => this.hideCategory(category, filterBy));
  }

  /**
   * hideCategory - Determines if category matches search term
   * @param category - Category to check
   * @param filterBy - Search term
   * @returns Boolean indicating match
   */
  private hideCategory(category: any, filterBy: string): boolean {
    return category.title.toLowerCase().includes(filterBy.toLowerCase());
  }

  /**
   * getKey - TrackBy function for ngFor optimization
   * @param index - Item index
   * @param item - Category item
   * @returns Unique identifier for the item
   */
  getKey(index: number, item: any): string {
    return item.id;
  }
}