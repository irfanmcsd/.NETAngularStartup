import { createAction, createReducer, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';

// create model
export interface Notify {
    title: string;
    text: string;
    css: string;
}

// create adapter
export const coreAdapter = createEntityAdapter<Notify>();

export interface CoreState extends EntityState<Notify> {
}

export const coreInitialState: CoreState = coreAdapter.getInitialState({
});

// create actions
export const renderNotify = createAction('[Notify] Render Notify', props<Notify>());

export const notifyReducer = createReducer(
  coreInitialState,
  on(renderNotify, (state, Notify) => {
    return ({ ...state });
  })
);

// create selectors
export const renderSelector = (state: CoreState) => state;