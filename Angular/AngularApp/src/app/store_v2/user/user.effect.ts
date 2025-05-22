
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import { UserService } from './user.services';

import {
  loadUsers,
  loadUsersSuccess,
  loadUsersFailure,
  addUsers,
  addUsersSuccess,
  addUsersFailure,
  actionUsers,
  UserActionSuccess,
  UserActionFailure,
} from './user.reducer';
import { Router } from '@angular/router';
import { UserModel, IUSER } from './model';
import { renderNotify } from '../core/notify/notify.reducers';

@Injectable()
export class UserEffects {
  private actions$ = inject(Actions);
  private UserService = inject(UserService);
  private router = inject(Router);

  loadUserModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadUsers),
      switchMap(({ Query }: { Query: IUSER }) => {
        return this.UserService.LoadRecords(Query).pipe(
          map((response) => {
            if (response.status == 'success') {
              const Users = response.posts || [];
              const records = response.records || 0;
              return loadUsersSuccess({ Users, records });
            } else {
              return loadUsersFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(loadUsersFailure({ error: error.message }))
          )
        );
      })
    )
  );

  addUserModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(addUsers),
      switchMap(({ Entity }: { Entity: UserModel }) => {
        return this.UserService.ProcessRecord(Entity).pipe(
          map((response) => {
            if (response.status == 'success') {
              const User = response.record;
              if (!User) {
                return addUsersFailure({
                  error: 'UserModel record is undefined',
                });
              }

              return addUsersSuccess({ User });
            } else {
              return addUsersFailure({ error: response.message || '' });
            }
          }),
          catchError((error) =>
            of(addUsersFailure({ error: error.message }))
          )
        );
      })
    )
  );

  actionUserModel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actionUsers),
      switchMap(
        ({
          Entities,
          actionstatus,
        }: {
          Entities: UserModel[];
          actionstatus: string;
        }) => {
          return this.UserService.ProcessActions(
            Entities,
            actionstatus
          ).pipe(
            map((response) => {
              if (response.status == 'success') {
                return UserActionSuccess({
                  entities: Entities,
                  actionstatus,
                });
              } else {
                return UserActionFailure({ error: response.message || '' });
              }
            }),
            catchError((error) =>
              of(UserActionFailure({ error: error.message }))
            )
          );
        }
      )
    )
  );

  // Success effect that could navigate after adding
  addUserModelSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(addUsersSuccess),
        tap(() => this.router.navigate(['/users']))
      ),
    { dispatch: false }
  );

  // Error handling effect (could show notifications)
  UserModelErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(
          loadUsersFailure,
          addUsersFailure,
          UserActionFailure
        ),
        tap(({ error }) => {
                  this.store.dispatch(renderNotify({ title: error, text: '', css: 'bg-danger' }));
                })
      ),
    { dispatch: false }
  );
  store: any;
}
