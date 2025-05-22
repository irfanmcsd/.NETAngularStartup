import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  inject,
} from '@angular/core';
import { FormBase } from '../../model/base';

// Models and Services
import {
  ICATEGORY,
  CATEGORY_QUERY_OBJECT,
  CategoryReponse,
} from '../../../../../store_v2/category/model';
import { CategoryService } from '../../../../../store_v2/category/category.services';
import { FetchColumnOptions } from '../../../../../configs/query.types';
import { CoreService } from '../../../../services/coreService';

// Angular Modules
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgIf, NgFor } from '@angular/common';

// Components
import { LoaderComponent } from '../../../loader/loader.component';

/**
 * MultiCategoryComponent - A component for selecting multiple categories with hierarchical support
 * 
 * Features:
 * - Supports both flat and hierarchical category structures
 * - Integrates with ng-select for multi-selection
 * - Handles both manual and automatic data loading
 * - Provides customizable display options
 * - Emits selection changes to parent components
 */
@Component({
  selector: 'app-multi-category',
  templateUrl: './multicategory.html',
  providers: [CategoryService],
  standalone: true,
  imports: [NgIf, LoaderComponent, NgSelectModule, FormsModule, NgFor],
})
export class MultiCategoryComponent implements OnInit {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly categoryDataService = inject(CategoryService);
  private readonly coreService = inject(CoreService);

  /* ---------------------------- INPUT/OUTPUT PROPERTIES ---------------------------- */
  @Input() categoryOptions: any = {};      // Configuration options for the component
  @Input() control!: FormBase<any>;       // Form control instance
  @Input() hasErrors: any;                // Error state indicator
  @Output() onChange = new EventEmitter<any>(); // Emits when selection changes

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showLoader = false;                     // Loading indicator
  selectedCar: number = 1;                // Example property (consider removing if unused)


  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Initializes the component
   * - Loads category data automatically unless configured otherwise
   */
  ngOnInit(): void {
    if (this.shouldLoadData()) {
      this.loadActionData();
    }
  }

  /* ---------------------------- DATA LOADING METHODS ---------------------------- */
  
  /**
   * loadActionData - Loads category data from the server
   * - Configures query based on component options
   * - Formats data based on display requirements
   */
  loadActionData(): void {
    let Query: ICATEGORY = Object.assign({}, CATEGORY_QUERY_OBJECT);
    Query.order = 'category.priority desc';
    Query.loadall = true;
    Query.type = this.categoryOptions.categorytype;
    
    this.showLoader = true;
    this.categoryDataService
      .LoadRecords(Query)
      .pipe()
      .subscribe((data: CategoryReponse) => {
        if (data.posts !== undefined) {

          let filterData : any[] = []
          for (let item of data.posts) {
             filterData.push({
                id: item.id,
                title: item.category_data.title,
                parentid: item.parentid
             })
          }
          const navList = this.categoryDataService.prepareParentChildCategories(filterData, 0);
          const values = navList.map(category => ({
            key: category.id,
            value: category.title,
          }));
         
          this.control.options = values;
          
          // Delay setting dropdownList to ensure ng-select initialization
          setTimeout(() => {
            this.control.multiselectOptions.dropdownList = values;
          }, 2000);
        
        }
        this.showLoader = false;
      });
  }
 
  /* ---------------------------- EVENT HANDLERS ---------------------------- */
  
  /**
   * onItemSelect - Handles item selection changes
   * @param event - Selection event
   */
  onItemSelect(event: any): void {
    console.log('selected item: ' , this.control.categoryOptions)
    this.onChange.emit(this.control.categoryOptions);
  }

  /* ---------------------------- UTILITY METHODS ---------------------------- */
  
  /**
   * shouldLoadData - Determines if data should be loaded automatically
   * @returns Boolean indicating if data should be loaded
   */
  private shouldLoadData(): boolean {
    return this.categoryOptions.manualload === undefined || 
           !this.categoryOptions.manualload;
  }

  /**
   * getKey - TrackBy function for ngFor optimization
   * @param index - Item index
   * @param item - Current item
   * @returns Unique key for the item
   */
  getKey(index: number, item: any): string {
    return item.key;
  }
}