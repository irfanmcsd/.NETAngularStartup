
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import { TagService } from './tags.services';

import {
  loadTags,
  loadTagsSuccess,
  loadTagsFailure,
  addTags,
  addTagsSuccess,
  addTagsFailure,
  actionTags,
  TagActionSuccess,
  TagActionFailure,
} from './tags.reducer';
import { Router } from '@angular/router';
import { TagModel, ITAG } from './model';
import { renderNotify } from '../core/notify/notify.reducers';

@Injectable()
export class TagEffects {
  private actions$ = inject(Actions);
  private TagService = inject(TagService);
  private router = inject(Router);

  loadTagModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadTags),
      switchMap(({ Query }: { Query: ITAG }) => {
        return this.TagService.LoadRecords(Query).pipe(
          map((response) => {
            if (response.status == 'success') {
              const Tags = response.posts || [];
              const records = response.records || 0;
              return loadTagsSuccess({ Tags, records });
            } else {
              return loadTagsFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(loadTagsFailure({ error: error.message }))
          )
        );
      })
    )
  );

  addTagModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(addTags),
      switchMap(({ Entity }: { Entity: TagModel }) => {
        return this.TagService.ProcessRecord(Entity).pipe(
          map((response) => {
            if (response.status == 'success') {
              const Tag = response.record;
              if (!Tag) {
                return addTagsFailure({
                  error: 'TagModel record is undefined',
                });
              }

              return addTagsSuccess({ Tag });
            } else {
              return addTagsFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(addTagsFailure({ error: error.message }))
          )
        );
      })
    )
  );

  actionTagModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actionTags),
      switchMap(
        ({
          Entities,
          actionstatus,
        }: {
          Entities: TagModel[];
          actionstatus: string;
        }) => {
          return this.TagService.ProcessActions(
            Entities,
            actionstatus
          ).pipe(
            map((response) => {
              if (response.status == 'success') {
                return TagActionSuccess({
                  entities: Entities,
                  actionstatus,
                });
              } else {
                return TagActionFailure({ error: response.message || '' });
              }
            }),
            catchError((error) =>
              of(TagActionFailure({ error: error.message }))
            )
          );
        }
      )
    )
  );

  // Error handling effect (could show notifications)
  TagModelErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(
          loadTagsFailure,
          addTagsFailure,
          TagActionFailure
        ),
        tap(({ error }) => {
                  this.store.dispatch(renderNotify({ title: error, text: '', css: 'bg-danger' }));
                })
      ),
    { dispatch: false }
  );
  store: any;
}
