import { inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { AppConfig } from '../configs/app.configs'

@Injectable()
export class XsrfInterceptor implements HttpInterceptor {

     private config = inject(AppConfig);
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        
        const token = this.config.getXsrfToken;
        
        if (token && !req.headers.has('X-XSRF-TOKEN')) {
            req = req.clone({
                headers: req.headers.set('X-XSRF-TOKEN', token)
            });
        }
        
        return next.handle(req);
    }
}