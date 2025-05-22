import { Injectable, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';

// Services
import { AuthService } from './auth.services';

// Actions
import { authorizeUser, loadAuthSuccess, loadAuthFailure } from './auth.reducer';

// Models
import { IAUTH } from './model';

// Notification Actions
import { renderNotify } from '../core/notify/notify.reducers';

/**
 * AuthEffects - NgRx Effects for Authentication
 * 
 * Handles side effects for authentication-related actions including:
 * - User authorization flow
 * - Success/failure handling
 * - Error notifications
 * 
 * Effects:
 * 1. authorizeUser$: Processes authentication requests
 * 2. authErrors$: Handles and displays authentication errors
 */
@Injectable()
export class AuthEffects {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly actions$ = inject(Actions);
  private readonly authService = inject(AuthService);
  private readonly store = inject(Store);

  /* ---------------------------- EFFECT DEFINITIONS ---------------------------- */

  /**
   * authorizeUser$ - Handles user authentication flow
   * 
   * Listens for authorizeUser action and:
   * 1. Calls AuthService to authenticate user
   * 2. On success:
   *    - Transforms response into IAUTH format
   *    - Dispatches loadAuthSuccess with user data
   * 3. On failure:
   *    - Dispatches loadAuthFailure with error message
   */
  authorizeUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(authorizeUser),
      switchMap(({ UserID }) => 
        this.authService.authorize(UserID).pipe(
          map(response => this.handleAuthResponse(response)),
          catchError(error => this.handleAuthError(error))
        )
      )
    )
  );

  /**
   * authErrors$ - Handles authentication errors
   * 
   * Listens for loadAuthFailure action and:
   * 1. Displays error notification using renderNotify
   * 2. Does not dispatch additional actions (dispatch: false)
   */
  authErrors$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(loadAuthFailure),
        tap(({ error }) => {
          this.store.dispatch(
            renderNotify({ 
              title: 'Authentication Failed', 
              text: error, 
              css: 'bg-danger' 
            })
          );
        })
      ),
    { dispatch: false }
  );

  /* ---------------------------- PRIVATE METHODS ---------------------------- */

  /**
   * Handles successful authentication response
   * @param response API response from AuthService
   * @returns loadAuthSuccess action with auth data
   */
  private handleAuthResponse(response: any) {
    
    if (response.status === 'success') {
      const auth: IAUTH = {
        id: 1, // Consider using actual user ID from response
        Token: response.token,
        isAuthenticated: true,
        User: response.post,
        Role: response.role,
      };
      return loadAuthSuccess({ auth });
    }
    return loadAuthFailure({ 
      error: response.message || 'Authentication failed' 
    });
  }

  /**
   * Handles authentication errors
   * @param error Error from AuthService
   * @returns loadAuthFailure action with error message
   */
  private handleAuthError(error: any) {
    return of(loadAuthFailure({ 
      error: error.message || 'Server connection failed' 
    }));
  }
}