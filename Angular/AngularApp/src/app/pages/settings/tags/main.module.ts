import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Bundle Components, Modules & Services
import { MainTagsComponent } from './main.component'

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'Manage Tags',
            breadcrumb: [
               {
                   label: 'Dashboard',
                   url: '/'
               },
               {
                   label: 'Tags',
                   url: ''
               },
            ]
        },
        component: MainTagsComponent
    }
]


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),
        MainTagsComponent
    ],
    providers: []
})
export class MainTagModule {}