import { UserModel, Initial_User_Entity } from '../user/model';

/**
 * Authentication Types and Interfaces
 * 
 * This file contains all type definitions related to:
 * - Authentication state
 * - User roles and permissions
 * - API endpoint configurations
 * - Authentication responses
 */

/* ---------------------------- AUTHENTICATION TYPES ---------------------------- */

/**
 * Interface representing authentication state
 */
export interface IAUTH {
  id: number;                   // Unique identifier
  isAuthenticated: boolean;     // Authentication status flag
  User?: UserModel;             // Authenticated user details
  Role?: string[];              // User roles/permissions
  Token?: string;               // Authentication token
}

/**
 * Initial authentication state
 */
export const AUTH_INITIAL_STATE: IAUTH = {
  id: 0,
  isAuthenticated: false,
  User: Initial_User_Entity,
  Token: '',
};

/* ---------------------------- USER QUERY TYPES ---------------------------- */

/**
 * Interface for user query parameters
 */
export interface IUSERQUERY {
  id: string;                   // User ID
  slug?: string;                // User slug/username
  email: string;                // User email
  firstName: string;            // User first name
  create_at?: Date;             // Account creation date
  avatar?: string;              // Profile image URL
  isenabled: number;            // Account status (0/1)
  user_role?: string;           // User role
  type: number;                 // User type
  locationid: number;           // Associated location ID
  profile?: any;                // Additional profile data
};

/* ---------------------------- ROLE DEFINITIONS ---------------------------- */

/**
 * Interface representing user role flags
 */
export interface UserRole {
  admin: boolean;               // Standard admin privileges
  super_admin: boolean;         // Full system access
  readonly_admin: boolean;      // Read-only admin access
  employer: boolean;            // Employer account
  agent: boolean;               // Agent account
  user: boolean;                // Regular user
}

/**
 * Default (empty) role state
 */
export const USER_ROLE_ENTITY: UserRole = {
  admin: false,
  super_admin: false,
  readonly_admin: false,
  employer: false,
  agent: false,
  user: false,
};

/* ---------------------------- API CONFIGURATION ---------------------------- */

/**
 * Authentication API endpoints
 */
export const API_EndPoints: ApiOptions = {
  authenticate: 'api/auth/authenticate',  // Primary authentication endpoint
};

/**
 * Interface for API endpoint configuration
 */
export interface ApiOptions {
  authenticate: string;  // Authentication endpoint path
}

/* ---------------------------- RESPONSE TYPES ---------------------------- */

/**
 * Interface for authentication API responses
 */
export interface AuthResponse {
  status: string;       // Response status ('success'/'error')
  message?: string;     // Optional status message
  token?: string;       // JWT token on successful auth
  post?: UserModel;     // Authenticated user data
  role?: string[];      // Assigned user roles
}