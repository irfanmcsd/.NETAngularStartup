import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import {
  provideHttpClient,
  withInterceptorsFromDi,
  withInterceptors,
  HttpClient,
} from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateService, TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { environment } from './environments/environment';

import { errorInterceptor } from './interceptors/error.interceptor';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideRouter } from '@angular/router';
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { XsrfInterceptor } from './interceptors/xsrf.interceptop'
import { routes } from './app.routes';
import { provideEnvironmentNgxMask } from 'ngx-mask';

// Effects
import { AuthEffects } from './store_v2/auth/auth.effect';
import { ConfigEffects } from './store_v2/configs/config.effect';
import { NotifyEffects } from './store_v2/core/notify/notify.effects';
import { BlogEffects } from './store_v2/blog/blog.effect';
import { CategoryEffects } from './store_v2/category/category.effect';
import { ErrorLogEffects } from './store_v2/errorlog/log.effect';
import { TagEffects } from './store_v2/tags/tags.effect';
import { UserEffects } from './store_v2/user/user.effect';
// Reducers
import { authReducer } from './store_v2/auth/auth.reducer';
import { configReducer } from './store_v2/configs/config.reducer';
import { notifyReducer } from './store_v2/core/notify/notify.reducers';
import { eventReducer } from './store_v2/core/event/event.reducers';
import { blogReducer } from './store_v2/blog/blog.reducer';
import { categoryReducer } from './store_v2/category/category.reducer';
import { errorLogReducer } from './store_v2/errorlog/log.reducer';
import { tagReducer } from './store_v2/tags/tags.reducer';
import { userReducer } from './store_v2/user/user.reducer';

// Root Providers
import { AppConfig } from './configs/app.configs';
import { CoreService } from './sdk/services/coreService';


// Ngx Traslator
export const provideTranslation = () => ({
  defaultLanguage: environment.defaultLanguage,
  loader: {
    provide: TranslateLoader,
    useFactory: HttpLoaderFactory,
    deps: [HttpClient],
  },
});
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, environment.i18nPath, '.json');
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom([TranslateModule.forRoot(provideTranslation())]),
    provideHttpClient(
      withInterceptors([errorInterceptor]),
      withInterceptorsFromDi()
    ),
    provideEffects([
      NotifyEffects,
      ConfigEffects,
      AuthEffects,
      BlogEffects,
      CategoryEffects,
      ErrorLogEffects,
      TagEffects,
      UserEffects,
    ]),
    provideStore({
      auth: authReducer,
      notify: notifyReducer,
      event: eventReducer,
      config: configReducer,
      blog: blogReducer,
      category: categoryReducer,
      errorlog: errorLogReducer,
      tag: tagReducer,
      user: userReducer,
    }),
    AppConfig,
    CoreService,
    TranslateService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: XsrfInterceptor, multi: true },
    provideEnvironmentNgxMask(),
    provideAnimations(),
    provideToastr(),
   
  ],
};
