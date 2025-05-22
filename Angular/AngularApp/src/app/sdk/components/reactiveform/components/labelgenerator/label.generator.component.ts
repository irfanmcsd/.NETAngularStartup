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
  OnInit,
  OnChanges,
  EventEmitter,
  inject,
} from '@angular/core';
import { Store } from '@ngrx/store';

// Services
import { DataService } from './services/data.service';

// NgRx Store Selectors and Actions
import { pushSelector } from '../../../../../store_v2/core/event/event.reducers';
import { pushData, CoreEvent } from '../../../../../store_v2/core/event/event.reducers';

// Angular Modules
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Observable, tap } from 'rxjs';

/**
 * LabelGeneratorComponent - A component for generating and validating slugs/labels
 * 
 * Features:
 * - Converts text input to URL-friendly slugs
 * - Performs server-side validation of generated slugs
 * - Supports edit/cancel functionality
 * - Integrates with NgRx store for event handling
 */
@Component({
  selector: 'app-label-generator',
  templateUrl: './labelgenerator.html',
  providers: [DataService],
  styleUrls: ['./label.css'],
  standalone: true,
  imports: [NgIf, FormsModule],
})
export class LabelGeneratorComponent implements OnInit, OnChanges {
  // Dependency Injection
  private store = inject(Store);
  constructor(private dataService: DataService) {}

  /* ---------------------------- INPUT/OUTPUT ---------------------------- */
  @Input() options: any[] = []; // Configuration options for the component
  @Output() onChange = new EventEmitter<any>(); // Emits when slug changes

  /* ---------------------------- COMPONENT STATE ---------------------------- */
  slug: string = ''; // The generated/validated slug
  type: number = 0; // Type of label (from options)
  editMode: boolean = false; // Tracks if in edit mode
  isUpdate: boolean = false;
  /* ---------------------------- STORE OBSERVABLES ---------------------------- */
  // Observable for push events from NgRx store
  push$: Observable<CoreEvent[]> = this.store.select(pushSelector);

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */
  
  /**
   * ngOnInit - Component initialization
   * - Sets up subscription to push events from store
   * - Handles text-focus events to auto-generate slugs
   */
  ngOnInit(): void {
    this.push$ = this.store.select(pushSelector).pipe(
      tap((event: CoreEvent[]) => {
        if (event.length > 0) {
          // Handle text-focus events
          if (
            event[0].action === 'text-focus' &&
            event[0].data !== undefined &&
            event[0].data !== null
          ) {
            if (event[0].data.value.length > 2) {
              // Convert to slug and validate
              this.slug = this.convertToSlug(event[0].data.value);
              this.ServerSideValidation();
              
              // Clear push data after processing
              this.store.dispatch(
                pushData({
                  event: [],
                })
              );
            }
          }
        }
      })
    );
    this.push$.subscribe();
  }

  /**
   * ngOnChanges - Handles input changes
   * - Updates component state when options change
   * - Emits change event if in update mode
   */
  ngOnChanges() {
    if (this.options.length > 0) {
      this.type = this.options[0].type;
      if (this.options[0].isupdate) {
        this.isUpdate = this.options[0].isupdate;
        this.slug = this.options[0].slug;
        this.onChange.emit(this.slug);
      }
    }
    
  }

  /* ---------------------------- CORE FUNCTIONALITY ---------------------------- */
  
  /**
   * ServerSideValidation - Validates the slug with backend
   * - Calls data service to validate slug
   * - Updates slug with validated version
   * - Emits change event
   */
  ServerSideValidation() {
    this.dataService
      .ValidateLabel(this.slug, this.type)
      .pipe()
      .subscribe((data: any) => {
        this.slug = data.slug;
        this.onChange.emit(this.slug);
      });
  }

  /**
   * convertToSlug - Converts text to URL-friendly slug
   * @param text - Input text to convert
   * @returns Generated slug string
   */
  convertToSlug(text: string): string {
    return text
      .toLowerCase()
      .replace(/[^\w\-. ]+/g, '') // Remove special chars
      .replace(/ +/g, '-'); // Replace spaces with hyphens
  }

  /* ---------------------------- UI EVENT HANDLERS ---------------------------- */
  
  /**
   * changeSlug - Enters edit mode
   * @param event - Mouse event
   */
  changeSlug(event: MouseEvent) {
    this.editMode = true;
    event.stopPropagation();
  }

  /**
   * update - Updates slug after edit
   * - Converts to slug format
   * - Validates with server
   * - Exits edit mode
   * @param event - Mouse event
   */
  update(event: MouseEvent) {
    this.slug = this.convertToSlug(this.slug);
    this.ServerSideValidation();
    this.editMode = false;
    event.stopPropagation();
  }

  /**
   * cancel - Cancels edit mode without saving changes
   * @param event - Mouse event
   */
  cancel(event: MouseEvent) {
    this.editMode = false;
    event.stopPropagation();
  }
}