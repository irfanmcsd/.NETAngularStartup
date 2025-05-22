// src/app/todo.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { delay, map, Observable, of } from 'rxjs';
import { AppConfig } from '../../configs/app.configs';
import { ErrorLogModel, ILOG, API_EndPoints, ErrorLogReponse } from './model';

@Injectable({ providedIn: 'root' })
export class ErrorLogService {
  private http = inject(HttpClient);
  private config = inject(AppConfig);

  constructor() {}

  LoadRecords(Query: ILOG): Observable<ErrorLogReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.load}`;
    return this.http.post<ErrorLogReponse>(URL, Query);
  }

  GetInfo(Query: ILOG): Observable<ErrorLogReponse> {
    Query.skip_record_stats = true;
    const URL = `${this.config.apiUrl}${API_EndPoints.info}`;
    return this.http.post<ErrorLogReponse>(URL, Query);
  }

  ProcessRecord(Entity: ErrorLogModel): Observable<ErrorLogReponse> {
    const URL = this.config.apiUrl + API_EndPoints.proc;
    return this.http.post<ErrorLogReponse>(URL, Entity);
  }

  ProcessActions(
    Entities: ErrorLogModel[],
    actionstatus: string
  ): Observable<ErrorLogReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.action}`;
    // Set the action status for all entities in the array
    const processedEntities = Entities.map((item) => ({
      ...item,
      actionstatus: actionstatus,
    }));
    return this.http.post<ErrorLogReponse>(URL, processedEntities);
  }
}
