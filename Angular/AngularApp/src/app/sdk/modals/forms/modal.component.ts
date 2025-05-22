/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Component, OnInit, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { NgIf } from "@angular/common";

@Component({
    selector: "viewmodal",
    templateUrl: "./modal.html",
    standalone: true,
    imports: [NgIf]
})
export class FormViewComponent implements OnInit {
  @Input() Info: any;
  title: string = "";
  data: any = [];

  showLoader = false;
  heading: string = "";
  controls: any[] = [];


  constructor(
    public activeModal: NgbActiveModal,
  ) {}

  ngOnInit() {
    this.title = this.Info.title;
   
   
  }

  SubmitForm(payload: any) {
    this.activeModal.close({
      data: payload
    });
  }

  close() {
    this.activeModal.dismiss("Cancel Clicked");
  }
}
