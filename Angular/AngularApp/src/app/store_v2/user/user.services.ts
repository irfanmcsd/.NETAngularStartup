// src/app/todo.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { delay, map, Observable, of } from 'rxjs';
import { AppConfig } from '../../configs/app.configs';
import { UserModel, IUSER, API_EndPoints, UserReponse } from './model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private config = inject(AppConfig);
 
  constructor() {}

  LoadRecords(Query: IUSER): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.load}`;
    return this.http.post<UserReponse>(URL, Query);
  }

  GetInfo(Query: IUSER): Observable<UserReponse> {
    Query.skip_record_stats = true;
    const URL = `${this.config.apiUrl}${API_EndPoints.info}`;
    return this.http.post<UserReponse>(URL, Query);
  }

  ProcessRecord(Entity: UserModel): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.proc}`;
    return this.http.post<UserReponse>(URL, Entity);
  }

  ChangePassword(Entity: UserModel): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.change_pass}`;
    return this.http.post<UserReponse>(URL, Entity);
  }

  ChangeEmail(Entity: UserModel): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.change_email}`;
    return this.http.post<UserReponse>(URL, Entity);
  }

  ChangeUserRole(Entity: UserModel): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.change_role}`;
    return this.http.post<UserReponse>(URL, Entity);
  }

  UpdateAvatar(Entity: UserModel): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.update_avatar}`;
    return this.http.post<UserReponse>(URL, Entity);
  }

  ProcessActions(
    Entities: UserModel[],
    actionstatus: string
  ): Observable<UserReponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.action}`;
    // Set the action status for all entities in the array
    const processedEntities = Entities.map((item) => ({
      ...item,
      actionstatus: actionstatus,
    }));
    return this.http.post<UserReponse>(URL, processedEntities);
  }
}
