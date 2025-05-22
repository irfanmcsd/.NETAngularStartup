/* -------------------------------------------------------------------------- */
/*                         Enterprise App Builder Toolkit                     */
/*                            Author: Mediasoftpro                            */
/*                       Email: support@mediasoftpro.com                      */
/*       License: Read license.txt located on root of your application.       */
/*                     Copyright 2007 - 2023 @Mediasoftpro                    */
/* -------------------------------------------------------------------------- */
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DataService {

  private http = inject(HttpClient);
  
  constructor() { }

  getData(QUERY: any, URL: string) {
    return this.http.post(URL, QUERY);
  }

}
