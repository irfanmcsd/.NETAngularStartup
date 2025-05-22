import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Bundle Components, Modules & Services
import { MainUsersComponent } from './main.component';

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Manage Logs',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Logs',
          url: '',
        },
      ],
    },
    component: MainUsersComponent,
  },
  {
    path: 'profile/:id',
    data: {
      key: '_PROFILE_',
      title: 'Detail Log',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Logs',
          url: '',
        },
      ],
    },
    component: MainUsersComponent,
  },
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    MainUsersComponent,
  ],
  providers: [],
})
export class MainErrorLogModule {}
