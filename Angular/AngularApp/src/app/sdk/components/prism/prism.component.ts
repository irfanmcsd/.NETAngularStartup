/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { AfterViewInit, Component, ElementRef, Input, OnChanges, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';
// import * as Prism from 'prismjs';
import 'prismjs';
import 'prismjs/plugins/toolbar/prism-toolbar';
//import 'prismjs/plugins/copy-to-clipboard/prism-copy-to-clipboard';
import 'prismjs/components/prism-yaml';
import 'prismjs/components/prism-css';
import 'prismjs/components/prism-javascript';
import 'prismjs/components/prism-csharp';
import 'prismjs/components/prism-markup';
import 'prismjs/components/prism-typescript';
import 'prismjs/components/prism-sass';
import 'prismjs/components/prism-scss';

declare var Prism: any;

@Component({
    selector: "app-prism",
    templateUrl: "./prism.html",
    standalone: true,
    imports: [NgIf]
})
export class PrismComponent implements AfterViewInit, OnChanges {

    @ViewChild('codeEle') codeEle!: ElementRef;
    @Input() data?: any;
    //@Input() language?: string;
    constructor() { }
    ngAfterViewInit() {
      Prism.highlightElement(this.codeEle.nativeElement);
    }
    ngOnChanges(changes: any): void {
      if (changes?.data) {
        if (this.codeEle?.nativeElement) {
          this.codeEle.nativeElement.textContent = this.data!.code;
          Prism.highlightElement(this.codeEle.nativeElement);
        }
      }
    }
  
}
*/