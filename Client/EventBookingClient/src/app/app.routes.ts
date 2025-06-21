import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { Home } from './User/home/home';
import { LandingPage } from './landing-page/landing-page';
import { AuthGuard, UserGuard } from './guard/auth-guard';
import { Events } from './User/events/events';
import { FrontPage } from './User/front-page/front-page';

export const routes: Routes = [
  { path: 'default', component: LandingPage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '', redirectTo: 'default', pathMatch: 'full' },
  {
    path: 'user',
    component: Home,
    canActivate: [AuthGuard,UserGuard],
    children: [
      {
        path: '',
        component: FrontPage,
      },
      {
        path: 'events',
        component: Events
      }
    ]
  }
];

