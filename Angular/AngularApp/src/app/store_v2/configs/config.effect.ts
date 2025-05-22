import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

// Services
import { ConfigService } from './config.services';

// Actions
import {
  loadConfigs,
  loadConfigsSuccess,
  loadConfigsFailure,
} from './config.reducer';

// Models
import { ConfigReponse, ICONFIG } from './model';

/**
 * ConfigEffects - NgRx effects for configuration management
 * 
 * Handles side effects for configuration-related actions including:
 * - Loading application configuration
 * - Success/failure handling
 * - Navigation on certain conditions
 */
@Injectable()
export class ConfigEffects {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly actions$ = inject(Actions);
  private readonly configService = inject(ConfigService);
  private readonly router = inject(Router);

  /* ---------------------------- EFFECT DEFINITIONS ---------------------------- */
  
  /**
   * loadConfigs$ - Effect for loading application configuration
   * 
   * Listens for loadConfigs action and:
   * 1. Calls ConfigService to fetch configuration
   * 2. On success:
   *    - Transforms response into ICONFIG format
   *    - Dispatches loadConfigsSuccess with the configuration
   * 3. On failure:
   *    - Dispatches loadConfigsFailure with error message
   */
  loadConfigs$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadConfigs),
      switchMap(() => 
        this.configService.loadRecords().pipe(
          map(response => this.handleConfigResponse(response)),
          catchError(error => this.handleConfigError(error))
        )
      )
    )
  );

  /* ---------------------------- PRIVATE METHODS ---------------------------- */
  
  /**
   * Handles successful configuration response
   * @param response The API response
   * @returns Appropriate action based on response status
   */
  private handleConfigResponse(response: ConfigReponse) {
    if (response.status === 'success') {
      const configs: ICONFIG = {
        id: 0,
        configs: response,
        isloaded: true
      };
      return loadConfigsSuccess({ Configs: configs });
    } else {
      return loadConfigsFailure({ 
        error: response.message || 'Unknown error occurred' 
      });
    }
  }

  /**
   * Handles configuration loading errors
   * @param error The error object
   * @returns loadConfigsFailure action with error message
   */
  private handleConfigError(error: any) {
    return of(loadConfigsFailure({ 
      error: error.message || 'Failed to load configuration' 
    }));
  }
}