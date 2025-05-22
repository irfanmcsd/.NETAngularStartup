import { Component, OnInit, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { map, Observable, tap } from 'rxjs';
import { RouterOutlet } from '@angular/router';
import { AppConfig } from './configs/app.configs';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';

// partial components
import { LoaderV2Component } from './sdk/components/loader/loader_v2.component';
import { SharedComponentModule } from './pages/partials/shared.component.module';
import { CoreService } from './sdk/services/coreService';

// State management
import { IAUTH, UserRole, USER_ROLE_ENTITY } from './store_v2/auth/model';
import { ICONFIG } from './store_v2/configs/model';
import {
  selectAuthLoading,
  selectAllAuth,
  authorizeUser,
} from './store_v2/auth/auth.reducer';
import {
  loadConfigs,
  selectAllConfigs,
} from './store_v2/configs/config.reducer';
import { renderSelector } from './store_v2/core/notify/notify.reducers';
import { UserModel, Initial_User_Entity } from './store_v2/user/model';
import { NgIf, AsyncPipe, JsonPipe } from '@angular/common';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    NgIf,
    AsyncPipe,
    JsonPipe,
    TranslatePipe,
    RouterOutlet,
    LoaderV2Component,
    SharedComponentModule,
  ],
  providers: [AppConfig, CoreService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  private store = inject(Store);
  private config = inject(AppConfig);
  private coreService = inject(CoreService);
  public translate = inject(TranslateService);

  constructor() {
    this.translate.setDefaultLang(this.config.culture);
    let lang = localStorage.getItem('app_lang');
    if (lang !== null) {
      this.selectedLanguage = lang
      this.translate.use(lang);
    }
  }

  notifySelector$: Observable<any> = this.store.select(renderSelector);
  authLoadingSelector$: Observable<any> = this.store.select(selectAuthLoading);

  auth$: Observable<IAUTH[]> = this.store.select(selectAllAuth);
  config$: Observable<ICONFIG[]> = this.store.select(selectAllConfigs);
  loading$: Observable<boolean> = this.store.select(selectAuthLoading);

  isAuthorized: boolean = false;
  User: UserModel = Object.assign({}, Initial_User_Entity);
  selectedLanguage: string = 'en'

  // page role
  pageRole: UserRole = USER_ROLE_ENTITY;

  ngOnInit(): void {
    this.auth$ = this.store.select(selectAllAuth).pipe(
      tap((auth: any) => {
        //console.log('Selected auth:', auth);
        if (auth.length > 0) {
          this.isAuthorized = auth[0].isAuthenticated;
          this.User = auth[0].User;
          this.pageRole = this.coreService.processRole(auth[0].Role);

          this.store.dispatch(loadConfigs());
        } else {
          this.isAuthorized = false;
        }
      })
    );
    this.auth$.subscribe();

    /*this.config$ = this.store.select(selectAllConfigs).pipe(
      tap((config: any) => {
        console.log('Config:', config);
        
      })
    );
    this.config$.subscribe();*/

    this.AuthorizeUser();
  }

  AuthorizeUser() {
    this.store.dispatch(
      authorizeUser({ UserID: this.config.getConfig('userid') })
    );
  }
}
