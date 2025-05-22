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
  OnChanges,
  EventEmitter
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgFor } from "@angular/common";

/**
 * MultiOptionsComponent - A dynamic multi-option input component
 * 
 * Features:
 * - Manages a list of input options
 * - Allows adding and removing options
 * - Tracks changes and emits updates
 * - Supports both existing and new options
 * - Simple and lightweight implementation
 */
@Component({
  selector: "app-multiple-options",
  templateUrl: "./multioptions.html",
  providers: [],
  standalone: true,
  imports: [NgFor, FormsModule]
})
export class MultiOptionsComponent implements OnChanges {
  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() options: any[] = [];          // Array of options to display/manage
  @Input() control: any = {};            // Form control reference

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() onChange = new EventEmitter<any[]>(); // Emits updated options array

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnChanges - Handles input property changes
   * @param changes - SimpleChanges object (not currently used)
   */
  ngOnChanges(): void {
    console.log('Input changes detected:', this.options);
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */
  
  /**
   * AddOption - Adds a new option to the list
   * @param event - Mouse event
   */
  addOption(event: MouseEvent): void {
    const newOption = {
      id: this.generateNextId(),
      value: "",
      isnew: true
    };
    
    this.options.push(newOption);
    this.emitOptionsUpdate();
    event.stopPropagation();
  }

  /**
   * removeOption - Removes an option after confirmation
   * @param option - The option to remove
   * @param index - Position in options array
   * @param event - Mouse event
   */
  removeOption(option: any, index: number, event: MouseEvent): void {
    if (confirm('Are you sure you want to delete this item?')) {
      this.options.splice(index, 1);
      this.emitOptionsUpdate();
    }
    event.stopPropagation();
  }

  /**
   * onSearchChange - Handles option value changes
   * @param id - ID of the changed option
   * @param event - Input event
   */
  onSearchChange(id: number, event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const option = this.options.find(item => item.id === id);
    
    if (option) {
      option.value = inputElement.value;
      this.emitOptionsUpdate();
    }
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */
  
  /**
   * generateNextId - Generates the next available ID for new options
   * @returns The next available ID
   */
  private generateNextId(): number {
    return this.options.length > 0 
      ? Math.max(...this.options.map(o => o.id)) + 1 
      : 1;
  }

  /**
   * emitOptionsUpdate - Emits the current options array
   */
  private emitOptionsUpdate(): void {
    this.onChange.emit([...this.options]);
  }
}