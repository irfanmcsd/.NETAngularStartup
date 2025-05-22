import {
  createAction,
  createReducer,
  createSelector,
  on,
  props,
} from '@ngrx/store';
import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { IListStats } from '../../configs/query.types';
import { CategoryModel, ICATEGORY, CATEGORY_QUERY_OBJECT } from './model';


export const categoryAdapter = createEntityAdapter<CategoryModel>();

// State interface
export interface CategoryState extends EntityState<CategoryModel> {
  // pagination related
  records: number;
  loading: boolean;
  error: string | null;
  selectedIds: number[];
  navList: CategoryModel[];
}

// 4. Create initial state
export const initialState: CategoryState = categoryAdapter.getInitialState({
  records: 0,
  loading: false,
  error: null,
  selectedIds: [],
  navList: [],
});

// 5. Define Actions
export const loadCategories = createAction(
  '[Categories] Load Categories',
  props<{ Query: ICATEGORY }>()
);

export const loadCategoriesSuccess = createAction(
  '[Categories] Load Categories Success',
  props<{
    categories: CategoryModel[];
    navList: CategoryModel[];
    records: number;
  }>()
);

export const loadCategoriesFailure = createAction(
  '[Categories] Load Categories Failure',
  props<{ error: string }>()
);

export const addCategories = createAction(
  '[Categories] Add Categories',
  props<{ category: Omit<CategoryModel, 'id'>; Entity: CategoryModel }>()
);

export const addCategoriesSuccess = createAction(
  '[Categories] Add Categories Success',
  props<{ category: CategoryModel }>()
);

export const deleteCategorySuccess = createAction(
  '[Categories] Delete Category Success',
  props<{ id: number }>()
);



export const updateCategorySuccess = createAction(
  '[Categories] Update Categories Success',
  props<{ category: CategoryModel }>()
);

export const addCategoriesFailure = createAction(
  '[Categories] Add Categories Failure',
  props<{ error: string }>()
);

export const actionCategories = createAction(
  '[Categories] Action Categories',
  props<{
    Entities: CategoryModel[];
    actionstatus: string;
  }>()
);

export const categoryActionSuccess = createAction(
  '[Categories] Add Categories Action Success',
  props<{
    entities: CategoryModel[];
    actionstatus: string;
  }>()
);

export const categoryActionFailure = createAction(
  '[Categories] Add Categories Action Failure',
  props<{ error: string }>()
);

export const toggleCategorySelection = createAction(
  '[Categories] Toggle Category Selection',
  props<{ id: number }>()
);

export const categoryReducer = createReducer(
  initialState,

  on(loadCategories, (state) => ({ ...state, loading: true, error: null })),
  on(loadCategoriesSuccess, (state, { categories, navList, records }) => {
    return categoryAdapter.setAll(categories, {
      ...state,
      records,
      navList,
      loading: false,
    });
  }),
  on(loadCategoriesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  /*on(addCategoriesSuccess, (state, { category }) => 
    categoryAdapter.addOne(category, { ...state })
  ),*/

  on(addCategoriesSuccess, (state, { category }) => {
    // First add the category to the entity state

    const updatedState = categoryAdapter.addOne(category, state);

    // Process the navList update
    let updatedNavList: CategoryModel[];

    if (
      category.parentid === null ||
      category.parentid === undefined ||
      category.parentid === 0
    ) {
      // If it's a root category, add to navList
      updatedNavList = [...state.navList, category];
    } else {
      // If it's a child category, find its parent in navList and add it
      updatedNavList = state.navList.map((parentCategory) => {
        if (parentCategory.id === category.parentid) {
          // Clone the parent category and add the new child
          return {
            ...parentCategory,
            children: [...(parentCategory.children || []), category],
          };
        }
        return parentCategory;
      });
    }

    // Return the fully updated state
    return {
      ...updatedState,
      navList: updatedNavList,
    };
  }),

  on(updateCategorySuccess, (state, { category }) => {
    console.log('update cateogry info >>:' + category);
    // First update the category in the entity state
    const updatedState = categoryAdapter.updateOne(
      {
        id: category.id,
        changes: category,
      },
      state
    );

    // Process the navList update
    const updatedNavList = updateCategoryInNavList(state.navList, category);

    return {
      ...updatedState,
      navList: updatedNavList,
    };
  }),

  on(actionCategories, (state) => ({ ...state, loading: true, error: null })),

  on(deleteCategorySuccess, (state, { id }) => {
    // First get the category to be deleted from the entity state
    const categoryToDelete = state.entities[id];

    if (!categoryToDelete) {
      return state; // category not found, return current state
    }

    // Remove the category from the entity state
    const updatedState = categoryAdapter.removeOne(id, state);

    // Process the navList update
    let updatedNavList: CategoryModel[];

    if (
      categoryToDelete.parentid === null ||
      categoryToDelete.parentid === undefined ||
      categoryToDelete.parentid === 0
    ) {
      // It's a root category - remove it from navList
      updatedNavList = state.navList.filter((cat) => cat.id !== id);
    } else {
      // It's a child category - find and remove it from its parent's children
      updatedNavList = state.navList.map((parentCategory) => {
        if (parentCategory.id === categoryToDelete.parentid) {
          // Filter out the deleted category from children
          return {
            ...parentCategory,
            children: (parentCategory.children || []).filter(
              (child) => child.id !== id
            ),
          };
        }
        return parentCategory;
      });
    }

    // Also handle the case where the deleted category might have children
    // This will remove any children of the deleted category from the entity state
    const childrenToRemove = Object.values(state.entities)
      .filter((cat) => cat?.parentid === id)
      .map((cat) => cat!.id);

    // If there are children, remove them too
    const finalState =
      childrenToRemove.length > 0
        ? categoryAdapter.removeMany(childrenToRemove, updatedState)
        : updatedState;

    // Also remove children from navList if the deleted category was a parent in navList
    if (categoryToDelete.children && categoryToDelete.children.length > 0) {
      updatedNavList = updatedNavList.map((cat) => {
        if (cat.id === id) {
          // This handles the case where the deleted category might still be in navList
          // (though it shouldn't be if we got here)
          return {
            ...cat,
            children: [],
          };
        }
        return cat;
      });
    }

    return {
      ...finalState,
      navList: updatedNavList,
    };
  }),
  on(categoryActionSuccess, (state, { entities, actionstatus }) => {
    return categoryAdapter.updateMany(
        entities.map((entity) => ({
          id: entity.id,
          changes: { ...entity },
        })),
        {
          ...state,
          loading: false,
        }
      );
  }),

  on(toggleCategorySelection, (state, { id }) => {
    const selectedIds = [...state.selectedIds];
    const index = selectedIds.indexOf(id);

    if (index === -1) {
      // ID not found, add it
      selectedIds.push(id);
    } else {
      // ID found, remove it
      selectedIds.splice(index, 1);
    }

    return {
      ...state,
      selectedIds,
    };
  })
);

// Selectors
export const selectCategoryState = (rootState: any) => rootState.category;
export const selectAllCategories = createSelector(
  selectCategoryState,
  (state: CategoryState) => {
    return categoryAdapter.getSelectors().selectAll(state);
  }
);

export const selectCategoriesLoading = (state: CategoryState) => state.loading;

export const selectNavList = createSelector(
  selectCategoryState,
  (state: CategoryState) => state.navList || [] // Return empty array if undefined
);

// Selector for root categories (optional - if your navList contains only roots)
export const selectRootCategories = createSelector(selectNavList, (navList) =>
  navList.filter(
    (category) =>
      category.parentid === null ||
      category.parentid === undefined ||
      category.parentid === 0
  )
);

// Selector for nested children of a specific category
export const selectCategoryChildren = (parentId: number) =>
  createSelector(selectNavList, (navList) => {
    if (parentId === 0) {
      return navList;
    } else {
      console.log('child nav list: nav list: ', navList);
      const parent = navList.find((cat) => cat.id === parentId);
      return parent?.children || [];
    }
  });

// Selector for a specific category by ID from navList
export const selectCategoryFromNav = (id: number) =>
  createSelector(selectNavList, (navList) => {
    // Flatten the navList to search through all levels
    const flattenCategories = (
      categories: CategoryModel[]
    ): CategoryModel[] => {
      return categories.reduce<CategoryModel[]>(
        (acc, category) => [
          ...acc,
          category,
          ...(category.children ? flattenCategories(category.children) : []),
        ],
        []
      );
    };

    return flattenCategories(navList).find((cat) => cat.id === id);
  });

export const selectFilteredNavList = (searchTerm: string) =>
  createSelector(selectNavList, (navList) => {
    if (!searchTerm) return navList; // Return all if no search term

    const lowerSearchTerm = searchTerm.toLowerCase();

    const filterCategories = (categories: CategoryModel[]): CategoryModel[] => {
      return categories.reduce<CategoryModel[]>((filtered, category) => {
        // Check if current category matches
        const matches = category.category_data.title
          .toLowerCase()
          .includes(lowerSearchTerm);

        // Filter children recursively
        const filteredChildren = category.children
          ? filterCategories(category.children)
          : undefined;

        // Include if current matches OR has matching children
        if (matches || (filteredChildren && filteredChildren.length > 0)) {
          filtered.push({
            ...category,
            children: filteredChildren || [],
          });
        }

        return filtered;
      }, []);
    };

    return filterCategories(navList);
  });

/*export const selectFilteredNavListV2 = memoize((searchTerm: string) => 
  createSelector(
    selectNavList,
    (navList) => {
      if (!searchTerm?.trim()) return navList;
      
      const term = searchTerm.toLowerCase();
      const searchProperties = ['name', 'description']; // Add other searchable props
      
      const searchInCategory = (category: CategoryModel): boolean => {
        return searchProperties.some(prop => 
          (category as Record<string, any>)[prop]?.toLowerCase().includes(term)
        );
      };
      
      const filterCategories = (categories: CategoryModel[]): CategoryModel[] => {
        return categories
          .map(category => ({
            ...category,
            children: category.children ? filterCategories(category.children) : []
          }))
          .filter(category => 
            searchInCategory(category) || 
            (category.children && category.children.length > 0)
          );
      };
      
      return filterCategories(navList);
    }
  )
);*/

// Selected category selector
export const selectSelectedCategoryIds = (state: CategoryState) =>
  state.selectedIds;

export const selectSelectedCategories = createSelector(
  selectAllCategories, // Assuming you have this selector from the adapter
  selectSelectedCategoryIds,
  (categories, selectedIds) =>
    categories.filter((cat) => selectedIds.includes(cat.id))
);

export const selectIsCategorySelected = (id: number) =>
  createSelector(selectSelectedCategoryIds, (selectedIds) =>
    (selectedIds ?? []).includes(id)
  );

// Helper function to recursively update category in navList
function updateCategoryInNavList(
  navList: CategoryModel[],
  updatedCategory: CategoryModel
): CategoryModel[] {
  return navList.map((category) => {
    if (category.id === updatedCategory.id) {
      // Found the category to update - merge with existing children
      return {
        ...updatedCategory,
        children: category.children, // Preserve existing children
      };
    }

    if (category.children && category.children.length > 0) {
      // Recursively update in children
      return {
        ...category,
        children: updateCategoryInNavList(category.children, updatedCategory),
      };
    }

    return category;
  });
}
