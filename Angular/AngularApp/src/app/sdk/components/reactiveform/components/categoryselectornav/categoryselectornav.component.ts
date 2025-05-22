/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit
} from "@angular/core";
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { NgIf, NgFor } from "@angular/common";

// Components
import { SelectorModalComponent } from "./View.component";

/**
 * CategorySelectorNavComponent - A component for selecting categories through a modal interface
 * 
 * Features:
 * - Opens a modal dialog for category selection
 * - Manages selected categories list
 * - Provides remove functionality for selected items
 * - Emits changes to parent components
 * - Customizable labels and behavior
 */
@Component({
  selector: "app-category-selector-nav",
  templateUrl: "./categoryselectornav.html",
  providers: [],
  standalone: true,
  imports: [NgIf, NgFor],
})
export class CategorySelectorNavComponent implements OnInit {
  /* ---------------------------- INPUT/OUTPUT PROPERTIES ---------------------------- */
  @Input() options: any = {};              // General component configuration
  @Input() categoryOptions: any = {};      // Category-specific configuration
  @Output() onChange = new EventEmitter<any>(); // Emits when selection changes

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  isDisabled: boolean = false;            // Disabled state during modal operations
  Selection: any[] = [];                 // Currently selected categories

  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  constructor(private modalService: NgbModal) {}

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Initializes component with default values
   * - Sets default labels if not provided
   */
  ngOnInit(): void {
    this.setDefaultLabels();
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */
  
  /**
   * remove - Removes a category from selection
   * @param obj - Category object to remove
   * @param index - Position in selection array
   * @param event - Mouse event
   */
  remove(obj: any, index: number, event: MouseEvent): void {
    this.Selection.splice(index, 1);
    this.emitCurrentSelection();
    event.stopPropagation();
  }

  /**
   * openModal - Opens the category selection modal
   * @param event - Mouse event
   */
  openModal(event: MouseEvent): void {
    this.isDisabled = true;
    
    const modalOptions: NgbModalOptions = {
      backdrop: false,
      size: 'lg',
    };

    const modalRef = this.modalService.open(SelectorModalComponent, modalOptions);
    
    // Configure modal content
    modalRef.componentInstance.Info = {
      title: 'Select Category',
      data: this.categoryOptions,
    };

    // Handle modal results
    modalRef.result.then(
      (result) => this.handleModalSuccess(result),
      (dismissed) => this.handleModalDismiss()
    );

    event.stopPropagation();
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */
  
  /**
   * setDefaultLabels - Sets default labels if none provided
   */
  private setDefaultLabels(): void {
    if (!this.categoryOptions.label) {
      this.categoryOptions.label = 'Select Category';
    }
    if (!this.categoryOptions.selected_label) {
      this.categoryOptions.selected_label = 'Selected Category';
    }
  }

  /**
   * handleModalSuccess - Processes successful modal completion
   * @param result - Modal result containing selected categories
   */
  private handleModalSuccess(result: any): void {
    this.Selection = result.data;
    this.emitCurrentSelection();
    this.isDisabled = false;
  }

  /**
   * handleModalDismiss - Handles modal dismissal
   */
  private handleModalDismiss(): void {
    this.isDisabled = false;
    console.log('Modal dismissed');
  }

  /**
   * emitCurrentSelection - Emits current selection to parent component
   */
  private emitCurrentSelection(): void {
    const categoryIds = this.Selection.map((item: any) => item.id);
    this.onChange.emit(categoryIds);
  }
}
