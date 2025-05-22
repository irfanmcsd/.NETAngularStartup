/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CoreService } from '../../../../services/coreService';
import { AppConfig } from "../../../../../configs/app.configs";
import { CategorySelectorComponent } from '../categoryselector/categoryselector.component';

/**
 * SelectorModalComponent - A reusable modal component for category selection
 * 
 * Features:
 * - Displays a modal dialog with category selection functionality
 * - Integrates with CategorySelectorComponent for hierarchical selection
 * - Provides form submission and cancellation handling
 * - Configurable title and control properties
 * - Clean modal lifecycle management
 */
@Component({
  selector: 'app-cat-selector-modal',
  templateUrl: './view.html',
  providers: [CoreService, AppConfig],
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CategorySelectorComponent
  ],
})
export class SelectorModalComponent implements OnInit {
  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() Info: any;                   // Configuration object for the modal

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  title: string = '';                   // Modal title
  showLoader: boolean = false;          // Loading state indicator
  controlKey: string = 'categoryid';    // Default form control key
  controlValue: string = '';            // Currently selected value

  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  constructor(
    public activeModal: NgbActiveModal, // NgbActiveModal for modal control
    private coreService: CoreService    // Core application service
  ) {}

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Initializes the component
   * - Sets the modal title from input configuration
   */
  ngOnInit(): void {
    this.title = this.Info?.title || '';
  }

  /* ---------------------------- EVENT HANDLERS ---------------------------- */
  
  /**
   * changeOption - Handles category selection changes
   * @param option - The selected category option
   */
  changeOption(option: any): void {
    this.controlValue = option;
    console.log('Selected option:', this.controlValue);
  }

  /**
   * SubmitForm - Handles form submission
   * @param payload - The form payload containing selected categories
   */
  SubmitForm(payload: any): void {
    this.activeModal.close({
      data: payload.categorylist_arr
    });
  }

  /**
   * close - Handles modal dismissal
   */
  close(): void {
    this.activeModal.dismiss('Cancel Clicked');
  }

  /* ---------------------------- UTILITY METHODS ---------------------------- */
  
  /**
   * getKey - TrackBy function for ngFor optimization
   * @param index - Item index
   * @param item - Current item
   * @returns Unique identifier for the item
   */
  getKey(index: number, item: any): string {
    return item.id;
  }
}
