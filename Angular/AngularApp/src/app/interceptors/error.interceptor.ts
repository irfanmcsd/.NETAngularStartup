import { inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { renderNotify } from '../store_v2/core/notify/notify.reducers';
import { throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);

  return next(req).pipe(
    catchError((err) => {
      const status = err.status;
      const errorResponse = err.error;
      
      // Handle different error statuses
      switch (true) {
        case status === 0 && err.statusText === 'Unknown Error':
          store.dispatch(
            renderNotify({
              title: 'Server Offline',
              text: 'The API is not responding. The server may be offline.',
              css: 'bg-danger',
            })
          );
          break;
          
        case status === 401:
          store.dispatch(
            renderNotify({
              title: 'Unauthorized',
              text: 'Please login to access this resource',
              css: 'bg-danger',
            })
          );
          break;
          
        case status === 400:
          store.dispatch(
            renderNotify({
              title: 'Bad Request',
              text: getErrorMessage(errorResponse) || 'Invalid request data',
              css: 'bg-danger',
            })
          );
          break;
          
        case status === 403:
          store.dispatch(
            renderNotify({
              title: 'Forbidden',
              text: 'You don\'t have permission to access this resource',
              css: 'bg-danger',
            })
          );
          break;
          
        case status >= 500:
          store.dispatch(
            renderNotify({
              title: 'Server Error',
              text: 'An error occurred on the server. Please try again later.',
              css: 'bg-danger',
            })
          );
          break;
          
        default:
          store.dispatch(
            renderNotify({
              title: 'Error Occurred',
              text: 'An unexpected error occurred',
              css: 'bg-danger',
            })
          );
      }

      // Propagate the error for further handling
      const errorMessage = getErrorMessage(errorResponse) || err.statusText;
      return throwError(() => new Error(errorMessage));
    })
  );
};

// Helper function to extract error message from different error response formats
function getErrorMessage(errorResponse: any): string {
  if (!errorResponse) return '';
  
  if (typeof errorResponse === 'string') return errorResponse;
  if (errorResponse.message) return errorResponse.message;
  if (errorResponse.error) return errorResponse.error;
  
  return JSON.stringify(errorResponse);
}

/*import { Injectable, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { renderNotify } from '../store_v2/core/notify/notify.reducers';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private store = inject(Store);

  constructor() {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    return next.handle(req).pipe(
      catchError((err) => {
        if (err.status !== undefined) {
          if (err.status === 0) {
            if (err.statusText === 'Unknown Error') {
              this.store.dispatch(
                renderNotify({
                  title: 'Server is Offline',
                  text: 'The API call is not responding, may be server is offline',
                  css: 'bg-danger',
                })
              );
            }
          } else if (err.status === 401 || err.status === 400) {
            this.store.dispatch(
              renderNotify({
                title: 'Bad Request',
                text: 'Invalid Data [' + err.error + ']',
                css: 'bg-danger',
              })
            );
          } else if (err.status === 500) {
            this.store.dispatch(
              renderNotify({
                title: 'Response Failed',
                text: 'Error occured while processing your request',
                css: 'bg-danger',
              })
            );
          } else {
            this.store.dispatch(
              renderNotify({
                title: 'Error Occured',
                text: 'Error occured while processing your request',
                css: 'bg-danger',
              })
            );
          }
        }

        const error = err.error.message || err.statusText;
        return throwError(() => error);
      })
    );
  }
}
*/