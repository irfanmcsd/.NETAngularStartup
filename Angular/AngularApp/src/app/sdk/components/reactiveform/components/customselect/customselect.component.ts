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
import { FormsModule } from "@angular/forms";

/**
 * CustomSelectComponent - A customizable numeric input selector component
 * 
 * Features:
 * - Displays a numeric value with optional label
 * - Provides increment/decrement functionality
 * - Minimum value constraint (0)
 * - Emits changes to parent components
 * - Simple and lightweight implementation
 */
@Component({
  selector: "app-input-selector",
  templateUrl: "./customselect.html",
  styleUrls: ["./customselect.css"],
  providers: [],
  standalone: true,
  imports: [FormsModule],
})
export class CustomSelectComponent implements OnInit {
  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() options: any = {};              // General component configuration
  @Input() categoryOptions: any = {};      // Value and display configuration

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() onChange = new EventEmitter<any>(); // Emits when value changes

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  showLoader: boolean = false;            // Loading indicator (currently unused)
  input_value: string = 'None';           // Formatted display value

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Initializes the component
   * - Sets the initial display value
   */
  ngOnInit(): void {
    this.setInputValue();
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */
  
  /**
   * upValue - Increments the current value
   */
  upValue(): void {
    this.categoryOptions.value++;
    this.setInputValue();
  }

  /**
   * downValue - Decrements the current value (with minimum of 0)
   */
  downValue(): void {
    this.categoryOptions.value = Math.max(0, this.categoryOptions.value - 1);
    this.setInputValue();
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */
  
  /**
   * setInputValue - Updates the display value and emits changes
   */
  private setInputValue(): void {
    this.input_value = this.formatDisplayValue();
    this.onChange.emit(this.categoryOptions.value);
  }

  /**
   * formatDisplayValue - Formats the value for display
   * @returns The formatted display string
   */
  private formatDisplayValue(): string {
    if (this.categoryOptions.value === 0) {
      return 'None';
    }
    return `${this.categoryOptions.value} ${this.categoryOptions.label || ''}`.trim();
  }
}
