import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MemberComponent } from './member.component';
import { LoginComponent } from './pages/login/login.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { WishlistComponent } from './pages/wishlist/wishlist.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { VerifyEmailComponent } from './pages/verify-email/verify-email.component';
import { NotificationsComponent } from './pages/notifications/notifications.component';

const routes: Routes = [
  {
    path: '',
    component: MemberComponent,
    children: [
      { path: 'login', component: LoginComponent },

      {
        path: 'profile',
        component: ProfileComponent,
        canActivate: [AuthGuard]
      },

      {
        path: 'wishlist',
        component: WishlistComponent,
        canActivate: [AuthGuard]
      },

      {
        path: 'notifications',
        component: NotificationsComponent,
        canActivate: [AuthGuard]
      },

      {
        path: 'cart',
        redirectTo: '/mall/cart',
        pathMatch: 'full'
      },


      { path: 'verify-email', component: VerifyEmailComponent },

      { path: 'verify-email/:token', component: VerifyEmailComponent },

      { path: 'forgot-password', component: ForgotPasswordComponent },

      { path: '', component: ProfileComponent },

    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MemberRoutingModule { }
