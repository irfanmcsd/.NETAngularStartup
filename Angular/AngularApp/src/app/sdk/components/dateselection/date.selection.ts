/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Component, OnInit, EventEmitter, Input, Output } from "@angular/core";
import { Store } from "@ngrx/store";
import { IAppState } from "../../../store_v2/model.store";
import {  TriggerEvent } from "../../../store_v2/core/core.actions";
import { NgFor } from "@angular/common";
import { FormsModule } from "@angular/forms";
@Component({
    selector: "app-date-selection",
    templateUrl: "./date.html",
    standalone: true,
    imports: [FormsModule, NgFor]
})
export class NavDateSelectionComponent implements OnInit {

  @Input() Title: string = ''
  @Input() Filters: any = []
  @Output() Action = new EventEmitter<any>();
 
  FilterOption = 10
  constructor(
    private _store: Store<IAppState>
  ) { }

  ngOnInit() {}

  toolbaraction(action: any, value: any, attribute: any, event: any) {
    this.Action.emit({ action, value, attribute })
    event.stopPropagation()
  }

  change (event: any) {
    this._store.dispatch(new TriggerEvent({
      type: 'datefilter',
      data: {
        datefilter: this.FilterOption
      }
    }));
  }


  getKey(index: number, item: any): string {
    return item.key;
  }

}*/
