import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Bundle Components, Modules & Services
import { MainBlogsComponent } from './main.component';
import { BLOG_QUERY_OBJECT } from '../../store_v2/blog/model';

const ROUTES: Routes = [
  {
    path: '',
    data: {
      key: "_INDEX_",
      title: 'Manage Blogs',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Blogs',
          url: '',
        },
      ],
    },
    component: MainBlogsComponent,
  },
  {
    path: 'create-post',
    data: {
      key: "_CREATE_",
      title: 'Create Post',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Blogs',
          url: '',
        },
      ],
    },
    component: MainBlogsComponent,
  },
  {
    path: 'update-post/:id',
    data: {
      key: "_UPDATE_",
      title: 'Update Post',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Blogs',
          url: '',
        },
      ],
    },
    component: MainBlogsComponent,
  },
  {
    path: 'profile/:id',
    data: {
      key: "_PROFILE_",
      title: 'Detail Profile',
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Blogs',
          url: '',
        },
      ],
    },
    component: MainBlogsComponent,
  },
  
];
// add routing for all available query filter properties
for (let prop in BLOG_QUERY_OBJECT) {
  ROUTES.push({
    path: prop + '/:' + prop,
    data: {
      title: 'Manage Blogs - Filter By ' + prop,
      breadcrumb: [
        {
          label: 'Dashboard',
          url: '/',
        },
        {
          label: 'Blogs',
          url: '/blogs',
        },
        {
          label: 'Filter By: ' + prop,
          url: '',
        },
      ],
    },
    component: MainBlogsComponent,
  });
}

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(ROUTES),
        MainBlogsComponent,
    ],
    providers: [],
})
export class MainBlogModule {}
