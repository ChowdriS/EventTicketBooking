import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { Register } from './components/auth/register/register';
import { Home } from './components/User/home/home';
import { LandingPage } from './components/landing-page/landing-page';
import { AuthGuard, UserGuard } from './guard/auth-guard';
import { Events } from './components/User/events/events';
import { FrontPage } from './components/User/front-page/front-page';

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

