/* -------------------------------------------------------------------------- */
/*                         .NET / Angular Startup App                         */
/*                           Author: Muhammad Irfan                           */
/*                    Email: clouddevarchitect@outlook.com                    */
/*       License: Read license.txt located on root of your application.       */
/* -------------------------------------------------------------------------- */

import { Injectable } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { FormBase } from "../model/base";

/**
 * ControlService - A service for dynamically generating FormGroups
 * from FormBase control configurations.
 * 
 * Features:
 * - Creates reactive form groups from control configurations
 * - Handles various validation requirements
 * - Supports email validation
 * - Manages optional/required fields
 * - Applies min/max length and value constraints
 */
@Injectable()
export class ControlService {

  /**
   * Converts an array of FormBase controls into a FormGroup
   * @param controls Array of FormBase controls
   * @returns Configured FormGroup
   */
  toFormGroup(controls: FormBase<any>[]): FormGroup {
    const group: Record<string, FormControl> = {};

    controls.forEach((control: FormBase<any>) => {
      group[control.key] = this.createFormControl(control);
    });

    return new FormGroup(group);
  }

  /**
   * Creates a FormControl with appropriate validators based on control configuration
   * @param control The FormBase control configuration
   * @returns Configured FormControl
   */
  private createFormControl(control: FormBase<any>): FormControl {
    const validators = this.buildValidators(control);
    return new FormControl(control.value || "", validators);
  }

  /**
   * Builds the validator array based on control configuration
   * @param control The FormBase control configuration
   * @returns Array of validators
   */
  private buildValidators(control: FormBase<any>): any[] {
    const validators = [];

    if (control.required) {
      validators.push(Validators.required);
    }

    if (control.email) {
      validators.push(Validators.email);
    }

    if (control.minLength) {
      validators.push(Validators.minLength(control.minLength));
    }

    if (control.maxLength) {
      validators.push(Validators.maxLength(control.maxLength));
    }

    if (control.min) {
      validators.push(Validators.min(control.min));
    }

    if (control.max) {
      validators.push(Validators.max(control.max));
    }

    if (control.pattern) {
      validators.push(Validators.pattern(control.pattern));
    }

    return validators;
  }
}