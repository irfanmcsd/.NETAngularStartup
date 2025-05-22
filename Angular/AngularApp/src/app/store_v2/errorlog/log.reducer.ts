import { createAction, createReducer, createSelector, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { ErrorLogModel, ILOG } from './model';

export const ErrorLogAdapter = createEntityAdapter<ErrorLogModel>();

// State interface
export interface ErrorLogState extends EntityState<ErrorLogModel> {
  records: number;
  loading: boolean;
  error: string | null;
}

// 4. Create initial state
export const initialState: ErrorLogState = ErrorLogAdapter.getInitialState({
  records: 0,
  loading: false,
  error: null,
});

// 5. Define Actions
export const loadErrorLogs = createAction(
  '[ErrorLogs] Load ErrorLogs',
  props<{ Query: ILOG }>()
);

export const loadErrorLogsSuccess = createAction(
  '[ErrorLogs] Load ErrorLogs Success',
  props<{ ErrorLogs: ErrorLogModel[], records: number}>()
);

export const loadErrorLogsFailure = createAction(
  '[ErrorLogs] Load ErrorLogs Failure',
  props<{ error: string }>()
);

export const addErrorLogs = createAction(
  '[ErrorLogs] Add ErrorLogs',
  props<{ ErrorLog: Omit<ErrorLogModel, 'id'>; Entity: ErrorLogModel }>()
);

export const addErrorLogsSuccess = createAction(
  '[ErrorLogs] Add ErrorLogs Success',
  props<{ ErrorLog: ErrorLogModel }>()
);

export const addErrorLogsFailure = createAction(
  '[ErrorLogs] Add ErrorLogs Failure',
  props<{ error: string }>()
);

export const actionErrorLogs = createAction(
  '[ErrorLogs] Action ErrorLogs',
  props<{
    Entities: ErrorLogModel[];
    actionstatus: string;
  }>()
);

export const ErrorLogActionSuccess = createAction(
  '[ErrorLogs] Add ErrorLogs Action Success',
  props<{
    entities: ErrorLogModel[];
    actionstatus: string;
  }>()
);

export const ErrorLogActionFailure = createAction(
  '[ErrorLogs] Add ErrorLogs Action Failure',
  props<{ error: string }>()
);


export const errorLogReducer = createReducer(
  initialState,

  on(loadErrorLogs, (state) => ({ ...state, loading: true, error: null })),
  on(loadErrorLogsSuccess, (state, { ErrorLogs, records }) => {
    return ErrorLogAdapter.setAll(ErrorLogs, {
      ...state,
      records,
      loading: false,
    });
  }),
  on(loadErrorLogsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(addErrorLogsSuccess, (state, { ErrorLog }) => ErrorLogAdapter.addOne(ErrorLog, state)),

  on(actionErrorLogs, (state) => ({ ...state, loading: true, error: null })),

  on(ErrorLogActionSuccess, (state, { entities, actionstatus }) => {
    if (actionstatus === 'delete') {
      return ErrorLogAdapter.removeMany(
        entities.map((entity) => entity.id),
        {
          ...state,
          loading: false,
        }
      );
    } else {
      return ErrorLogAdapter.updateMany(
        entities.map((entity) => ({
          id: entity.id,
          changes: { ...entity },
        })),
        {
          ...state,
          loading: false,
        }
      );
    }
  })
);

// Selectors
export const selectErrorLogState = (rootState: any) => rootState.errorlog;
export const selectAllErrorLogs = createSelector(
  selectErrorLogState,
  (state: ErrorLogState) => {
    return ErrorLogAdapter.getSelectors().selectAll(state);
  }
);

// Selector for the records property
export const selectErrorLogsLoading = createSelector(
  selectErrorLogState,
  (state: ErrorLogState) => state.loading
);


// Selector for the records property
export const selectRecords = createSelector(
  selectErrorLogState,
  (state: ErrorLogState) => state.records
);

