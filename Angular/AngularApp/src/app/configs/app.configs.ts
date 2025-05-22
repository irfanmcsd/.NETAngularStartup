import { Injectable } from '@angular/core';

/**
 * Application configuration service that retrieves and provides
 * global configuration values from external sources.
 */
// Define a type for our configuration keys for better type safety
type ConfigKey =
  | 'userid'
  | 'title'
  | 'culture'
  | 'url'
  | 'admin_access'
  | 'xsrfToken';

// Strongly typed interface for the expected window._ang_set structure
interface ExternalConfig {
  title?: string;
  cul?: string; // culture
  u?: string; // url
  id?: string; // userid
  a_access?: boolean; // admin access
  xsrfToken?: string; // xsrf token
}

@Injectable({
  providedIn: 'root', // Makes it tree-shakable and available application-wide
})
export class AppConfig {
  // Default values with proper typing
  private global_vars: Record<ConfigKey, string> = {
    userid: '',
    title: '', // Core application title
    culture: 'en', // Default culture if not provided
    url: '', // Root API URL
    admin_access: 'true', // Admin access as string
    xsrfToken: '',
  };

  constructor() {
    // Safely access window with proper typing
    const externalConfig = (window as { _angConfig?: ExternalConfig })
      ._angConfig;
    if (externalConfig) {
      this.global_vars = {
        userid: decodeURIComponent(externalConfig.id ?? ''),
        title: decodeURIComponent(externalConfig.title ?? ''),
        culture: decodeURIComponent(externalConfig.cul ?? 'en').toLowerCase(), // Fallback to 'en' if not provided
        url: decodeURIComponent(externalConfig.u ?? ''),
        xsrfToken: externalConfig.xsrfToken ?? '',
        admin_access:
          externalConfig.a_access !== undefined
            ? String(externalConfig.a_access)
            : 'true',
      };
    }
  }

  /**
   * Retrieves a configuration value by key
   * @param key The configuration key to retrieve
   * @returns The configuration value or empty string if not found
   * @throws Error if the key is invalid
   */
  public getConfig(key: ConfigKey): string {
    if (!(key in this.global_vars)) {
      throw new Error(`Invalid config key: ${key}`);
    }
    return this.global_vars[key];
  }

  // Convenience getters for better access and autocompletion
  public get userId(): string {
    return this.global_vars.userid;
  }

  public get title(): string {
    return this.global_vars.title;
  }

  public get culture(): string {
    return this.global_vars.culture;
  }

  get apiUrl(): string {
    if (!this.global_vars?.url) {
      console.error('API URL is not configured');
      return ''; // or throw an error depending on your error handling strategy
    }

    // Trim and normalize URL
    const cleanUrl = this.global_vars.url.trim().replace(/\/+$/, ''); // Remove existing trailing slashes

    // Add trailing slash and handle query parameters
    const urlObj = new URL(cleanUrl);

    // Preserve existing path while ensuring trailing slash
    if (!urlObj.pathname.endsWith('/')) {
      urlObj.pathname += '/';
    }

    // Reconstruct URL with proper encoding
    return urlObj.toString();
  }

  public get getXsrfToken(): string {
    return this.global_vars.xsrfToken;
  }

  public get adminAccess(): boolean {
    return this.global_vars.admin_access === 'true';
  }
}
