import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
// Bundle Components, Modules & Services
import { ManageAccountComponent } from './manage.account'

const routes: Routes = [
    {
        path: '',
        data: {
            key: "_UPDATE_",
            title: 'Manage Account',
            breadcrumb: []
        },
        component: ManageAccountComponent
    }
]

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        RouterModule.forChild(routes),
        ManageAccountComponent
    ],
    exports: [ManageAccountComponent],
    providers: []
})
export class MainAccountModule {}