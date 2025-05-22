import { createAction, createReducer, createSelector, on, props } from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { UserModel, IUSER } from './model';

export const UserAdapter = createEntityAdapter<UserModel>();

// State interface
export interface UserState extends EntityState<UserModel> {
  records: number;
  loading: boolean;
  error: string | null;
}

// 4. Create initial state
export const initialState: UserState = UserAdapter.getInitialState({
  records: 0,
  loading: false,
  error: null,
});

// 5. Define Actions
export const loadUsers = createAction(
  '[Users] Load Users',
  props<{ Query: IUSER }>()
);

export const loadUsersSuccess = createAction(
  '[Users] Load Users Success',
  props<{ Users: UserModel[], records: number}>()
);

export const loadUsersFailure = createAction(
  '[Users] Load Users Failure',
  props<{ error: string }>()
);

export const addUsers = createAction(
  '[Users] Add Users',
  props<{ User: Omit<UserModel, 'id'>; Entity: UserModel }>()
);

export const addUsersSuccess = createAction(
  '[Users] Add Users Success',
  props<{ User: UserModel }>()
);

export const addUsersFailure = createAction(
  '[Users] Add Users Failure',
  props<{ error: string }>()
);

export const actionUsers = createAction(
  '[Users] Action Users',
  props<{
    Entities: UserModel[];
    actionstatus: string;
  }>()
);

export const UserActionSuccess = createAction(
  '[Users] Add Users Action Success',
  props<{
    entities: UserModel[];
    actionstatus: string;
  }>()
);

export const UserActionFailure = createAction(
  '[Users] Add Users Action Failure',
  props<{ error: string }>()
);


export const userReducer = createReducer(
  initialState,

  on(loadUsers, (state) => ({ ...state, loading: true, error: null })),
  on(loadUsersSuccess, (state, { Users, records }) => {
    return UserAdapter.setAll(Users, {
      ...state,
      records,
      loading: false,
    });
  }),
  on(loadUsersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(addUsersSuccess, (state, { User }) => UserAdapter.addOne(User, state)),

  on(actionUsers, (state) => ({ ...state, loading: true, error: null })),

  on(UserActionSuccess, (state, { entities, actionstatus }) => {
    if (actionstatus === 'delete') {
      return UserAdapter.removeMany(
        entities.map((entity) => entity.id),
        {
          ...state,
          loading: false,
        }
      );
    } else {
      return UserAdapter.updateMany(
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

export const selectUserState = (rootState: any) => rootState.user;
export const selectAllUsers = createSelector(
  selectUserState,
  (state: UserState) => {
    return UserAdapter.getSelectors().selectAll(state);
  }
);

export const selectUsersLoading = (state: UserState) => state.loading;

// Selector for the records property
export const selectRecords = createSelector(
  selectUserState,
  (state: UserState) => state.records
);
