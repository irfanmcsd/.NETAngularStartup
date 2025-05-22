/*import {
  Component,
  Input,
  Output,
  EventEmitter,
  AfterViewChecked,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { FormBase } from '../../model/base';

// Models and Services
import {
  ILOCATION,
  LOCATION_QUERY_OBJECT,
} from '../../../../../store_v2/location/model';
import { LocationService } from '../../../../../store_v2/location/location.services';
import { CoreService } from '../../../../services/coreService';

// Third-party Modules
import { AutocompleteLibModule } from 'angular-ng-autocomplete';

/**
 * AutoCompleteComponent - A reusable autocomplete component with location search functionality
 *
 * Features:
 * - Integrates with angular-ng-autocomplete for UI
 * - Supports location search with customizable queries
 * - Handles selection and clearing of values
 * - Manages loading states during search
 * - Works with form controls for seamless form integration
 */
/*@Component({
  selector: 'app-autocomplete',
  templateUrl: './autocomplete.html',
  providers: [LocationService, CoreService],
  standalone: true,
  imports: [AutocompleteLibModule],
})
export class AutoCompleteComponent implements AfterViewChecked {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  /*private readonly locationDataService = inject(LocationService);
  private readonly coreService = inject(CoreService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  /* ---------------------------- INPUT/OUTPUT PROPERTIES ---------------------------- */
 /* @Input() control!: FormBase<any>; // Form control instance
  @Input() hasErrors: any; // Error state indicator
  @Output() onChange = new EventEmitter<any>(); // Emits when selection changes

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */

  /**
   * ngAfterViewChecked - Ensures proper change detection
   * Note: This might be performance intensive - consider if really needed
   */
  /*ngAfterViewChecked(): void {
    this.changeDetectorRef.detectChanges();
  }

  /* ---------------------------- CORE FUNCTIONALITY ---------------------------- */

  /**
   * fetchlocation - Fetches location data based on search text
   * @param control - The form control instance
   * @param searchText - Text to search for locations
   */
 /* fetchlocation(control: FormBase<any>, searchText: string): void {
    // Prepare query object with default values
    let Query: ILOCATION = Object.assign({}, LOCATION_QUERY_OBJECT);

    // Configure query parameters
    Query.order = 'title asc';
    Query.start_search_key = searchText;
    Query.pagesize = 6;
    Query.loadall = false;

    // Handle country-only search
    if (this.isCountryOnlySearch(control)) {
      Query.parentid = 0; // Only top-level countries
    } else {
      Query.parentid = -1; // All locations
    }

    // Filter by country ISO code if specified
    if (this.hasCountryFilter(control)) {
      Query.country = control.autocompleteOptions.iso2;
    }

    // Set loading state
    control.autocompleteOptions.isLoading = true;

    // Execute search
    this.locationDataService
      .GetAutoComplete(Query)
      .pipe()
      .subscribe((data: any) => {
        this.coreService.bindLocationData(control, data.posts);
      });
  }

  /* ---------------------------- EVENT HANDLERS ---------------------------- */

  /**
   * selectEvent - Handles item selection from autocomplete
   * @param item - Selected item
   */
  /*selectEvent(item: any): void {
    this.control.value = item;
    this.onChange.emit(this.control.value);
  }

  /**
   * onChangeSearch - Handles search text changes
   * @param text - Current search text
   */
  /*onChangeSearch(text: string): void {
    this.control.autocompleteOptions.initialValue = null;
    this.fetchlocation(this.control, text);
  }

  
  /**
   * clearEvent - Clears current selection
   * @param item - Item to clear (unused parameter)
   */
  /*clearEvent(item: any): void {
    this.control.value = [];
    this.onChange.emit(this.control.value);
  }

  /* ---------------------------- UTILITY METHODS ---------------------------- */

  /**
   * getKey - TrackBy function for ngFor optimization
   * @param index - Item index
   * @param item - Current item
   * @returns Unique key for the item
   */
  /*getKey(index: number, item: any): string {
    return item.key;
  }

  /**
   * isCountryOnlySearch - Checks if search should be limited to countries only
   * @param control - Form control instance
   * @returns Boolean indicating country-only search
   */
  /*private isCountryOnlySearch(control: FormBase<any>): boolean {
    return (
      control.autocompleteOptions.countryonly !== undefined &&
      control.autocompleteOptions.countryonly !== null &&
      control.autocompleteOptions.countryonly
    );
  }

  /**
   * hasCountryFilter - Checks if country filter is applied
   * @param control - Form control instance
   * @returns Boolean indicating country filter presence
   */
  /*private hasCountryFilter(control: FormBase<any>): boolean {
    return (
      control.autocompleteOptions.iso2 !== undefined &&
      control.autocompleteOptions.iso2 !== null &&
      control.autocompleteOptions.iso2 !== ''
    );
  }
}*/
