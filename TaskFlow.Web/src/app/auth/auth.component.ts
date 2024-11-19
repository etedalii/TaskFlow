import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from './auth.service';
import { UserSignUp } from './user.signup';
import { LoadingSpinnerComponent } from "../shared/loading-spinner/loading-spinner.component";
import { UserSignIn } from './user.signIn';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [FormsModule, LoadingSpinnerComponent],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css',
})
export class AuthComponent {
  isLoginMode = signal(true);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  isLoading = signal(false);
  errorMessage = signal('')
  private router = inject(Router)

  onSwtichMode() {
    this.isLoginMode.set(!this.isLoginMode());
  }

  onSubmit(form: NgForm) {
    if (!form.valid) {
      console.log('Form is invalid!');
      return;
    }
    this.isLoading.set(true)
    this.errorMessage.set('')
    let subscribe: any;
    if (!this.isLoginMode()) {
      let userSignUp: UserSignUp = {
        firstName: form.value.firstName,
        lastName: form.value.lastName,
        userName: form.value.email,
        emailAddress: form.value.email,
        password: form.value.password,
        role: 'Client',
      };

       subscribe = this.authService.signUp(userSignUp).subscribe({
        error: (error) => {
          console.log(error.error);
          this.isLoading.set(false)
        },
        next: (data) => {
          this.isLoading.set(false)
          alert(
            `Message from server: ${(data as { data: { data: string } }).data}`
          );
        },
      });
    } 
    else{
      let usersignIn: UserSignIn = {
        emailAddress: form.value.email,
        password: form.value.password,
      };

      subscribe = this.authService.signIn(usersignIn).subscribe({
        error: (error) => {
          this.errorMessage.set(error)
          console.log(error);
          this.isLoading.set(false)
        },
        next: (data) => {
          this.isLoading.set(false)
           this.router.navigate([''])
        },
      })
    }

    this.destroyRef.onDestroy(() => {
      subscribe.unsubscribe();
    });

    form.reset();
  }
}
