/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Layout Components
import { SideBarComponent } from './sidebar/component';
import { HeaderComponent } from './header/component';
//import { BreadCrumbComponent } from './breadcrumb/component';
import { FooterComponent } from './footer/component';

@NgModule({
    imports: [
        CommonModule,
        SideBarComponent,
        HeaderComponent,
        //BreadCrumbComponent,
        FooterComponent
    ],
    exports: [
        SideBarComponent,
        HeaderComponent,
        //BreadCrumbComponent,
        FooterComponent
    ],
    providers: []
})
export class SharedComponentModule {}
