/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import {
  Component,
  OnInit,
  OnChanges,
  EventEmitter,
  Input,
  Output
} from "@angular/core";
import { PaginationService } from "./pagination.service";
import { PaginationEntity, PaginationLinkEntity } from "./pagination.model";
import { NgFor, NgClass, NgIf } from "@angular/common";

@Component({
    selector: "pagination",
    templateUrl: "./pagination.html",
    providers: [PaginationService],
    standalone: true,
    imports: [NgFor, NgClass, NgIf]
})
export class PaginationComponent implements OnInit, OnChanges {

  @Input() currentPage: number = 1;
  @Input() totalRecords: number = 0;
  @Input() pageSize: number = 20;
  @Input() showFirst: number = 1;
  @Input() showLast: number = 1;
  @Input() paginationstyle: number = 0;
  @Input() totalLinks: number = 7;
  @Input() prevCss = "";
  @Input() nextCss = "";
  @Input() urlpath = "";

  @Output() OnSelection = new EventEmitter<number>();
  
  Links: PaginationLinkEntity[] = [];

  constructor(private paginationService: PaginationService) {  }

  ngOnChanges() {
    this.refreshPagination();
  }

  ngOnInit() {
    this.refreshPagination();
  }

  refreshPagination() {
    const options: PaginationEntity = {
      currentPage: parseInt(this.currentPage.toString(), 10), // due to some unknow reason this input receives as string which causes problem in calculation
      totalRecords: this.totalRecords,
      pageSize: this.pageSize,
      showFirst: this.showFirst,
      showLast: this.showLast,
      paginationstyle: this.paginationstyle,
      totalLinks: this.totalLinks,
      prevCss: this.prevCss,
      nextCss: this.nextCss,
      urlpath: this.urlpath
    };

    this.Links = this.paginationService.ProcessPagination(options);
  }

  page(id: any, event: any) {
    this.OnSelection.emit(id);
    event.stopPropagation();
  }
}
