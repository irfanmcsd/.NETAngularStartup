import { createAction, createReducer, createSelector, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { IAUTH } from './model';

/**
 * Authentication Store Configuration
 * 
 * This file contains the NgRx store setup for authentication including:
 * - Entity adapter configuration
 * - State interface definition
 * - Action definitions
 * - Reducer implementation
 * - Selector definitions
 */

/* ---------------------------- ENTITY ADAPTER CONFIGURATION ---------------------------- */

/**
 * Auth Entity Adapter
 * Provides standardized methods for managing auth state
 */
export const authAdapter = createEntityAdapter<IAUTH>({
  // Optional: Add sorting if needed
  // sortComparer: (a, b) => a.UserID.localeCompare(b.UserID),
});

/* ---------------------------- STATE DEFINITION ---------------------------- */

/**
 * Authentication State Interface
 * Extends EntityState with additional loading and error states
 */
export interface AuthState extends EntityState<IAUTH> {
  loading: boolean;      // Indicates if auth operation is in progress
  error: string | null;  // Stores error message if auth fails
}

/**
 * Initial Authentication State
 */
export const initialState: AuthState = authAdapter.getInitialState({
  loading: false,
  error: null,
});

/* ---------------------------- ACTION DEFINITIONS ---------------------------- */

/**
 * Action to initiate user authorization
 * @param UserID - The user identifier
 * @param IsAdmin - Flag indicating admin status
 */
export const authorizeUser = createAction(
  '[Auth] Authorize User',
  props<{ UserID: string; }>()
);

/**
 * Action dispatched on successful authentication
 * @param auth - The authentication payload
 */
export const loadAuthSuccess = createAction(
  '[Auth] Auth Success',
  props<{ auth: IAUTH }>()
);

/**
 * Action dispatched on authentication failure
 * @param error - The error message
 */
export const loadAuthFailure = createAction(
  '[Auth] Auth Failure',
  props<{ error: string }>()
);

/* ---------------------------- REDUCER IMPLEMENTATION ---------------------------- */

/**
 * Authentication Reducer
 * Handles state transitions for auth actions
 */
export const authReducer = createReducer(
  initialState,

  // Authorization started
  on(authorizeUser, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  // Authorization succeeded
  on(loadAuthSuccess, (state, { auth }) => {
    return authAdapter.setOne(auth, {  // Using setOne for single auth entity
      ...state,
      loading: false
    });
  }),

  // Authorization failed
  on(loadAuthFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);

/* ---------------------------- SELECTOR DEFINITIONS ---------------------------- */

/**
 * Selects the auth feature state from root state
 */
const selectAuthFeature = (rootState: any) => rootState.auth;

/**
 * Gets all auth entities from store
 */
export const selectAllAuth = createSelector(
  selectAuthFeature,
  authAdapter.getSelectors().selectAll
);

/**
 * Gets current authentication loading state
 */
export const selectAuthLoading = createSelector(
  selectAuthFeature,
  (state: AuthState) => state.loading
);

/**
 * Gets current authentication error
 */
export const selectAuthError = createSelector(
  selectAuthFeature,
  (state: AuthState) => state.error
);

/**
 * Gets current authenticated user
 */
export const selectCurrentUser = createSelector(
  selectAllAuth,
  (auths) => auths.length > 0 ? auths[0] : null
);

/**
 * Gets authentication token
 */
export const selectAuthToken = createSelector(
  selectCurrentUser,
  (user) => user?.Token || null
);

/**
 * Gets authentication status
 */
export const selectIsAuthenticated = createSelector(
  selectCurrentUser,
  (user) => user?.isAuthenticated || false
);