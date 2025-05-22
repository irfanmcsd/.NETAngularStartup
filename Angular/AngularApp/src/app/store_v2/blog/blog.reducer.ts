import {
  createAction,
  createReducer,
  createSelector,
  on,
  props,
} from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { BlogModel, IBLOGS } from './model';

/**
 * Creates an entity adapter for BlogModel with default sort comparer
 * The adapter provides CRUD operations and selectors for the entity collection
 */
export const blogAdapter = createEntityAdapter<BlogModel>();

/**
 * Interface representing the Blog feature state
 * Extends EntityState<BlogModel> to include entity collection methods
 */
export interface BlogState extends EntityState<BlogModel> {
  loading: boolean; // Indicates if async operation is in progress
  records: number; // Total number of records available on server
  error: string | null; // Last error message, if any
}

/**
 * Initial state for the Blog feature
 * Uses blogAdapter to initialize entity state and adds custom properties
 */
export const initialState: BlogState = blogAdapter.getInitialState({
  loading: false,
  records: 0,
  error: null,
});

// ======================
// ACTION DEFINITIONS
// ======================

/**
 * Action to trigger loading of blogs with query parameters
 * @param Query - The query parameters for filtering/sorting blogs
 */
export const loadBlogs = createAction(
  '[Blogs] Load Blogs',
  props<{ Query: IBLOGS }>()
);

/**
 * Action dispatched when blogs are successfully loaded
 * @param blogs - Array of loaded blog entities
 * @param records - Total count of records available on server
 */
export const loadBlogsSuccess = createAction(
  '[Blogs] Load Blogs Success',
  props<{ blogs: BlogModel[]; records: number }>()
);

/**
 * Action dispatched when blog loading fails
 * @param error - Error message describing the failure
 */
export const loadBlogsFailure = createAction(
  '[Blogs] Load Blogs Failure',
  props<{ error: string }>()
);

/**
 * Action to add a new blog
 * @param Entity - The blog entity to be added
 */
export const addBlogs = createAction(
  '[Blogs] Add Blogs',
  props<{ Entity: BlogModel }>()
);

/**
 * Action dispatched when a blog is successfully added
 * @param blogs - The newly added blog entity
 */
export const addBlogsSuccess = createAction(
  '[Blogs] Add Blogs Success',
  props<{ blogs: BlogModel }>()
);

/**
 * Action dispatched when a blog is successfully updated
 * @param blogs - The updated blog entity
 */
export const updateBlogsSuccess = createAction(
  '[Blogs] Update Blogs Success',
  props<{ blogs: BlogModel }>()
);

/**
 * Action dispatched when blog addition fails
 * @param error - Error message describing the failure
 */
export const addBlogsFailure = createAction(
  '[Blogs] Add Blogs Failure',
  props<{ error: string }>()
);

/**
 * Action to perform batch operations on blogs
 * @param Entities - Array of blog entities to be processed
 * @param actionstatus - The type of action to perform (e.g., 'update', 'delete')
 */
export const actionBlogs = createAction(
  '[Blogs] Action Blogs',
  props<{ Entities: BlogModel[]; actionstatus: string }>()
);

/**
 * Action dispatched when batch operation succeeds
 * @param entities - Array of processed blog entities
 * @param actionstatus - The type of action that was performed
 */
export const addBlogsActionSuccess = createAction(
  '[Blogs] Add Blogs Action Success',
  props<{ entities: BlogModel[]; actionstatus: string }>()
);

/**
 * Action dispatched when batch operation fails
 * @param error - Error message describing the failure
 */
export const addBlogsActionFailure = createAction(
  '[Blogs] Add Blogs Action Failure',
  props<{ error: string }>()
);

// ======================
// REDUCER DEFINITION
// ======================

/**
 * Blog feature reducer that handles all blog-related actions
 */
export const blogReducer = createReducer(
  initialState,

  // Load Blogs handlers
  on(loadBlogs, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(loadBlogsSuccess, (state, { blogs, records }) =>
    blogAdapter.setAll(blogs, {
      ...state,
      records,
      loading: false,
    })
  ),

  on(loadBlogsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Add Blog handlers
  on(addBlogsSuccess, (state, { blogs }) => blogAdapter.addOne(blogs, state)),

  on(updateBlogsSuccess, (state, { blogs }) =>
    blogAdapter.updateOne(
      {
        id: blogs.id,
        changes: blogs,
      },
      state
    )
  ),
  // Batch Action handlers
  on(actionBlogs, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(addBlogsActionSuccess, (state, { entities, actionstatus }) => {
    if (actionstatus === 'delete') {
      return blogAdapter.removeMany(
        entities.map((entity) => entity.id),
        {
          ...state,
          loading: false,
        }
      );
    }
    return blogAdapter.updateMany(
      entities.map((entity) => ({
        id: entity.id,
        changes: { ...entity },
      })),
      {
        ...state,
        loading: false,
      }
    );
  })
);

// ======================
// SELECTOR DEFINITIONS
// ======================

/**
 * Selects the blog feature state from the root state
 * @param rootState - The root application state
 */
export const selectBlogState = (rootState: any) => rootState.blog;

/**
 * Selects all blog entities from the state
 */
export const selectAllBlogs = createSelector(
  selectBlogState,
  blogAdapter.getSelectors().selectAll
);

/**
 * Selects the loading status from blog state
 * @param state - The blog feature state
 */
export const selectBlogsLoading = createSelector(
  selectBlogState,
  (state: BlogState) => state.loading
);


// Selector for the records property
export const selectRecords = createSelector(
  selectBlogState,
  (state: BlogState) => state.records
);
