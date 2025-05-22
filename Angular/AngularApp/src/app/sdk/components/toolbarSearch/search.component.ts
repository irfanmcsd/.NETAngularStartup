/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Component, OnInit, EventEmitter, Input, Output } from "@angular/core";
import { FormsModule } from "@angular/forms";

@Component({
    selector: "app-nav-search",
    templateUrl: "./search.html",
    standalone: true,
    imports: [FormsModule]
})
export class ToolbarSearchComponent implements OnInit {

  @Input() Term: any = [];
  @Input() PlaceHolder: string = "Search something ..."
  @Output() Action = new EventEmitter<any>();
 
  ngOnInit() {}
  
  search(event: any) {
     this.Action.emit({ term: this.Term })
  }
  
  getActionsKey(index:number, item: any): string {
    return item.id;
  }
  
}
*/