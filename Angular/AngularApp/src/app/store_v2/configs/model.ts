/**
 * Application Configuration Types
 * ==============================
 * 
 * This file defines all type interfaces for application configuration settings.
 * It serves as a central repository for type definitions related to:
 * - Global application settings
 * - Module-specific configurations
 * - API endpoint definitions
 * - Status types and options
 */

import { KeyValuePair } from '../../configs/query.types';
import { CategorySettings } from '../../store_v2/category/model';
import { BlogSettings } from '../../store_v2/blog/model';
import { UserSettings } from '../../store_v2/user/model';

/* ---------------------------- CORE CONFIGURATION TYPES ---------------------------- */

/**
 * Main configuration interface containing all application settings
 */
export interface ICONFIG {
    id: number;
    configs: ConfigReponse;          // Complete configuration response
    // config_types: ConfigurationsTypes; // Typed configuration options
    isloaded: boolean;               // Loading status flag
}

/**
 * API endpoint configuration
 */
export interface ApiOptions {
    config: string;  // Configuration API endpoint
}

export const API_EndPoints: ApiOptions = {
    config: 'api/app/init'  // Default configuration endpoint
};

/* ---------------------------- SETTINGS TYPE DEFINITIONS ---------------------------- */

/**
 * Application-wide settings
 */
export interface IConfigSettings {
    general: GeneralSettings;    // General application settings
    media: MediaSettings;        // Media handling settings
    category: CategorySettings;  // Category management settings
    blog: BlogSettings;          // Blog module settings
    user: UserSettings;          // User management settings
    keys: KeySettings;           // API keys and secrets
}

/**
 * General application settings
 */
export interface GeneralSettings {
    pageSize: number;  // Default page size for listings
    currency: string;  // Default currency code
}

/**
 * Media handling settings
 */
export interface MediaSettings {
    photoExtensions: string;  // Allowed photo extensions
    photoSize: string;        // Maximum photo size
}

/**
 * API key settings
 */
export interface KeySettings {
    googleMapApiKey: string;  // Google Maps API key
    stripeSiteKey: string;    // Stripe payment key
}

/* ---------------------------- MODULE-SPECIFIC CONFIG TYPES ---------------------------- */

/**
 * General module configuration options
 */
export interface IConfigGeneral {
    featured: KeyValuePair[];         // Featured items configuration
    expiry_options: KeyValuePair[];   // Expiry period options
    datefilter: KeyValuePair[];       // Date filter options
    groupby: KeyValuePair[];          // Grouping options
}

/**
 * Property module configuration
 */
export interface IConfigProperties {
    groupby: KeyValuePair[];         // Property grouping options
    featured: KeyValuePair[];        // Featured property settings
    adtypes: KeyValuePair[];         // Advertisement types
    wantefor: KeyValuePair[];        // "Wanted for" options
    itemtypes: KeyValuePair[];       // Item type classifications
    adstatus: KeyValuePair[];        // Advertisement statuses
    rentpaytypes: KeyValuePair[];    // Rent payment types
    offersetatus: KeyValuePair[];    // Offer status types
}

/**
 * Abuse reporting configuration
 */
export interface IConfigAbuse {
    contenttypes: KeyValuePair[];    // Content types for reporting
    status: KeyValuePair[];          // Report status options
    actiontypes: KeyValuePair[];     // Available actions
    groupby: KeyValuePair[];         // Grouping options
}

/**
 * Blog module configuration
 */
export interface IConfigBlogs {
    groupby: KeyValuePair[];  // Blog grouping options
}

/**
 * Category management configuration
 */
export interface IConfigCategories {
    types: KeyValuePair[];  // Category types
}

/**
 * Company management configuration
 */
export interface IConfigCompanies {
    types: KeyValuePair[];    // Company types
    groupby: KeyValuePair[];  // Grouping options
}

/**
 * Attribute configuration
 */
export interface IConfigAttrs {
    types: KeyValuePair[];  // Attribute types
}
/*
export interface IConfigAttrsTypes {
    types: StatusTypes[];
}
*/

/**
 * Payment system configuration
 */
export interface IConfigPayments {
    credit_types: KeyValuePair[];           // Credit types
    subscription_types: KeyValuePair[];     // Subscription options
    package_types: KeyValuePair[];          // Package types
    target_types: KeyValuePair[];           // Payment targets
    plan_types: KeyValuePair[];             // Billing plans
    adduration_types: KeyValuePair[];       // Ad duration options
    order_status: KeyValuePair[];           // Order statuses
    order_placement: KeyValuePair[];        // Order placement types
    group_by: KeyValuePair[];               // Grouping options
}

/**
 * Review system configuration
 */
export interface IConfigReviews {
    types: KeyValuePair[];  // Review types
}

/**
 * Role management configuration
 */
export interface IConfigRoles {
    types: KeyValuePair[];  // Role types
}

/**
 * Tag management configuration
 */
export interface IConfigTags {
    types: KeyValuePair[];       // Tag types
    tag_types: KeyValuePair[];   // Tag category types
    tag_levels: KeyValuePair[];  // Tag hierarchy levels
}

/**
 * User management configuration
 */
export interface IConfigUsers {
    types: KeyValuePair[];  // User types
}

/**
 * Processed settings container
 */
export interface IConfigProcessedSettings {
    general: any;
    property: any;
    media: any;
    category: any;
    location: any;
    blog: any;
    company: any;
    user: any;
    keys: any;
}

/**
 * Complete configuration response from API
 */
export interface ConfigReponse {
    status: string;             // Response status
    message: string;            // Status message
    settings: IConfigSettings;  // Application settings
    cultures: KeyValuePair[];         // Supported cultures
    general: IConfigGeneral;    // General configurations
    property: IConfigProperties; // Property configurations
    abuse: IConfigAbuse;        // Abuse system config
    blog: IConfigBlogs;         // Blog configurations
    category: IConfigCategories; // Category config
    company: IConfigCompanies;  // Company configurations
    attr: IConfigAttrs;         // Attribute config
    payment: IConfigPayments;   // Payment config
    review: IConfigReviews;     // Review system config
    role: IConfigRoles;         // Role config
    tag: IConfigTags;           // Tag config
    user: IConfigUsers;         // User config
}