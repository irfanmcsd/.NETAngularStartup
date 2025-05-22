import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
// Bundle Components, Modules & Services
import { UserProfileComponent } from './userprofile.component'

const routes: Routes = [
    {
        path: '',
        data: {
            key: "_UPDATE_",
            title: 'User Profile',
            breadcrumb: []
        },
        component: UserProfileComponent
    }
]

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        RouterModule.forChild(routes),
        UserProfileComponent
    ],
    exports: [UserProfileComponent],
    providers: []
})
export class MainUserProfileModule {}