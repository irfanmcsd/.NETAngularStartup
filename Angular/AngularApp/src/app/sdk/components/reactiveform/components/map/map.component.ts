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
  EventEmitter,
  ViewChild,
  OnInit,
  inject,
  AfterViewInit,
} from '@angular/core';
import {
  MapInfoWindow,
  MapMarker,
  MapGeocoder,
  GoogleMap,
} from '@angular/google-maps';
import { Store } from '@ngrx/store';
import { NgIf, NgFor } from '@angular/common';
import { Observable, tap } from 'rxjs';

// NgRx Store Selectors and Actions
import { pushSelector } from '../../../../../store_v2/core/event/event.reducers';
import { pushData, CoreEvent } from '../../../../../store_v2/core/event/event.reducers';
import { CoreService } from '../../../../services/coreService';

/**
 * GoogleMapComponent - A reusable Google Maps component with geocoding functionality
 * 
 * Features:
 * - Displays interactive Google Maps
 * - Geocoding address lookup
 * - Marker placement
 * - Info window support
 * - NgRx store integration for address changes
 * - Customizable map options
 */
@Component({
  selector: 'app-googlemap',
  templateUrl: './map.component.html',
  providers: [],
  standalone: true,
  imports: [NgIf, GoogleMap, NgFor, MapMarker, MapInfoWindow],
})
export class GoogleMapComponent implements OnInit, OnChanges, AfterViewInit {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly store = inject(Store);
  private readonly geocoder = inject(MapGeocoder);
  private readonly coreService = inject(CoreService);

  /* ---------------------------- VIEW CHILDREN ---------------------------- */
  @ViewChild(MapInfoWindow) infoWindow!: MapInfoWindow;

  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() options: any = {
    enableMarkerWindow: false,
    width: '750px',
    height: '400px',
    zoom: 11,
    draggable: false,
    map_key: '',
  };

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() onChange = new EventEmitter<any>();

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  push$: Observable<CoreEvent[]> = this.store.select(pushSelector);
  schema: any = {};
  center: google.maps.LatLngLiteral = { lat: 90, lng: 90 };
  markerOptions: google.maps.MarkerOptions = {
    draggable: this.options.draggable,
  };
  markerPositions: google.maps.LatLngLiteral[] = [];
  message = '';
  renderMap = false;
  apiLoaded: any;
  loaded = false;
  address = '';

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */

  /**
   * ngAfterViewInit - Loads Google Maps API after view initialization
   */
  ngAfterViewInit(): void {
    this.loadGoogleMapsAPI();
  }

  /**
   * ngOnInit - Sets up NgRx store subscription for address changes
   */
  ngOnInit(): void {
    this.setupAddressChangeListener();
  }

  /**
   * ngOnChanges - Handles input changes (currently empty implementation)
   */
  ngOnChanges(): void {}

  /* ---------------------------- PUBLIC METHODS ---------------------------- */

  /**
   * addMarker - Adds a marker at the clicked location (currently placeholder)
   * @param event - Google Maps mouse event
   */
  addMarker(event: google.maps.MapMouseEvent): void {
    // Implementation placeholder
    // this.markerPositions.push(event.latLng!.toJSON());
  }

  /**
   * openInfoWindow - Opens info window for a marker
   * @param marker - The marker to show info for
   */
  openInfoWindow(marker: MapMarker): void {
    if (this.options.enableMarkerWindow) {
      this.infoWindow.open(marker);
    }
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */

  /**
   * loadGoogleMapsAPI - Dynamically loads Google Maps JavaScript API
   */
  private loadGoogleMapsAPI(): void {
    this.coreService.loadScript(
      `https://maps.googleapis.com/maps/api/js?key=${this.options.map_key}&libraries=visualization`,
      'google-map',
      () => this.handleAPILoaded()
    );
  }

  /**
   * handleAPILoaded - Callback when Google Maps API is loaded
   */
  private handleAPILoaded(): void {
    this.loaded = true;
  }

  /**
   * setupAddressChangeListener - Listens for address changes via NgRx
   */
  private setupAddressChangeListener(): void {
    this.push$ = this.store.select(pushSelector).pipe(
      tap((event: CoreEvent[]) => {
        if (event.length > 0 && event[0].action === 'map-render') {
          this.handleAddressChange(event[0].data);
        }
      })
    );
    this.push$.subscribe();
  }

  /**
   * handleAddressChange - Processes address change events
   * @param addressdata - Address data from store
   */
  private handleAddressChange(addressdata: any): void {
    if (addressdata?.key === 'address' && addressdata.value?.length > 2) {
      if (addressdata.value !== this.address) {
        this.address = addressdata.value;
        this.processMap();
        this.clearStoreEvent();
      }
    }
  }

  /**
   * clearStoreEvent - Clears the current store event
   */
  private clearStoreEvent(): void {
    this.store.dispatch(pushData({ event: [] }));
  }

  /**
   * processMap - Geocodes the current address and updates the map
   */
  private processMap(): void {
    this.geocoder.geocode({ address: this.address })
      .subscribe(({ results, status }) => {
        if (status === google.maps.GeocoderStatus.OK) {
          this.updateMapWithGeocode(results[0]);
        } else {
          this.handleGeocodeError(status);
        }
      });
  }

  /**
   * updateMapWithGeocode - Updates map with geocoded location
   * @param result - Geocoding result
   */
  private updateMapWithGeocode(result: google.maps.GeocoderResult): void {
    const location = result.geometry.location;
    const coordinates = { lat: location.lat(), lng: location.lng() };

    this.center = coordinates;
    this.markerPositions = [coordinates];
    this.renderMap = true;
    this.onChange.emit(coordinates);
  }

  /**
   * handleGeocodeError - Handles geocoding errors
   * @param status - Geocoding status code
   */
  private handleGeocodeError(status: google.maps.GeocoderStatus): void {
    this.message = `Unable to geocode address: ${status}`;
    console.error(this.message);
  }
}