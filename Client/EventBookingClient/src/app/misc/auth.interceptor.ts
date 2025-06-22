import { HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { Auth } from "../services/Auth/auth";
import { inject } from "@angular/core";

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    const authToken = inject(Auth).getToken();
    const newReq = req.clone({
        setHeaders: {
            Authorization: `Bearer ${authToken!}`,
        },
    });
    return next(newReq);
}