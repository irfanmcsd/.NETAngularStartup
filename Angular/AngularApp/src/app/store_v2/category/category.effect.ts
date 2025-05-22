import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import { CategoryService } from './category.services';
import {
  loadCategories,
  loadCategoriesSuccess,
  loadCategoriesFailure,
  addCategories,
  addCategoriesSuccess,
  addCategoriesFailure,
  actionCategories,
  categoryActionSuccess,
  categoryActionFailure,
  deleteCategorySuccess,
} from './category.reducer';
import { Router } from '@angular/router';
import { CategoryModel, ICATEGORY } from './model';
import { renderNotify } from '../core/notify/notify.reducers';

@Injectable()
export class CategoryEffects {
  private actions$ = inject(Actions);
  private CategoryService = inject(CategoryService);
  private router = inject(Router);

  loadCategoryModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadCategories),
      switchMap(({ Query }: { Query: ICATEGORY }) => {
        return this.CategoryService.LoadRecords(Query).pipe(
          map((response) => {
            if (response.status == 'success') {
              const categories = response.posts || [];
              // format categories in parent child herarchi
              const navList = this.CategoryService.filterCategories(
                categories,
                0
              );
              const records = response.records || 0;
              return loadCategoriesSuccess({ categories, navList, records });
            } else {
              return loadCategoriesFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(loadCategoriesFailure({ error: error.message }))
          )
        );
      })
    )
  );

  actionCategoryModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actionCategories),
      switchMap(
        ({
          Entities,
          actionstatus,
        }: {
          Entities: CategoryModel[];
          actionstatus: string;
        }) => {
          return this.CategoryService.ProcessActions(
            Entities,
            actionstatus
          ).pipe(
            map((response) => {
              if (response.status == 'success') {
                if (actionstatus === 'delete') {
                  console.log('delete action triggered ', Entities[0].id)
                  return deleteCategorySuccess({
                    id: Entities[0].id,
                  });
                } else {
                  return categoryActionSuccess({
                    entities: Entities,
                    actionstatus,
                  });
                }
              } else {
                return categoryActionFailure({ error: response.message || '' });
              }
            }),
            catchError((error) =>
              of(categoryActionFailure({ error: error.message }))
            )
          );
        }
      )
    )
  );

  // Error handling effect (could show notifications)
  CategoryModelErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(
          loadCategoriesFailure,
          addCategoriesFailure,
          categoryActionFailure
        ),
        tap(({ error }) => {
          this.store.dispatch(
            renderNotify({ title: error, text: '', css: 'bg-danger' })
          );
        })
      ),
    { dispatch: false }
  );
  store: any;
}
