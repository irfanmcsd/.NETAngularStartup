/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Injectable, inject } from "@angular/core";
import { PaginationEntity, PaginationLinkEntity } from "./pagination.model";
// import { each } from "lodash";

@Injectable()
export class PaginationService {
 /* currentPage = 0;
  totalRecords = 0;
  pageSize = 20;
  showFirst = 1;
  showLast = 1;
  paginationstyle = 0;
  totalLinks = 7;
  prevCss = "previous";
  nextCss = "next";
  urlpath = "";
  totalPages = 0;

  PaginationLinks: PaginationLinkEntity[] = [];*/
  totalPages: number = 0;
  Options: PaginationEntity; 
  PaginationLinks: PaginationLinkEntity[] = [];

  constructor() {
     this.Options = this.Initialize()
  }

  Initialize(): PaginationEntity {
     return {
      currentPage: 1,
      totalRecords: 0,
      pageSize:  20,
      showFirst: 1,
      showLast: 1,
      paginationstyle: 0,
      totalLinks: 7,
      prevCss: "previous",
      nextCss: "next",
      urlpath: ""
     }
  }

  ProcessPagination(entity: PaginationEntity): PaginationLinkEntity[] {
    this.PaginationLinks = [];
    this.Options = entity;
   
    // normal script
    let firstbound: number = 0;
    let lastbound: number = 0;
    let tooltip: string = "";
    if (this.Options.totalRecords > this.Options.pageSize) {
      this.totalPages = Math.ceil(this.Options.totalRecords / this.Options.pageSize);

      if (this.Options.currentPage > 1) {
        if (this.Options.showFirst === 1 && this.Options.paginationstyle != 2) {
          firstbound = 1;
          lastbound = firstbound + this.Options.pageSize - 1;
          tooltip =
            "showing " +
            firstbound +
            " - " +
            lastbound +
            " records of " +
            this.Options.totalRecords +
            " records";
          // First Link
          this.addLink("1", "#/" + this.Options.urlpath, "First", tooltip, "first");
        }

        firstbound = (this.totalPages - 1) * this.Options.pageSize;
        lastbound = firstbound + this.Options.pageSize - 1;

        if (lastbound > this.Options.totalRecords) {
          lastbound = this.Options.totalRecords;
        }

        tooltip =
          "showing " +
          firstbound +
          " - " +
          lastbound +
          " records of " +
          this.Options.totalRecords +
          " records";
        // Previous Link Enabled
        var pid = this.Options.currentPage - 1;
        if (pid < 1) pid = 1;

        let prevPageCss: string = "";
        let prevIcon: string = "Prev";

        if (this.Options.paginationstyle === 2) {
          if (this.Options.prevCss != "") prevPageCss = this.Options.prevCss;

          prevIcon = "&larr; Previous";
        }

        let _urlpath: string = "";
        if (this.Options.urlpath !== "") _urlpath = this.Options.urlpath + "/" + pid;

        this.addLink(pid.toString(), "#/" + _urlpath, prevIcon, tooltip, "previous");

        // Normal Links
        if (this.Options.paginationstyle !== 2) this.gen_links(this.Options.urlpath);

        if (this.Options.currentPage < this.totalPages)
          this.set_prev_last_links(this.Options.urlpath);
      } else {
        if (this.Options.paginationstyle !== 2) this.gen_links(this.Options.urlpath);

        if (this.Options.currentPage < this.totalPages)
          this.set_prev_last_links(this.Options.urlpath);
      }
    }
    return this.PaginationLinks;
  }

  gen_links(urlpath: any) {
    let firstbound: number = 0;
    let lastbound: number = 0;
    let tooltip: string = "";

    let Links: number[] = this.SimplePagination();
   
    for (let link of Links) {
      firstbound = (link - 1) * this.Options.pageSize + 1;
      lastbound = firstbound +this.Options.pageSize - 1;
      if (lastbound > this.Options.totalRecords) lastbound = this.Options.totalRecords;

      tooltip =
        "showing " +
        firstbound +
        " - " +
        lastbound +
        " records  of " +
         this.Options.totalRecords +
        " records";

      let _urlpath: string = "";
      if (urlpath != "") _urlpath = urlpath + "/" + link;

      this.addLink(link.toString(), "#/" + _urlpath, link.toString(), tooltip, "");
    }

  }

  set_prev_last_links(urlpath: any) {
    let firstbound: number = this.Options.currentPage * this.Options.pageSize + 1;
    let lastbound: number = firstbound + this.Options.pageSize - 1;

    if (lastbound > this.Options.totalRecords) lastbound = this.Options.totalRecords;

    let tooltip =
      "showing " +
      firstbound +
      " - " +
      lastbound +
      " records of " +
      this.Options.totalRecords +
      " records";

    // Next Link
    var pid = this.Options.currentPage + 1;
    if (pid > this.totalPages) pid = this.totalPages;

    let nextPageCss: string = "";
    let nextPageIcon: string = "Next";

    if (this.Options.paginationstyle === 2) {
      if (this.Options.nextCss != "") nextPageCss = this.Options.nextCss;

      nextPageIcon = "Next &rarr;";
    }

    this.addLink(pid.toString(), "#/" + urlpath, nextPageIcon, tooltip, "next");

    if (this.Options.showLast === 1 && this.Options.paginationstyle !== 2) {
      // Last Link
      firstbound = (this.totalPages - 1) * this.Options.pageSize + 1;
     
      lastbound = firstbound + this.Options.pageSize - 1;
     
      if (lastbound > this.Options.totalRecords) lastbound = this.Options.totalRecords;
     
      tooltip =
        "showing " +
        firstbound +
        " - " +
        lastbound +
        " records of " +
        this.Options.totalRecords +
        " records";

      let _urlpath: string = "";
      if (urlpath != "") _urlpath = urlpath + "/" + this.totalPages;

      let pagenumber = Math.ceil(lastbound / this.Options.pageSize)
    
      this.addLink(pagenumber.toString(), "#/" + _urlpath, "Last", tooltip, "last");
    }
  }

  addLink(id: string, url: string, name: string, tooltip: string, css: string) {
    this.PaginationLinks.push({
      id: id,
      url: url,
      name: name,
      tooltip: tooltip,
      css: css
    });
  }

  SimplePagination(): number[] {
    let arr: number[] = [];
    if (this.totalPages < this.Options.totalLinks) {
      for (var i = 1; i <= this.totalPages; i++) {
        arr[i - 1] = i;
      }
    } else {
     
      let startindex: number = this.Options.currentPage;
      let lowerbound: number = startindex - Math.floor(this.Options.totalLinks / 2);
      let upperbound = startindex + Math.floor(this.Options.totalLinks / 2);
     
      if (lowerbound < 1) {
        //calculate the difference and increment the upper bound
        upperbound = upperbound + (1 - lowerbound);
        lowerbound = 1;
      }
      //if upperbound is greater than total page is
      if (upperbound > this.totalPages) {
        //calculate the difference and decrement the lower bound
        lowerbound = lowerbound - (upperbound - this.totalPages);
        upperbound = this.totalPages;
      }
      var counter = 0;
      for (var i = lowerbound; i <= upperbound; i++) {
        arr[counter] = i;
        counter++;
      }
    }
    return arr;
  }
}
