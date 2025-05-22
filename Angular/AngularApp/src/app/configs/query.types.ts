export type KeyValuePair = {
    key: string;
    value: string;
}

// Default Query Types 
export type DefaultQuery = {
    slug?: string;
    slug_started_with?: string;
    id?: number;
    order?: string;
    month?: number;
    year?: number;
    iscache?: boolean;
    pagenumber: number;
    pagesize: number;
    loadall?: boolean;
    ispublic?: boolean;
    categoryid?: number;
    categoryname?: string;
    category_ids?: any;
    range_ids?: any;
    term?: string;
    userid?: string;
    start_date?: Date;
    end_date?: Date;
    dateFilter: DateFilter;
    archive_expiry?: ArchiveExpiryOptions;
    column_options?: FetchColumnOptions;
    isenabled: ActionTypes;
    isapproved: ActionTypes;
    isdraft: ActionTypes;
    isarchive: ActionTypes;
    tags?: string;
    advancefilter?: boolean;
    loadfavorites?: boolean;
    loadanalytics?: boolean;
    loadliked?: boolean;
    render_report?: boolean;
    skip_record_stats?: boolean;
    isadmin?: boolean;
    showall?: boolean;
}

export enum FetchColumnOptions
{
    List = 1,
    Dropdown = 2,
    Profile = 3
}

export enum DateFilter {
    Today = 1,
    ThisWeek = 2,
    ThisMonth = 3,
    ThisYear = 4,
    ThisHour = 5,
    CurrentPrevMonth = 6,
    PrevMonth = 7,
    PrevThreeMonths = 8,
    PrevSixMonths = 9,
    LastSixHour = 10,
    Last12Months = 11,
    Last24Months = 12,
    PrevYear = 13,
    All = 0
}

export enum ArchiveExpiryOptions
{
    Expired = 1,
    ExpireToday = 2,
    Expire_in_5Days = 3,
    All = 4
}

export enum ExpiryOptions {
    Expired = 1,
    ExpireToday = 2,
    Expire_in_5Days = 3,
    Expire_in_10Days = 6,
    Expire_in_Month = 5,
    All = 4
}

export enum FeaturedTypes
{
    Basic = 0,
    Featured = 1,
    Premium = 2,
    All = 3
}

export enum ActionTypes
{
    Disabled = 0,
    Enabled = 1,
    Archive = 3,
    All = 2
}

export enum ChartGroupBy {
    Hour = 0,
    Day = 1,
    Month = 2,
    Year = 3,
    Country = 4,
    Location = 5,
    Categories = 6,
    None = 7
}

export type IListStats = {
    first_boundary: number;
    last_boundary: number;
}


/*export type IPagination = {
    current_page: number;
    total_records: number;
    page_size: number;
    show_first: boolean;
    show_last: boolean;
    pagination_style: number;
    total_links: number;
    prev_css: string;
    next_css: string;
    url_path: ''
}*/
