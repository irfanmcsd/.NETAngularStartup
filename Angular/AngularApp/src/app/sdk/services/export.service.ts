/*import { Inject, Injectable, ElementRef } from '@angular/core';
import * as FileSaver from 'file-saver';

import * as XLSX from 'xlsx';
import { ExcelJson } from '../interfaces/excel-json.interface';
const EXCEL_EXTENSION = '.xlsx';
const CSV_EXTENSION = '.csv';
const CSV_TYPE = 'text/plain;charset=utf-8'; // 'text/plain;charset=utf-8';

@Injectable()
export class ExportService {

  constructor() { 
  }

  public exportTableElmToExcel(element: ElementRef, fileName: string): void {
    const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(element.nativeElement);
    // generate workbook and add the worksheet
    const workbook: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, ws, 'Sheet1');
    // save to file
    XLSX.writeFile(workbook, `${fileName}${EXCEL_EXTENSION}`);

  }

  public exportJsonToExcel(json: ExcelJson[], fileName: string): void {
    // inserting first blank row
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(
      json[0].data,
      this.getOptions(json[0])
    );

    for (let i = 1, length = json.length; i < length; i++) {
      // adding a dummy row for separation
      XLSX.utils.sheet_add_json(
        worksheet,
        [{}],
        this.getOptions(
          {
            data: [],
            skipHeader: true
          }, -1)
      );
      XLSX.utils.sheet_add_json(
        worksheet,
        json[i].data,
        this.getOptions(json[i], -1)
      );
    }
    const workbook: XLSX.WorkBook = { Sheets: { Sheet1: worksheet }, SheetNames: ['Sheet1'] };
    // save to file
    XLSX.writeFile(workbook, `${fileName}${EXCEL_EXTENSION}`);
  }

  private getOptions(json: any, origin?: number): any {
    // adding actual data
    const options = {
      skipHeader: true,
      origin: -1,
      header: []
    };
    options.skipHeader = json.skipHeader ? json.skipHeader : false;
    if (!options.skipHeader && json.header && json.header.length) {
      options.header = json.header;
    }
    if (origin) {
      options.origin = origin ? origin : -1;
    }
    return options;
  }

  private saveAsFile(buffer: any, fileName: string, fileType: string): void {
    
    const data: Blob = new Blob([buffer], { type: fileType });
    FileSaver.saveAs(data, fileName);
    //this.fileSaverService.save(data, fileName);
    
    //console.log (data)
    try {
      
      // this.fileSaverService.save(data, fileName);
      
    }
    catch (e)
    {
      console.log(e) ;
    }
    
  }


  public exportToCsv(rows: object[], fileName: string, columns?: string[]): any {
    if (!rows || !rows.length) {
      return "";
    }
    const separator = ',';
    const keys = Object.keys(rows[0]).filter(k => {
      if (columns?.length) {
        return columns.includes(k);
      } else {
        return true;
      }
    });
    const csvContent =
      keys.join(separator) +
      '\n' +
      rows.map((row: any) => {
        return keys.map(k => {
          let cell = row[k] === null || row[k] === undefined ? '' : row[k];
          cell = cell instanceof Date
            ? cell.toLocaleString()
            : cell.toString().replace(/"/g, '""');
          if (cell.search(/("|,|\n)/g) >= 0) {
            cell = `"${cell}"`;
          }
          return cell;
        }).join(separator);
      }).join('\n');

    this.saveAsFile(csvContent, `${fileName}${CSV_EXTENSION}`, CSV_TYPE);
  }
}*/