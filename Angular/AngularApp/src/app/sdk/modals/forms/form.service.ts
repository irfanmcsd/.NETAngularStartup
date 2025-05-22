/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */
/*
import { Injectable, inject } from "@angular/core";
import * as Controls from "../../components/reactiveform/model/elements";
import { FormBase } from "../../components/reactiveform/model/base";
import { AppConfig } from "../../../configs/app.configs";
import { CoreService } from "../../services/coreService"

@Injectable()
export class FormService {
  constructor(public config: AppConfig, private coreService: CoreService) {}
  
  getControls(
    entity: any,
    isedit: false
  ) {
    const controls: FormBase<any>[] = [];
  
    // dynamic form
    //this.coreService.renderDynamicControls(controls, entity, isedit);

    return controls.sort((a, b) => a.order - b.order);
  }

}
*/