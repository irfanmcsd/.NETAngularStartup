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
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, JsonPipe, NgFor } from '@angular/common';
import { QuillModule } from 'ngx-quill';
import { EntityFactoryService } from './category.service';

@Component({
  selector: 'app-culture-category',
  templateUrl: './category.html',
  styleUrls: ['./category.css'],
  providers: [EntityFactoryService],
  standalone: true,
  imports: [NgIf, NgFor, JsonPipe, FormsModule, QuillModule],
})
export class CultureCategoryComponent implements OnChanges {
  entityFactory = inject(EntityFactoryService);

  /* ---------------------------- INPUT PROPERTIES ---------------------------- */
  @Input() options: any = {
    type: 0, // 0: category, 1: blog
    cultures: [],
    data: [],
  };
  @Input() tinymiceOptions: any;

  /* ---------------------------- OUTPUT EVENTS ---------------------------- */
  @Output() onChange = new EventEmitter<any[]>(); // Emits updated options array

  /* ---------------------------- LIFECYCLE HOOKS ---------------------------- */

  /**
   * ngOnChanges - Handles input property changes
   * @param changes - SimpleChanges object (not currently used)
   */
  ngOnChanges(): void {
    this.emitOptionsUpdate();
  }

  /* ---------------------------- PUBLIC METHODS ---------------------------- */

  /**
   * AddOption - Adds a new option to the list
   * @param event - Mouse event
   */
  addOption(event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    const entityType = this.options.type === 1 ? 'blog' : 'category';
    const newOption = this.entityFactory.createEntity(entityType);

    this.options.data.push(newOption);
    this.emitOptionsUpdate();
  }
 

  /**
   * removeOption - Removes an option after confirmation
   * @param index - Position in options array
   * @param event - Mouse event
   */
  removeOption(index: number, event: MouseEvent): void {
    if (confirm('Are you sure you want to delete this item?')) {
      this.options.data.splice(index, 1);
      this.emitOptionsUpdate();
    }
    event.stopPropagation();
  }

  /**
   * emitOptionsUpdate - Emits the current options array
   */
  private emitOptionsUpdate(): void {
    this.onChange.emit([...this.options.data]);
  }
}
