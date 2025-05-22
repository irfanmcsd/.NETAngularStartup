/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Component, OnInit, Input } from "@angular/core";
import { AppConfig } from "../../../configs/app.configs"
import { CodeSnippet } from '../../services/codeSnippet';
import { PrismComponent } from "./prism.component";
import { NgIf, NgFor } from "@angular/common";
@Component({
    selector: "app-previewcode",
    templateUrl: "./preview.html",
    standalone: true,
    imports: [NgIf, NgFor, PrismComponent]
})
export class PreviewCodeComponent implements OnInit {

  @Input() Type: number = 0; // 0: list, 1: add, 2: edit, 3: analytic report, 4: dashboard, 5: profile
  
  constructor(public appConfig: AppConfig, public codeSnippet: CodeSnippet
    ) {}
  
  ngOnInit() {

  }
  
  
}
*/