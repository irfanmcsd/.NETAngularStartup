/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

export interface PaginationEntity {
  currentPage: number;
  totalRecords: number;
  pageSize: number;
  showFirst: number;
  showLast: number;
  paginationstyle: number;
  totalLinks: number;
  prevCss: string;
  nextCss: string;
  urlpath: string;
}

export interface PaginationLinkEntity {
  id: string;
  url: string;
  name: string;
  tooltip: string;
  css: string;
}
