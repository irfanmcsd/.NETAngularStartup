import { Injectable, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  HttpRequest,
  HttpHandler,
  HttpInterceptor,
  HttpEvent,
} from '@angular/common/http';
import { first, mergeMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { IAUTH } from '../store_v2/auth/model';
import { selectAllAuth } from '../store_v2/auth/auth.reducer';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private store = inject(Store);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.store.select(selectAllAuth).pipe(
      first(), // Take the first emitted value and complete
      mergeMap((auth: IAUTH[]) => {
        //console.log('Auth Interceptor:', auth);
        if (auth.length > 0 && auth[0].isAuthenticated && auth[0].Token) {
          const authReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${auth[0].Token}`,
            },
          });
          return next.handle(authReq);
        }
        return next.handle(req);
      })
    );
  }
}

/*import { Injectable, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  HttpRequest,
  HttpHandler,
  HttpResponse,
  HttpInterceptor,
} from '@angular/common/http';

import { IAUTH } from '../store_v2/auth/model';
import { selectAllAuth } from '../store_v2/auth/auth.reducer';
import { Observable, tap } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private store = inject(Store);
  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);

  Token: string = '';
  isAuthenticated = false;

  constructor() {
    this.auth$ = this.store.select(selectAllAuth).pipe(
      tap((auth: any) => {
        //console.log('Selected auth:', auth);
        if (auth.length > 0) {
          this.isAuthenticated = auth[0].isAuthenticated;
          this.Token = auth[0].Token;
          console.log('Token:', this.Token);
        }
      })
    );
    this.auth$.subscribe();
  }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    if (this.isAuthenticated) {
      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${this.Token}`,
        },
      });

      return next.handle(authReq);
    }

    return next.handle(req);
  }
}*/
