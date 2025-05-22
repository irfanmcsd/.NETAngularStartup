// src/app/todo.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConfig } from '../../configs/app.configs';
import { BlogModel, IBLOGS, API_EndPoints, BlogResponse } from './model';

/**
 * Service for handling blog-related operations including:
 * - Loading blog records
 * - Getting blog information
 * - Processing individual blog records
 * - Performing batch actions on multiple blogs
 */
@Injectable({ providedIn: 'root' })
export class BlogsService {
  private http = inject(HttpClient);
  private config = inject(AppConfig);
  
  constructor() {}

  /**
   * Loads blog records based on the provided query parameters
   * @param Query - The query parameters for filtering/sorting blogs
   * @returns Observable with the blog response
   */
  LoadRecords(Query: IBLOGS): Observable<BlogResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.load}`;
    return this.http.post<BlogResponse>(URL, Query);
  }

  /**
   * Gets detailed information about a specific blog
   * @param Query - The query parameters including blog identifier
   * @returns Observable with the blog response
   */
  GetInfo(Query: IBLOGS): Observable<BlogResponse> {
    Query.skip_record_stats = true; // Skip stats for faster response
    const URL = `${this.config.apiUrl}${API_EndPoints.info}`;
    return this.http.post<BlogResponse>(URL, Query);
  }

  /**
   * Processes a single blog record (create/update)
   * @param Entity - The blog model to be processed
   * @returns Observable with the operation result
   */
  ProcessRecord(Entity: BlogModel): Observable<BlogResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.proc}`;
    return this.http.post<BlogResponse>(URL, Entity);
  }

  /**
   * Performs batch actions on multiple blog records
   * @param Entities - Array of blog models to process
   * @param actionstatus - The action to perform on all entities
   * @returns Observable with the operation result
   */
  ProcessActions(
    Entities: BlogModel[],
    actionstatus: string
  ): Observable<BlogResponse> {
    const URL = `${this.config.apiUrl}${API_EndPoints.action}`;
    
    // Set the action status for all entities in the array
    const processedEntities = Entities.map(item => ({
      ...item,
      actionstatus: actionstatus
    }));
    
    return this.http.post<BlogResponse>(URL, processedEntities);
  }
}