import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { UserSignUp } from './user.signup';
import { BehaviorSubject, catchError, tap, throwError } from 'rxjs';
import { ErrorService } from '../shared/error.service';
import { UserSignIn } from './user.signIn';
import { AuthResponse } from './auth.response';
import { User } from './user.model';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

const headers = new HttpHeaders({
  'Content-Type': 'application/json',
});

@Injectable({ providedIn: 'root' })
export class AuthService {
  private httpClient = inject(HttpClient);
  // API URL from the environment configuration
  private readonly API_URL = `${environment.apiUrl}`;
  private errorService = inject(ErrorService);
  //user = new Subject<User>();
  user = new BehaviorSubject<User | null>(null);
  private router = inject(Router);
  private tokenExpiration: any = null;
  private userSignal = signal<User | null>(null)

  currentUser = this.userSignal.asReadonly();

  signUp(userSignUp: UserSignUp) {
    return this.httpClient
      .post(`${this.API_URL}/Authentication/create-user`, userSignUp, {
        headers,
      })
      .pipe(
        catchError((error) => {
          return throwError(() => error.error);
        })
      );
  }

  signIn(userSignIn: UserSignIn) {
    return this.httpClient
      .post<AuthResponse>(
        `${this.API_URL}/Authentication/user-signIn`,
        userSignIn,
        {
          headers,
        }
      )
      .pipe(
        catchError((error) => {
          return throwError(() => error.error);
        }),
        tap((responseData) => {
          const decodedToken: any = jwtDecode(responseData.token);
          const userId = decodedToken.Uid;
          const name =
            decodedToken[
              'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
            ];
          const role =
            decodedToken[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ];
          const surename =
            decodedToken[
              'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname'
            ];
          const email = decodedToken.email;
          const user = new User(
            name,
            surename,
            role,
            email,
            userId,
            responseData.token,
            responseData.expiresAt,
            responseData.refreshToken
          );
          this.userSignal.set(user)
          this.user.next(user);
          this.autoLogout(
            new Date(new Date(responseData.expiresAt).toUTCString()).getTime() *
              100000
          );
          localStorage.setItem('userData', JSON.stringify(user));
        })
      );
  }

  autoLogin() {
    const storedUserData = localStorage.getItem('userData');
    const userData: {
      name: string;
      lastName: string;
      role: string;
      email: string;
      id: string;
      _token: string;
      _tokenExpireAt: Date;
      _refreshToken: string;
    } = storedUserData ? JSON.parse(storedUserData) : null;
    if (!userData) {
      return;
    } else {
      const loadedUser = new User(
        userData.name,
        userData.lastName,
        userData.role,
        userData.email,
        userData.id,
        userData._token,
        new Date(userData._tokenExpireAt),
        userData._refreshToken
      );
      if (loadedUser.token) {
        this.userSignal.set(loadedUser);
        this.user.next(loadedUser);
        const expiration =
         new Date(new Date(userData._tokenExpireAt).toUTCString()).getTime() -
          new Date().getDate();
        this.autoLogout(expiration);
      }
    }
  }

  logout() {
    this.user.next(null);
    this.userSignal.set(null)
    this.router.navigate(['/auth']);
    localStorage.removeItem('userData');
    if (this.tokenExpiration) {
      clearTimeout(this.tokenExpiration);
    }
    this.tokenExpiration = null;
  }

  autoLogout(expirationDuration: number) {
    this.tokenExpiration = setTimeout(() => {
      console.log('timer ran: ',expirationDuration)
      this.logout();
    }, expirationDuration);
  }
}
