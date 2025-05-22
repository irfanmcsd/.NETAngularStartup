// src/app/todo.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { delay, map, Observable, of } from 'rxjs';
import { AppConfig } from '../../configs/app.configs';
import { TagModel, ITAG, API_EndPoints, TagResponse } from './model';

@Injectable({ providedIn: 'root' })
export class TagService {
  private http = inject(HttpClient);
  private config = inject(AppConfig);

  constructor() {}

  LoadRecords(Query: ITAG): Observable<TagResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.load}`;
    return this.http.post<TagResponse>(URL, Query);
  }

  GetInfo(Query: ITAG): Observable<TagResponse> {
    Query.skip_record_stats = true;
    const URL = `${this.config.apiUrl}${API_EndPoints.info}`;
    return this.http.post<TagResponse>(URL, Query);
  }

  ProcessRecord(Entity: TagModel): Observable<TagResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.proc}`;
    return this.http.post<TagResponse>(URL, Entity);
  }

  ProcessActions(
    Entities: TagModel[],
    actionstatus: string
  ): Observable<TagResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.action}`;
    // Set the action status for all entities in the array
    const processedEntities = Entities.map((item) => ({
      ...item,
      actionstatus: actionstatus,
    }));
    return this.http.post<TagResponse>(URL, processedEntities);
  }
}
