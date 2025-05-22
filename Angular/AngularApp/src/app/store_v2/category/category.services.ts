// src/app/todo.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { delay, map, Observable, of } from 'rxjs';
import { AppConfig } from '../../configs/app.configs';
import { CategoryModel, ICATEGORY, API_EndPoints, CategoryReponse } from './model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private config = inject(AppConfig);


  constructor() {
    
  }

  LoadRecords(Query: ICATEGORY): Observable<CategoryReponse> {
    const URL = this.config.apiUrl + API_EndPoints.load;
    return this.http.post<CategoryReponse>(URL, Query);
  }

  

  GetInfo(Query: ICATEGORY): Observable<CategoryReponse> {
    Query.skip_record_stats = true;
    const URL = this.config.apiUrl + API_EndPoints.info;
    return this.http.post<CategoryReponse>(URL, Query);
  }

  ProcessRecord(Entity: CategoryModel): Observable<CategoryReponse> {
    const URL = this.config.apiUrl + API_EndPoints.proc;
    return this.http.post<CategoryReponse>(URL, Entity);
  }

  ProcessActions(
    Entities: CategoryModel[],
    actionstatus: string,
  ): Observable<CategoryReponse> {

    const URL = `${this.config.apiUrl}${API_EndPoints.action}`;
     // Set the action status for all entities in the array
    const processedEntities = Entities.map(item => ({
      ...item,
      actionstatus: actionstatus
    }));
    return this.http.post<CategoryReponse>(URL, processedEntities);
  }

  filterCategories(categories: CategoryModel[], parentId: number | null): CategoryModel[] {
    // Validate input
    if (!Array.isArray(categories)) {
      throw new TypeError('categories must be an array');
    }
  
    // Use filter and map for better readability
    return categories
      .filter(cat => cat.parentid === parentId)
      .map(cat => ({
        ...cat,
        picturename: '', // Ensure picturename is empty string
        children: this.filterCategories(categories, cat.id)
      }));
  }

  /* Filter and prepare parent child category heirarchy by adding hypen */
  prepareParentChildCategories(
    categories: CategoryModel[], 
    parentId: number | null = null, 
    level: number = 0,
    parentTitle: string = ''
  ): CategoryModel[] {
    // Validate input
    if (!Array.isArray(categories)) {
      throw new TypeError('categories must be an array');
    }
  
    // Get all categories with matching parentId
    const matchedCategories = categories.filter(cat => cat.parentid === parentId);
  
    let result: CategoryModel[] = [];
  
    matchedCategories.forEach(cat => {
      // Create the new title with hierarchy
      const indent = '—'.repeat(level);
      const newTitle = level > 0 
        ? `${parentTitle} ${indent}${cat.title}`
        : cat.title;
  
      // Add the modified category to results
      result.push({
        ...cat,
        title: newTitle,
        //picturename: '' // Ensure picturename is empty string
      });
  
      // Recursively process children and add to results
      const childCategories = this.prepareParentChildCategories(
        categories,
        cat.id,
        level + 1,
        newTitle
      );
      result = result.concat(childCategories);
    });
  
    return result;
  }
 

}
