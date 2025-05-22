import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Bundle Components, Modules & Services
import { MainComponent } from './main.component'

const routes: Routes = [
    {
        path: '',
        component: MainComponent
    }
]


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),
        MainComponent
    ],
    providers: []
})
export class NotFoundModule {}