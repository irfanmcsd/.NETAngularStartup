import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Bundle Components, Modules & Services
import { MainUserComponent } from './main.component';
import { USER_QUERY_OBJECT } from '../../store_v2/user/model';

const routes: Routes = [
  {
    path: '',
    data: {
      key: "_INDEX_",
      title: 'Manage Users',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'create-account',
    data: {
      key: "_CREATE_",
      title: 'Create Account',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Create Account',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'update/:id',
    data: {
      title: 'Edit User Information',
      key: "_UPDATE_",
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Edit Information',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'profile/:id',
    data: {
       key: "_PROFILE_",
      title: 'Detail Information',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Detail Information',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'change-password/:id',
    data: {
      title: 'Change Password',
      key: "_CHANGE_PASS_",
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Change Password',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'change-email/:id',
    data: {
      title: 'Change Email',
      key: "_CHANGE_EMAIL_",
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Change Email',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'update-role/:id',
    data: {
      title: 'Update User-Role',
      key: "_CHANGE_USER_ROLE_",
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Update User-Role',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'update-avatar/:id',
    data: {
      title: 'Update Avatar',
      key: "_CHANGE_AVATAR_",
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Update User-Role',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  },
  {
    path: 'reports',
    data: {
      title: 'Manage Reports',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Reports',
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  }
];

// add routing for all available query filter properties
for (let prop in USER_QUERY_OBJECT) {
  routes.push({
    path: prop + '/:' + prop,
    data: {
      title: 'Manage Users - Filter By ' + prop,
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Users',
          url: '/users',
        },
        {
          label: 'Filter By: ' + prop,
          url: '',
        },
      ],
    },
    component: MainUserComponent,
  });
}

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),
        MainUserComponent
    ],
    providers: [],
})
export class MainUserModule {}
