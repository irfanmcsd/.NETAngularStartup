import { Observable } from "rxjs";
import { API_EndPoints, ConfigReponse } from "./model";
import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AppConfig } from "../../configs/app.configs";

/**
 * ConfigService - Service for managing application configuration
 * 
 * Responsibilities:
 * - Loading application configuration from server
 * - Providing configuration data to components
 * - Handling configuration-related API calls
 * 
 * Dependencies:
 * - HttpClient: For making HTTP requests
 * - AppConfig: For accessing base API URL
 */
@Injectable({ providedIn: 'root' })
export class ConfigService {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly http = inject(HttpClient);
  private readonly appConfig = inject(AppConfig);

  constructor() {}

  /* ---------------------------- PUBLIC METHODS ---------------------------- */

  /**
   * Loads application configuration from server
   * @returns Observable emitting configuration response
   * 
   * Example:
   * this.configService.loadRecords().subscribe(config => {
   *   console.log('Loaded config:', config);
   * });
   */
  loadRecords(): Observable<ConfigReponse> {
    const endpointUrl = this.buildEndpointUrl(API_EndPoints.config);
    return this.http.post<ConfigReponse>(endpointUrl, {});
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */

  /**
   * Constructs full endpoint URL
   * @param endpoint API endpoint path
   * @returns Complete URL string
   */
  private buildEndpointUrl(endpoint: string): string {
    return `${this.appConfig.apiUrl}${endpoint}`;
  }

}