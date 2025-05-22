/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CoreService } from '../../services/coreService';
import { CdkDragDrop, CdkDragEnter, CdkDragMove, moveItemInArray, CdkDropList, CdkDrag } from '@angular/cdk/drag-drop';
import { orderBy } from 'lodash';
import { FormsModule } from '@angular/forms';
import { NgFor } from '@angular/common';
@Component({
    selector: 'app-column-builder',
    templateUrl: './column.html',
    styles: [
        `
      div.scroll {
        margin: 4px, 4px;
        padding: 4px;
        width: 100%;
        height: 500px;
        overflow-x: hidden;
        overflow-y: auto;
        text-align: justify;
      }
      .listitem-box {
        cursor: move;
      }
      ::ng-deep .cdk-drag-preview {
        display: table;
      }

      ::ng-deep .cdk-drag-placeholder {
        opacity: 0;
      }
    `,
    ],
    providers: [],
    standalone: true,
    imports: [
        CdkDropList,
        NgFor,
        CdkDrag,
        FormsModule,
    ],
})
export class ColumnBuilderComponent implements OnInit {
  @Input() Info: any;
  title: string = '';
  Columns: any[] = [];
  @ViewChild('dropListContainer') dropListContainer?: ElementRef;

  constructor(
    public activeModal: NgbActiveModal,
    private coreService: CoreService
  ) {}

  dropListReceiverElement?: HTMLElement;
  dragDropInfo?: {
    dragIndex: number;
    dropIndex: number;
  };

  drop(event: CdkDragDrop<number>) {
    moveItemInArray(this.Columns, event.previousIndex, event.currentIndex);
  }

  ngOnInit() {
    this.title = this.Info.title;
    this.renderColumns();
  }

  select(obj: any) {
    this.Columns = orderBy(this.Columns, ['selected'], ['desc']);
  }
  renderColumns() {
    let _columns = Object.keys(this.Info.data);
    // check for objects
    for (let prop of _columns) {
      if (prop === 'author' || prop === 'user') {
        _columns.push('fullName');
      }
      
    }
    let index = 1;
    for (let col of _columns) {
      if (
        typeof this.Info.data[col] === 'object' &&
        !Array.isArray(this.Info.data[col]) &&
        this.Info.data[col] !== null
      ) {
      } else {
        let _format = '';
        let _prefix = '';
        if (col === 'created_at' || col === 'updated_at') {
          _format = 'YYYY-MM-DDTHH:MM:SSZ';
        }
        if (this.coreService.isNumber(this.Info.data[col])) {
          _prefix = '';
        }
        this.Columns.push({
          id: index,
          column: col,
          selected: true,
          prefix: '',
          format: _format,
        });
        index++;
      }
    }
  }

  getKey(index: number, item: any): string {
    return item.id;
  }

  submit() {
    let selectedColumns: any[] = [];
    for (let col of this.Columns) {
      if (col.selected) {
        selectedColumns.push(col);
      }
    }

    this.activeModal.close({
      data: selectedColumns,
    });
  }

  close() {
    this.activeModal.dismiss('Cancel Clicked');
  }
}
*/