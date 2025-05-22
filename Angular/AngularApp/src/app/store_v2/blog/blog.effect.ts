import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { CoreService } from '../../sdk/services/coreService';

import { BlogsService } from './blog.services';
import {
  loadBlogs,
  loadBlogsSuccess,
  loadBlogsFailure,
  addBlogs,
  addBlogsSuccess,
  addBlogsFailure,
  actionBlogs,
  addBlogsActionSuccess,
  addBlogsActionFailure,
} from './blog.reducer';
import { BlogModel, IBLOGS } from './model';
import { renderNotify } from '../core/notify/notify.reducers';

/**
 * NgRx Effects for Blog operations
 * Handles side effects for blog-related actions including:
 * - Loading blogs
 * - Adding new blogs
 * - Performing actions on blogs
 * - Navigation and error handling
 */
@Injectable()
export class BlogEffects {
  private actions$ = inject(Actions);
  private blogService = inject(BlogsService);
  private router = inject(Router);
  private store = inject(Store);
  private coreService = inject(CoreService)
  /**
   * Effect to handle loading blogs
   * Dispatches success action with blogs data or failure action on error
   */
  loadBlogs$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadBlogs),
      switchMap(({ Query }) =>
        this.blogService.LoadRecords(Query).pipe(
          map((response) => {
            if (response.status === 'success') {
              const _raw_blogs = response.posts || [];
              const blogs = _raw_blogs.map((item: any) => {
                return Object.assign({}, item, {
                  enc_id: this.coreService.encrypt(item.id)
                })
              })
              const records = response.records || 0;
              return loadBlogsSuccess({ blogs, records });
            }
            return loadBlogsFailure({ error: response.message || 'Failed to load blogs' });
          }),
          catchError((error) => of(loadBlogsFailure({ error: error.message })))
        )
      )
    )
  );

  /**
   * Effect to handle adding new blogs
   * Dispatches success action with new blog data or failure action on error
   */
  addBlog$ = createEffect(() =>
    this.actions$.pipe(
      ofType(addBlogs),
      switchMap(({ Entity }) =>
        this.blogService.ProcessRecord(Entity).pipe(
          map((response) => {
            if (response.status === 'success') {
              const blogs = response.record;
              if (!blogs) {
                return addBlogsFailure({ error: 'Blog record is undefined' });
              }
              return addBlogsSuccess({ blogs });
            }
            return addBlogsFailure({ error: response.message || 'Failed to add blog' });
          }),
          catchError((error) => of(addBlogsFailure({ error: error.message })))
        )
      )
    )
  );

  /**
   * Effect to handle batch actions on blogs
   * Dispatches success action with updated entities or failure action on error
   */
  actionBlogs$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actionBlogs),
      switchMap(({ Entities, actionstatus }) =>
        this.blogService.ProcessActions(Entities, actionstatus).pipe(
          map((response) => {
            if (response.status === 'success') {
              return addBlogsActionSuccess({ entities: Entities, actionstatus });
            }
            return addBlogsActionFailure({ 
              error: response.message || 'Failed to perform action' 
            });
          }),
          catchError((error) => 
            of(addBlogsActionFailure({ error: error.message }))
          )
        )
      )
    )
  );

  /**
   * Effect to handle navigation after successful blog addition
   * Does not dispatch additional actions
   */
  addBlogSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(addBlogsSuccess),
        tap(() => this.router.navigate(['/blogs']))
      ),
    { dispatch: false }
  );

  /**
   * Effect to handle error notifications for all blog operations
   * Displays error notifications using the notification system
   */
  blogErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(loadBlogsFailure, addBlogsFailure, addBlogsActionFailure),
        tap(({ error }) => {
          this.store.dispatch(
            renderNotify({ 
              title: error, 
              text: '', 
              css: 'bg-danger' 
            })
          );
        })
      ),
    { dispatch: false }
  );
}