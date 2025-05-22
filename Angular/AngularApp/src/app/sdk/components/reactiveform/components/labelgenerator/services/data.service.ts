/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { AppConfig } from '../../../../../../configs/app.configs'

@Injectable()
export class DataService {

  private http = inject(HttpClient);
  private config = inject(AppConfig)
  APIURL: string = '';

  constructor() {
    
  }

  
  ValidateLabel(term: string, type: number) {
    let URL = this.APIURL + 'api/categories/labelvalidator'
    return this.http.post(
      URL,
      JSON.stringify({ term, type }),
    )
  }

}
