import {
  createAction,
  createReducer,
  createFeatureSelector,
  createSelector,
  on,
  props,
} from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';

// create model
export interface CoreEvent {
  id?: number;
  action: string;
  data: any;
}

// create adapter
export const eventAdapter = createEntityAdapter<CoreEvent>();

export interface EventState extends EntityState<CoreEvent> {}

export const eventInitialState: EventState = eventAdapter.getInitialState({});

// create actions
export const pushData = createAction(
  '[Event] Push Event',
  props<{ event: CoreEvent[] }>()
);

export const eventReducer = createReducer(
  eventInitialState,

  on(pushData, (state, { event }) => {
    console.log('Reducer received event:', event); // Add logging
    return eventAdapter.setAll(event, {
      ...state
    });
  }),

  on(pushData, (state, Event) => {
    console.log('eventReducer', Event);
    return { ...state };
  })
);

// create selectors
export const selectEventState = (rootState: any) => rootState.event;
export const pushSelector = createSelector(
  selectEventState,
  (state: EventState) => {
    return eventAdapter.getSelectors().selectAll(state);
  }
);

