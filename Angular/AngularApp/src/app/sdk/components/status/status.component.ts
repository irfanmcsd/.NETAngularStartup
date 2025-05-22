/*import { Component, Input, OnInit } from '@angular/core'
import { NgIf } from '@angular/common';

@Component({
    selector: 'app-action',
    templateUrl: './status.html',
    standalone: true,
    imports: [NgIf],
})
export class StatusComponent implements OnInit {
  @Input() List: any = []
  @Input() Action: number = 0
  @Input() Active: number = -1 // not set, > -1 [set css green if match]
  @Input() InActive: number = -1 // not set, > -1 [set css red if match]
  CSS: string = 'badge rounded-pill bg-light-dark text-dark p-2'
  Label: string = ''

  constructor() {}

  ngOnInit(): void {
    for (let item of this.List) {
      if (item.value === this.Action) {
        this.Label = item.title
        if (this.Action === this.Active) {
          this.CSS = 'badge rounded-pill bg-light-success text-success p-2'
        } else if (this.Action === this.InActive) {
          this.CSS = 'badge rounded-pill bg-light-danger text-danger p-2'
        }
      }
    }
  }
}
*/