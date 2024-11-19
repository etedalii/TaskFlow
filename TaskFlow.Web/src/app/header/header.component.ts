import { Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  isAuthenticated = signal(false);

  ngOnInit() {
    const userUnsub = this.authService.user.subscribe((user) => {
      this.isAuthenticated.set(!!user);
    });

    this.destroyRef.onDestroy(() => {
      userUnsub.unsubscribe();
    });
  }

  onLogOut() {
    this.isAuthenticated.set(false);
    this.authService.logout();
  }
}
