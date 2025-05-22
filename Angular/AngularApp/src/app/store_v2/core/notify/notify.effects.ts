
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import { NotifyService } from './notify.service';
import { renderNotify, Notify } from './notify.reducers';
import { Router } from '@angular/router';

@Injectable()
export class NotifyEffects {
  private actions$ = inject(Actions);
  private notifyService = inject(NotifyService);

  renderNotify$ = createEffect(() =>
    this.actions$.pipe(
      ofType(renderNotify),
      switchMap((notify: Notify) =>
        this.notifyService.renderNotification(notify).pipe(
          map(() => {
            // console.log('Notification rendered successfully');
            return { type: '[Notify] Notification Success' };
          })
        )
      )
    )
  );
}
