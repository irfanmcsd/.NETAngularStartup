import {
  DefaultQuery,
  ActionTypes,
  DateFilter,
} from '../../configs/query.types';

export type ICATEGORY = DefaultQuery & {
  parentid: number;
  start_search_key: string;
  type:Types
  isfeatured: FeaturedTypes;
}

export enum FeaturedTypes
{
  Basic = 0,
  Featured = 1,
  Premium = 2,
  All = 3
}

export enum Types
{
  Properties = 1,
  Properties_Rent = 4,
  Blogs = 2,
  Companies = 3
}

export const CATEGORY_QUERY_OBJECT: ICATEGORY = {
  parentid: -1, // skip
  start_search_key: '',
  type: Types.Properties,
  isfeatured: FeaturedTypes.All,
  isapproved: ActionTypes.All, // for admin = All, for users choose enable or appropriate
  isenabled: ActionTypes.All, // for admin = All, for users choose enable or appropriate
  isarchive: ActionTypes.All, // for admin = All, for users choose disable or appropriate
  isdraft: ActionTypes.All,
  dateFilter: DateFilter.All,
  pagenumber: 1,
  pagesize: 21 // 3 column
};

// Settings
export interface CategorySettings {
  thumbWidth: number;
  thumbHeight: number;
}

// Model

export interface CategoryModel {
   id: number;
   title: string;
   term: string;
   sub_term: string;
   parentid: number;
   type: number;
   priority: number;
   isenabled: number;
   isfeatured: number;
   avatar: string;
   records: number; 
   children: any[]
   isnew: boolean;
   selected: boolean;
   isopen: boolean;
   category_data: CategoryData
   culture_categories: CategoryData[]
   actionstatus?: string;
}

export interface CategoryData {
   id: number;
   categoryid: number;
   culture: string;
   title: string;
   sub_title: string;
   description: string;
}

export const Initial_Category_Settings: CategorySettings = {
  thumbWidth: 300,
  thumbHeight: 300
}

export const Initial_Category_Entity: CategoryModel = {
  id: 0,
  title: '',
  term: '',
  sub_term: '',
  parentid: 0,
  type: 0,
  priority: 0,
  isenabled: 1,
  isfeatured: 0,
  avatar: '',
  records: 0,
  children: [],
  isnew: false,
  selected: false,
  isopen: false,
  category_data: {
     id: 0,
     categoryid: 0,
     culture: 'en',
     title: '',
     sub_title: '',
     description: ''
  },
  culture_categories:[]
}

export const API_EndPoints: ApiOptions = {
  root: 'api/categories',
  load: 'api/categories/load',
  info: 'api/categories/getinfo',
  proc: 'api/categories/proc',
  action: 'api/categories/action',
};

export interface ApiOptions {
  root: string; // root api (without key - action)
  load: string; // load records
  info: string; // load single record
  proc: string; // save record
  action: string; // apply actions e.g enable, disable
}


export interface CategoryReponse {
    status: string;
    message?: string;
    records?: 0;
    record?: CategoryModel
    posts?: CategoryModel[]
}
