import { createAction, createReducer, createSelector, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { TagModel, ITAG } from './model';

export const tagAdapter = createEntityAdapter<TagModel>();

// State interface
export interface Tagstate extends EntityState<TagModel> {
  records: number;
  loading: boolean;
  error: string | null;
}

// 4. Create initial state
export const initialState: Tagstate = tagAdapter.getInitialState({
  records: 0,
  loading: false,
  error: null,
});

// 5. Define Actions
export const loadTags = createAction(
  '[Tags] Load Tags',
  props<{ Query: ITAG }>()
);

export const loadTagsSuccess = createAction(
  '[Tags] Load Tags Success',
  props<{ Tags: TagModel[], records: number}>()
);

export const loadTagsFailure = createAction(
  '[Tags] Load Tags Failure',
  props<{ error: string }>()
);

export const addTags = createAction(
  '[Tags] Add Tags',
  props<{ Tag: Omit<TagModel, 'id'>; Entity: TagModel }>()
);

export const addTagsSuccess = createAction(
  '[Tags] Add Tags Success',
  props<{ Tag: TagModel }>()
);

export const addTagsFailure = createAction(
  '[Tags] Add Tags Failure',
  props<{ error: string }>()
);

export const actionTags = createAction(
  '[Tags] Action Tags',
  props<{
    Entities: TagModel[];
    actionstatus: string;
  }>()
);

export const TagActionSuccess = createAction(
  '[Tags] Add Tags Action Success',
  props<{
    entities: TagModel[];
    actionstatus: string;
  }>()
);

export const TagActionFailure = createAction(
  '[Tags] Add Tags Action Failure',
  props<{ error: string }>()
);


export const tagReducer = createReducer(
  initialState,

  on(loadTags, (state) => ({ ...state, loading: true, error: null })),
  on(loadTagsSuccess, (state, { Tags, records }) => {
    return tagAdapter.setAll(Tags, {
      ...state,
      records,
      loading: false,
    });
  }),
  on(loadTagsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(addTagsSuccess, (state, { Tag }) => tagAdapter.addOne(Tag, state)),

  on(actionTags, (state) => ({ ...state, loading: true, error: null })),

  on(TagActionSuccess, (state, { entities, actionstatus }) => {
    if (actionstatus === 'delete') {
      return tagAdapter.removeMany(
        entities.map((entity) => entity.id),
        {
          ...state,
          loading: false,
        }
      );
    } else {
      return tagAdapter.updateMany(
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
export const selectTagState = (rootState: any) => rootState.tag;
export const selectAllTags = createSelector(
  selectTagState,
  (state: Tagstate) => {
    return tagAdapter.getSelectors().selectAll(state);
  }
);

// Selector for the loading property
export const selectTagsLoading = createSelector(
  selectTagState,
  (state: Tagstate) => state.loading
);

// Selector for the records property
export const selectRecords = createSelector(
  selectTagState,
  (state: Tagstate) => state.records
);
