
import { DefaultQuery, ActionTypes, DateFilter } from '../../configs/query.types'

export type ITAG = DefaultQuery & {
    start_search_key?: string;
    tag_type: TagType;
    type: Types;
    tag_level: TagLevel
}

export enum Types
{
    Blog = 0,
    Property = 1,
    Company = 2,
    All = 3
}

export enum TagType
{
    Normal = 0,
    UserSearches = 1,
    All = 2
}

export enum TagLevel
{
    High = 0,
    Medium = 1,
    Low = 2,
    All = 3
}

export const TAG_QUERY_OBJECT: ITAG = {
    start_search_key: '',
    tag_type: TagType.Normal,
    type: Types.Blog,
    tag_level: TagLevel.All,
    isapproved: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isenabled: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isarchive: ActionTypes.All, // for admin = All, for users choose disable or appropriate
    isdraft: ActionTypes.All,
    dateFilter: DateFilter.All,
    order: 'tag.id desc',
    pagenumber: 1,
    pagesize: 21, // 3 column

};


// Model
export interface  TagModel {
    id: number;
    title: string;
    term: string;
    isenabled: number;
    tag_level: number; // group by importance e.g low, medium, high
    tag_type : number; // group by type e.g normal tags, user searches
    type : number; // group by content e.g blogs, properties
    records : number;

    actionstatus?: string
}

export const Initial_Tag_Entity: TagModel = {
    id: 0,
    title: '',
    term: '',
    isenabled: 0,
    tag_level: 0, // group by importance e.g low, medium, high
    tag_type :0, // group by type e.g normal tags, user searches
    type : 0, // group by content e.g blogs, properties
    records :0
}



export const API_EndPoints: ApiOptions = {
  root: 'api/tags',
  load: 'api/tags/load',
  info: 'api/tags/getinfo',
  proc: 'api/tags/proc',
  action: 'api/tags/action',
};

export interface ApiOptions {
  root: string; // root api (without key - action)
  load: string; // load records
  info: string; // load single record
  proc: string; // save record
  action: string;
}


export interface TagResponse {
    status: string;
    message?: string;
    records?: 0;
    record?: TagModel
    posts?: TagModel[]
}
