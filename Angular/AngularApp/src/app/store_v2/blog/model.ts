import {
  DefaultQuery,
  ActionTypes,
  DateFilter,
} from '../../configs/query.types';

/**
 * Blog Types and Interfaces
 * 
 * This file contains all type definitions related to:
 * - Blog query parameters
 * - Blog feature types and grouping options
 * - Blog model and data structure
 * - API endpoint configurations
 * - Blog response formats
 */

/* ---------------------------- BLOG QUERY TYPES ---------------------------- */

/**
 * Extended query parameters for blog operations
 */
export type IBLOGS = DefaultQuery & {
  user_slug: string;        // User identifier for filtering
  isfeatured: FeaturedTypes; // Featured status filter
  groupBy: ChartGroupBy;    // Grouping option for analytics
  loadCategoryList: boolean;
};

/**
 * Enum for featured blog types
 */
export enum FeaturedTypes {
  Basic = 0,    // Standard blog post
  Featured = 1, // Featured content
  Premium = 2,  // Premium/exclusive content
  All = 3       // All types (no filter)
}

/**
 * Enum for chart/data grouping options
 */
export enum ChartGroupBy {
  Hour = 0,      // Group by hour
  Day = 1,       // Group by day
  Month = 2,     // Group by month
  Year = 3,      // Group by year
  Categories = 4,// Group by categories
  None = 5       // No grouping
}

/**
 * Default blog query parameters
 */
export const BLOG_QUERY_OBJECT: IBLOGS = {
  user_slug: '',
  ispublic: false,
  isfeatured: FeaturedTypes.All,
  groupBy: ChartGroupBy.None,
  isapproved: ActionTypes.All,
  isenabled: ActionTypes.All,
  isarchive: ActionTypes.All,
  isdraft: ActionTypes.All,
  dateFilter: DateFilter.All,
  pagenumber: 1,
  pagesize: 21,  // Default page size (3 column layout)
  order: 'blog.id desc',
  loadCategoryList: false
};

/* ---------------------------- BLOG SETTINGS ---------------------------- */

/**
 * Interface for blog media dimensions
 */
export interface BlogSettings {
  thumbWidth: number;   // Thumbnail width in pixels
  thumbHeight: number;  // Thumbnail height in pixels
  bannerWidth: number;  // Banner width in pixels
  bannerHeight: number; // Banner height in pixels
}

export const Initial_Blog_Settings: BlogSettings = {
  thumbWidth: 300,
  thumbHeight: 300,
  bannerWidth: 1000,
  bannerHeight: 250
};

/* ---------------------------- BLOG MODEL ---------------------------- */

/**
 * Main blog post interface
 */
export interface BlogModel {
  id: number;               // Unique identifier
  userid: string;           // Author ID
  term: string;             // Page / Slug
  tags: string;             // Comma-separated tags
  cover?: string;           // Cover image URL
  created_at?: Date;        // Creation timestamp
  updated_at?: Date;        // Last update timestamp
  isenabled: number;        // Enabled status (0/1)
  isapproved: number;       // Approval status (0/1)
  isfeatured: number;       // Featured status (0-2)
  isdraft: number;          // Draft status (0/1)
  isarchive: number;        // Archived status (0/1)
  archive_at?: Date;        // Archival timestamp
  views: number;            // View count
  comments: number;         // Comment count
  favorites: number;        // Favorite count
  total_ratings: number;    // Total rating submissions
  ratings: number;          // Sum of all ratings
  avg_rating: number;       // Calculated average rating
  categorylist?: any;       // Associated categories (array)
  categories: any[];          // Category information
  enc_id?: string;          // Encrypted identifier
  author?: any;             // Author information
  category_list?: any;      // Formatted category list
  blog_data: BlogDataModel; // Localized content
  blog_culture_data: BlogDataModel[] // List of localized contents
  actionstatus?: string;    // Available actions
}

/**
 * Blog content data model (localized)
 */
export interface BlogDataModel {
  id: number;               // Content ID
  blogid: number;           // Parent blog ID
  culture: string;          // Language/culture code
  title: string;            // Blog title
  short_description: string;// Summary/excerpt
  description: string;      // Full content
}

/**
 * Default empty blog state
 */
export const Initial_Blog_Entity: BlogModel = {
  id: 0,
  userid: '',
  term: '',
  tags: '',
  isenabled: 1,
  isapproved: 1,
  isfeatured: 0,
  isdraft: 0,
  isarchive: 0,
  views: 0,
  comments: 0,
  favorites: 0,
  total_ratings: 0,
  ratings: 0,
  avg_rating: 0,
  categorylist: [],
  categories: [],
  author: null,
  category_list: null,
  blog_culture_data: [],
  blog_data: {
    id: 0,
    blogid: 0,
    culture: 'en',
    title: '',
    short_description: '',
    description: ''
  }
};

/* ---------------------------- API CONFIGURATION ---------------------------- */

/**
 * Blog API endpoints
 */
export const API_EndPoints: ApiOptions = {
  root: 'api/blogs',       // Base endpoint
  load: 'api/blogs/load',  // Load multiple records
  info: 'api/blogs/getinfo',  // Get single record
  proc: 'api/blogs/proc',  // Create/update record
  action: 'api/blogs/action' // Special actions
};

/**
 * Interface for API endpoint configuration
 */
export interface ApiOptions {
  root: string;   // Base API path
  load: string;   // List endpoint
  info: string;   // Detail endpoint
  proc: string;   // Save endpoint
  action: string; // Action endpoint
}

/* ---------------------------- RESPONSE TYPES ---------------------------- */

/**
 * Interface for blog API responses
 */
export interface BlogResponse {
  status: string;         // Response status
  message?: string;       // Optional message
  records?: number;       // Total available records
  record?: BlogModel;     // Single blog record
  posts?: BlogModel[];    // Array of blog records
}