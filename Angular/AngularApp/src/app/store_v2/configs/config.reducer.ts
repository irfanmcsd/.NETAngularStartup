import { createAction, createReducer, createSelector, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { ICONFIG } from './model';

/**
 * NgRx Store Configuration for Application Settings
 * 
 * This file contains:
 * - Entity adapter configuration for Config state
 * - State interface definition
 * - Action definitions
 * - Reducer implementation
 * - Selector definitions
 */

/* ---------------------------- ENTITY ADAPTER CONFIGURATION ---------------------------- */

/**
 * Creates entity adapter for ICONFIG objects
 * Provides standardized methods for managing config state
 */
export const configAdapter = createEntityAdapter<ICONFIG>({
  // Optional: Add sorting if needed
  // sortComparer: (a, b) => a.id.localeCompare(b.id),
});

/* ---------------------------- STATE DEFINITION ---------------------------- */

/**
 * Interface representing the configuration module state
 * Extends EntityState to include loading and error status
 */
export interface ConfigState extends EntityState<ICONFIG> {
  loading: boolean;      // Indicates if config is being loaded
  error: string | null;  // Stores error message if loading fails
}

/**
 * Initial state for the configuration store
 */
export const initialState: ConfigState = configAdapter.getInitialState({
  loading: false,
  error: null,
});

/* ---------------------------- ACTION DEFINITIONS ---------------------------- */

/**
 * Action to trigger configuration loading
 */
export const loadConfigs = createAction(
  '[Config] Load Configurations'
);

/**
 * Action dispatched when configurations are successfully loaded
 * @param {ICONFIG} Configs - The loaded configuration data
 */
export const loadConfigsSuccess = createAction(
  '[Config] Load Configurations Success',
  props<{ Configs: ICONFIG }>()
);

/**
 * Action dispatched when configuration loading fails
 * @param {string} error - Error message
 */
export const loadConfigsFailure = createAction(
  '[Config] Load Configurations Failure',
  props<{ error: string }>()
);

/* ---------------------------- REDUCER IMPLEMENTATION ---------------------------- */

/**
 * Configuration reducer function
 * Handles state transitions for configuration actions
 */
export const configReducer = createReducer(
  initialState,

  // Loading started
  on(loadConfigs, (state) => ({ 
    ...state, 
    loading: true, 
    error: null 
  })),

  // Loading succeeded
  on(loadConfigsSuccess, (state, { Configs }) => {
    return configAdapter.setOne(Configs, {  // Using setOne instead of setAll for single config
      ...state,
      loading: false,
    });
  }),

  // Loading failed
  on(loadConfigsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
);

/* ---------------------------- SELECTOR DEFINITIONS ---------------------------- */

/**
 * Selects the entire config state from root state
 */
const selectConfigState = (rootState: any) => rootState.config;

/**
 * Gets all configurations from the store
 */
export const selectAllConfigs = createSelector(
  selectConfigState,
  configAdapter.getSelectors().selectAll
);

/**
 * Gets loading state from config store
 */
export const selectConfigsLoading = createSelector(
  selectConfigState,
  (state: ConfigState) => state.loading
);

/**
 * Gets error state from config store
 */
export const selectConfigsError = createSelector(
  selectConfigState,
  (state: ConfigState) => state.error
);

/**
 * Gets the first (usually only) configuration
 * Useful when only one config exists
 */
export const selectMainConfig = createSelector(
  selectAllConfigs,
  (configs) => configs.length > 0 ? configs[0] : null
);