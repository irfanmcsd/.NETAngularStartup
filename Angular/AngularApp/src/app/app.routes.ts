import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'user-profile',
    loadChildren: () =>
      import('./pages/user-profile/userprofile.module').then((m) => m.MainUserProfileModule),
  },
  {
    path: 'manage-account',
    loadChildren: () =>
      import('./pages/manage-account/manage.module').then((m) => m.MainAccountModule),
  },
  {
    path: 'manage-account',
    loadChildren: () =>
      import('./pages/users/main.module').then((m) => m.MainUserModule),
  },
  {
    path: 'blogs',
    loadChildren: () =>
      import('./pages/blogs/main.module').then((m) => m.MainBlogModule),
  },
  {
    path: 'documentation',
    loadChildren: () =>
      import('./pages/documentation/main.module').then((m) => m.MainDocModule),
  },
 
  {
    path: 'users',
    loadChildren: () =>
      import('./pages/users/main.module').then((m) => m.MainUserModule),
  },
  {
    path: 'categories',
    loadChildren: () =>
      import('./pages/settings/categories/main.module').then(
        (m) => m.MainCategoryModule
      ),
  },
  
  {
    path: 'logs',
    loadChildren: () =>
      import('./pages/settings/errorlogs/main.module').then(
        (m) => m.MainErrorLogModule
      ),
  },
 
  {
    path: 'tags',
    loadChildren: () =>
      import('./pages/settings/tags/main.module').then((m) => m.MainTagModule),
  },

  {
    path: '',
    loadChildren: () =>
      import('./pages/dashboard/main.module').then(
        (m) => m.MainDashboardModule
      ),
  },
  {
    path: '**',
    loadChildren: () =>
      import('./pages/notfound/main.module').then((m) => m.NotFoundModule),
  },
];
