import { DefaultQuery, ActionTypes, DateFilter, ExpiryOptions, FeaturedTypes, ChartGroupBy } from '../../configs/query.types'

export type IUSER = DefaultQuery & {
    company_slug?: string;
    action_type: ActionType;
    agent_type: AgentTypes;
    Id?: string;
    locationid: number;
    city?: string;
    state?: string;
    country?: string;
    groupby: ChartGroupBy;
    user_rolename?:string;
    user_roleid?: string;
    emailconfirmed?: ActionTypes,
    companyid: number;
    character?: string;
    isfeatured: FeaturedTypes;
    subscription_expiry_option: ExpiryOptions;
}

export enum AgentTypes
{
    Dealer = 1,
    Builder = 2,
    Electrician = 3,
    Plumber = 4,
    All = 5
}
   
export enum ActionType
{
    Management = 0, // internal users e.g admins, moderators that have roles
    NonManagement = 1 // external users, logged in users, company users, agent, service provider etc
}

export const USER_QUERY_OBJECT: IUSER = {
    locationid: 0, // skip
    action_type: ActionType.NonManagement,
    agent_type: AgentTypes.All,
    isfeatured: FeaturedTypes.All,
    companyid: 0,
    ispublic: false,
    groupby: ChartGroupBy.None,
    subscription_expiry_option: ExpiryOptions.All,
    isapproved: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isenabled: ActionTypes.All, // for admin = All, for users choose enable or appropriate
    isarchive: ActionTypes.All, // for admin = All, for users choose disable or appropriate
    isdraft: ActionTypes.All,
    dateFilter: DateFilter.All,
    emailconfirmed: ActionTypes.All,
    pagenumber: 1,
    pagesize: 21, // 3 column
    order: "user.createdAt desc"
};

// Settings
export interface UserSettings {
    avatarWidth: number;
    avatarHeight: number;
}

// Model
export interface  UserModel {
    id: string;
    slug: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    password: string;
    cpassword: string;
    npassword: string;
    createdAt?: Date;
    avatar?: string;
    isEnabled: number;
    isFeatured: number;
    userRole: string;
    locationId: number;
    type: number;
    attr_list_values?: any;
    options?: any;
    attr_values?: any;
    location?: any;
    emailConfirmed: boolean;
    company?: any;
    profile?: any;

    fullName?: string;
    actionStatus?: string
    epassword?: string; // current password
}

export const Initial_User_Settings: UserSettings = {
    avatarWidth: 300,
    avatarHeight: 300
}

export const Initial_User_Entity: UserModel = {
    id: '',
    slug: '',
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
    cpassword: '',
    npassword: '',
    isEnabled: 0,
    isFeatured: 0,
    userRole: '',
    locationId: 0,
    type: 0,
    attr_list_values: [],
    options: [],
    location: null,
    emailConfirmed: false,
    company: null,
    profile: null
}

export const API_EndPoints: ApiOptions = {
  root: 'api/user',
  load: 'api/user/load',
  info: 'api/user/getinfo',
  proc: 'api/user/proc',
  action: 'api/user/action',
  update_avatar: 'api/user/update_avatar',
  change_pass: 'api/user/change_pass',
  change_email: 'api/user/change_email',
  change_role: 'api/user/change_role',
};

export interface ApiOptions {
  root: string; // root api (without key - action)
  load: string; // load records
  info: string; // load single record
  proc: string; // save record
  action: string; // apply actions e.g enable, disable
  update_avatar: string; // update user avatar
  change_pass: string; // change user password
  change_email: string; // change user email
  change_role: string; // change user role
}

export interface UserReponse {
    status: string;
    message?: string;
    records?: number;
    record?: UserModel
    posts?: UserModel[]
}
