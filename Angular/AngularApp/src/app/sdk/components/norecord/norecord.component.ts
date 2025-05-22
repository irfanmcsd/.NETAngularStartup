/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Component, Input } from "@angular/core";

@Component({
    selector: "app-norecord",
    template: `
 
  <div class="p-5">
    <div class="d-flex justify-content-center">
      <h3>{{message}}</h3>
    </div>
  </div>
  
  `,
    standalone: true
})
export class NoRecordFoundComponent {
  @Input() message = "";
  constructor() {
    if (this.message === "") {
      this.message = "No Record Found!";
    }
  }
}
