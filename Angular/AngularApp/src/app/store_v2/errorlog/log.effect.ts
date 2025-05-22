
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import { ErrorLogService } from './log.services';
import {
  loadErrorLogs,
  loadErrorLogsSuccess,
  loadErrorLogsFailure,
  addErrorLogs,
  addErrorLogsSuccess,
  addErrorLogsFailure,
  actionErrorLogs,
  ErrorLogActionSuccess,
  ErrorLogActionFailure,
} from './log.reducer';
import { Router } from '@angular/router';
import { ErrorLogModel, ILOG } from './model';
import { renderNotify } from '../core/notify/notify.reducers';

@Injectable()
export class ErrorLogEffects {
  private actions$ = inject(Actions);
  private ErrorLogService = inject(ErrorLogService);
  private router = inject(Router);

  loadErrorLogModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadErrorLogs),
      switchMap(({ Query }: { Query: ILOG }) => {
        return this.ErrorLogService.LoadRecords(Query).pipe(
          map((response) => {
            if (response.status == 'success') {
              const ErrorLogs = response.posts || [];
              const records = response.records || 0;
              return loadErrorLogsSuccess({ ErrorLogs, records });
            } else {
              return loadErrorLogsFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(loadErrorLogsFailure({ error: error.message }))
          )
        );
      })
    )
  );

  addErrorLogModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(addErrorLogs),
      switchMap(({ Entity }: { Entity: ErrorLogModel }) => {
        return this.ErrorLogService.ProcessRecord(Entity).pipe(
          map((response) => {
            if (response.status == 'success') {
              const ErrorLog = response.record;
              if (!ErrorLog) {
                return addErrorLogsFailure({
                  error: 'ErrorLogModel record is undefined',
                });
              }

              return addErrorLogsSuccess({ ErrorLog });
            } else {
              return addErrorLogsFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(addErrorLogsFailure({ error: error.message }))
          )
        );
      })
    )
  );

  actionErrorLogModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actionErrorLogs),
      switchMap(
        ({
          Entities,
          actionstatus,
        }: {
          Entities: ErrorLogModel[];
          actionstatus: string;
        }) => {
          return this.ErrorLogService.ProcessActions(
            Entities,
            actionstatus
          ).pipe(
            map((response) => {
              if (response.status == 'success') {
                return ErrorLogActionSuccess({
                  entities: Entities,
                  actionstatus,
                });
              } else {
                return ErrorLogActionFailure({ error: response.message || '' });
              }
            }),
            catchError((error) =>
              of(ErrorLogActionFailure({ error: error.message }))
            )
          );
        }
      )
    )
  );

  // Error handling effect (could show notifications)
  ErrorLogModelErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(
          loadErrorLogsFailure,
          addErrorLogsFailure,
          ErrorLogActionFailure
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
