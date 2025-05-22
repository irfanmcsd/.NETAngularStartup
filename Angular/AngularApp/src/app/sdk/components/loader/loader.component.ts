/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
import { Component, OnInit, Input } from '@angular/core';
@Component({
  selector: 'app-loader',
  styles: [
    `
      .loader {
        width: 48px;
        height: 48px;
        border: 5px solid;
        border-color: #e33048 transparent;
        border-radius: 50%;
        display: inline-block;
        box-sizing: border-box;
        animation: rotation 1s linear infinite;
      }

      @keyframes rotation {
        0% {
          transform: rotate(0deg);
        }
        100% {
          transform: rotate(360deg);
        }
      }
    `,
  ],
  template: `
    <div style="margin: 100px auto;" class="d-flex justify-content-center">
      <span class="loader"></span>
    </div>
  `,
  standalone: true,
})
export class LoaderComponent implements OnInit {
  @Input() type = 0; // 0: normal loader, 1: global loader

  ngOnInit() {
    this.type = this.type === undefined ? 0 : this.type;
  }
}
