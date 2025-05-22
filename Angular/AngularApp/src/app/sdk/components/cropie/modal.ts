/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Component, OnInit, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { CroppieComponent } from "../../directives/croppie/croppie";
import { NgIf } from "@angular/common";

@Component({
    templateUrl: "./modal.html",
    standalone: true,
    styleUrls: ['./modal.css'],
    imports: [NgIf, CroppieComponent]
})
export class CropperViewComponent implements OnInit {
  @Input() Info: any;
  title: string = "";
  data: any = [];
  CropOption = 0; // 0: user logo, 1: logo (e.g agency), 2: banner
  
  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit() {
    this.title = this.Info.title;
  }

  croppedImage(data: any) {
    this.activeModal.close({
      data: data,
      type: 100
    });
  }

  close(event: any) {
    this.activeModal.dismiss("Cancel Clicked");
  }
}
