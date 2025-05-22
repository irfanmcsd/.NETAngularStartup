/* -------------------------------------------------------------------------- */
/*                         Enterprise App Builder Toolkit                     */
/*                            Author: Mediasoftpro                            */
/*                       Email: support@mediasoftpro.com                      */
/*       License: Read license.txt located on root of your application.       */
/*                     Copyright 2007 - 2023 @Mediasoftpro                    */
/* -------------------------------------------------------------------------- */

import { Injectable } from '@angular/core';
import { CoreService } from '../../../services/coreService';

@Injectable()
export class SettingsService {

  constructor(private coreService: CoreService) {}

  getButtonLink(list: any[], key: string) {
     for (let item of list) {
        if (item.text === key) {
           return item.url
        }
     }
     return '/'
  }
}
