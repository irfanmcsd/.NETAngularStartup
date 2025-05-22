/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { NgFor, NgClass, NgIf } from '@angular/common';

/**
 * ChipSelectorComponent - A flexible chip/tag selector component
 * 
 * Features:
 * - Supports both single and multiple selection modes
 * - Toggle selection with visual feedback
 * - Emits selected values to parent components
 * - Customizable through input options
 * - Lightweight and standalone
 */
@Component({
  selector: 'app-chips',
  templateUrl: './chips.html',
  styleUrls: ['./chips.css'],
  providers: [],
  standalone: true,
  imports: [NgFor, NgClass, NgIf],
})
export class ChipSelectorComponent implements OnInit {
  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() options: any = {};              // General component configuration
  @Input() categoryOptions: any = {        // Selection configuration
    options: [],                          // Array of available options
    selection: 0,                         // 0 = single selection, 1 = multiple selection
  };

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() onChange = new EventEmitter<any>(); // Emits when selection changes

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showLoader: boolean = false;            // Loading indicator (currently unused)
  filterBy: string = '';                  // Filter text (currently unused)
  selectedCategories: any[] = [];         // Currently selected options

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Initializes the component
   * - Processes initial selected options
   * - Sets up default selections
   */
  ngOnInit(): void {
    this.initializeSelections();
    this.getSelectedOptions();
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */
  
  /**
   * toggleSelection - Toggles the selection state of an option
   * @param option - The option to toggle
   * @param event - The mouse event
   */
  toggleSelection(option: any, event: MouseEvent): void {
    if (this.isSingleSelectionMode()) {
      this.handleSingleSelection(option);
    } else {
      this.handleMultiSelection(option);
    }

    this.getSelectedOptions();
    event.stopPropagation();
  }

  /**
   * getKey - TrackBy function for ngFor optimization
   * @param index - The item index
   * @param item - The current item
   * @returns A unique identifier for the item
   */
  getKey(index: number, item: any): string {
    return item.id;
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */
  
  /**
   * initializeSelections - Sets initial selected options based on input
   */
  private initializeSelections(): void {
    if (this.options?.length > 0) {
      this.categoryOptions.options.forEach((opt: any) => {
        opt.selected = this.options.some((selectedOpt: any) => selectedOpt === opt.id);
      });
    }
  }

  /**
   * getSelectedOptions - Updates and emits the current selection
   */
  private getSelectedOptions(): void {
    this.selectedCategories = this.categoryOptions.options
      .filter((option: any) => option.selected);

    if (this.selectedCategories.length > 0) {
      this.emitSelection();
    }
  }

  /**
   * emitSelection - Emits the current selection based on selection mode
   */
  private emitSelection(): void {
    const value = this.isSingleSelectionMode()
      ? this.selectedCategories[0].id      // Single selection: emit ID
      : this.selectedCategories;          // Multiple selection: emit array

    this.onChange.emit(value);
  }

  /**
   * handleSingleSelection - Handles selection logic for single-select mode
   * @param option - The newly selected option
   */
  private handleSingleSelection(option: any): void {
    this.categoryOptions.options.forEach((opt: any) => {
      opt.selected = opt.id === option.id;
    });
  }

  /**
   * handleMultiSelection - Handles selection logic for multi-select mode
   * @param option - The toggled option
   */
  private handleMultiSelection(option: any): void {
    option.selected = !option.selected;
  }

  /**
   * isSingleSelectionMode - Checks if component is in single selection mode
   * @returns True if in single selection mode
   */
  private isSingleSelectionMode(): boolean {
    return this.categoryOptions.selection === 0;
  }
}
