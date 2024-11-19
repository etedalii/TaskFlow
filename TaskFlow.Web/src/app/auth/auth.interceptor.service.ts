import { HttpEvent, HttpHandler, HttpHandlerFn, HttpInterceptor, HttpParams, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable, switchMap, take } from "rxjs";
import { AuthService } from "./auth.service";


// Function-based interceptor
export function authInterceptor(request: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> {
    // Inject AuthService to get the user
    const authService = inject(AuthService);
  
    return authService.user.pipe(
      take(1),  // Take the first emitted user value
      switchMap((user) => {
        if (user && user.token) {
          // Clone the request and add the Authorization header with the Bearer token
          const modifiedRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`,
            },
          });
          // Proceed with the modified request
          return next(modifiedRequest);
        }
        // If no user or token, just pass the original request
        return next(request);
      })
    );
  }

// @Injectable({providedIn: 'root'})
// export class AuthInterceptorService implements HttpInterceptor {
//     private authService = inject(AuthService)

//     intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         return this.authService.user.pipe(
//             take(1), // Take the first emitted user value
//             switchMap((user) => {
//                 const modifiedRequest = req.clone({
//                     setHeaders: {
//                       Authorization: `Bearer ${user!.token}`
//                     }
//                   });
//                 return next.handle(modifiedRequest)
//             }))
//     }
// }