

import { DefaultQuery, ActionTypes, DateFilter } from '../../configs/query.types'

export type ILOG = DefaultQuery & {
   
}

export const LOG_QUERY_OBJECT: ILOG = {
    isapproved: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isenabled: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isarchive: ActionTypes.All, // for admin = All, for users choose disable or appropriate
    isdraft: ActionTypes.All,
    dateFilter: DateFilter.All,
    pagenumber: 1,
    pagesize: 21, // 3 column
};

// Model

export interface ErrorLogModel {
     id: number;
     description: string;
     url: string;
     stack_trace: string;
     created_at?: Date;
     
     actionstatus?: string;
}


export const Initial_Log_Entity: ErrorLogModel = {
    id: 0,
    description: '',
    url: '',
    stack_trace: ''
}

export const API_EndPoints: ApiOptions = {
  root: 'api/log',
  load: 'api/log/load',
  info: 'api/log/getinfo',
  proc: 'api/log/proc',
  deleteall: 'api/log/deleteall',
  action: 'api/companies/action',
};

export interface ApiOptions {
  root: string; // root api (without key - action)
  load: string; // load records
  info: string; // load single record
  proc: string; // save record
  deleteall: string;
  action: string; // apply actions e.g enable, disable
}


export interface ErrorLogReponse {
    status: string;
    message?: string;
    records?: 0;
    record?: ErrorLogModel
    posts?: ErrorLogModel[]
}
