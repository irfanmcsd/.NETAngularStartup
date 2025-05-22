// src/app/todo.service.ts
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { delay, Observable, of } from 'rxjs';
import { Notify } from './notify.reducers';

@Injectable({ providedIn: 'root' })
export class NotifyService {

  constructor(private toasterService: ToastrService) {}

  renderNotification(notify: Notify): Observable<boolean> {

    const params = {
      closeButton: true,
    };

    if (notify.css == 'bg-success') {
      this.toasterService.success(notify.title, notify.text, params);
    } else if (notify.css === 'bg-error' || notify.css === 'bg-danger') {
      this.toasterService.error(notify.title, notify.text, params);
    } else {
      this.toasterService.info(notify.title, notify.text, params);
    }
    return of(true);
  }
}
