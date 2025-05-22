/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Injectable, inject } from "@angular/core";
import * as Controls from '../../model/elements'
import { FormBase } from "../../model/base";
import { AppConfig } from "../../../../../configs/app.configs";
import { CoreService } from "../../../../services/coreService";



@Injectable()
export class FormService {
  constructor(public config: AppConfig, private coreService: CoreService) {}
 
  getControls(
    categoryOptions: any
  ) {
    const controls: FormBase<any>[] = [];
    
    console.log('category options')
    console.log ( categoryOptions )
    controls.push(
        new Controls.CategorySelector({
          key: 'categorylist_arr',
          label: '',
          required: false,
          value: '',
          options: [],
          categoryOptions: categoryOptions,
          isVisible: true,
          order: 0,
          helpblock: '',
        })
      );

     return controls.sort((a, b) => a.order - b.order);
  }

}
*/