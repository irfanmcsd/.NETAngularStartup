/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Component, OnInit, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
    templateUrl: "./dialog.html",
    standalone: true
})
export class ConfirmDialogViewComponent implements OnInit {
  @Input() Info: any;
  title: string = "";

  constructor(
    public activeModal: NgbActiveModal
  ) {}

  ngOnInit() {
    this.title = this.Info.title;
   
   
  }

  confirm(event: any) {
    this.activeModal.close({
      confirm: true
    });
    event.stopPropagation();
  }

  close() {
    this.activeModal.dismiss("Cancel Clicked");
  }
}
