import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { AppConfig } from "../../configs/app.configs";
import { Observable } from "rxjs";
import { API_EndPoints, AuthResponse } from "./model";

/**
 * AuthService - Authentication Service
 * 
 * Responsibilities:
 * - Handles user authentication and authorization
 * - Manages communication with authentication API endpoints
 * - Provides authentication state to application components
 * 
 * Dependencies:
 * - HttpClient: For making HTTP requests to authentication endpoints
 * - AppConfig: For accessing API base URL configuration
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  /* ---------------------------- DEPENDENCY INJECTION ---------------------------- */
  private readonly http = inject(HttpClient);
  private readonly appConfig = inject(AppConfig);

  constructor() {}

  /* ---------------------------- PUBLIC METHODS ---------------------------- */

  /**
   * Authorizes a user with the backend server
   * @param userId The unique identifier of the user
   * @param isAdmin Flag indicating if user should be authenticated as admin
   * @returns Observable emitting authentication response
   * 
   * Example Usage:
   * this.authService.authorize('user123', false).subscribe({
   *   next: (response) => handleAuthentication(response),
   *   error: (error) => handleError(error)
   * });
   */
  authorize(userId: string): Observable<AuthResponse> {
    
    const endpointUrl = this.buildEndpointUrl(API_EndPoints.authenticate);
    const isAdmin: boolean = this.appConfig.adminAccess;
    const requestBody = { 
      userId, 
      isAdmin,
      // Additional parameters can be added here
      // deviceId: this.getDeviceId()
    };

    return this.http.post<AuthResponse>(endpointUrl, requestBody);
  }

  /* ---------------------------- PRIVATE METHODS ---------------------------- */

  /**
   * Constructs complete endpoint URL
   * @param endpoint The API endpoint path
   * @returns Full URL string
   */
  private buildEndpointUrl(endpoint: string): string {
    console.log('api url is ' + this.appConfig.apiUrl)
    return `${this.appConfig.apiUrl}${endpoint}`;
  }

}