import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { Register } from './components/auth/register/register';
import { Home as UserHome} from './components/User/home/home';
import { Home as ManagerHome} from './components/Manager/home/home';
import { LandingPage } from './components/landing-page/landing-page';
import { AuthGuard, ManagerGuard, UserGuard } from './guard/auth-guard';
import { Events as UserEvents} from './components/User/events/events';
import { Events as ManagerEvents} from './components/Manager/events/events';
import { FrontPage as UserFrontPage } from './components/User/front-page/front-page';
import { FrontPage as ManagerFrontPage } from './components/Manager/front-page/front-page';
import { EventById as UserEventById } from './components/User/event-by-id/event-by-id';
import { EventsById as ManagerEventById } from './components/Manager/events-by-id/events-by-id';
import { Profile as UserProfile} from './components/User/profile/profile';
import { Profile as ManagerProfile} from './components/Manager/profile/profile';
import { Tickets as ManagerTickets} from './components/Manager/tickets/tickets';
import { AddEvent as ManagerAddEvent} from './components/Manager/add-event/add-event';

export const routes: Routes = [
  { path: 'default', component: LandingPage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '', redirectTo: 'default', pathMatch: 'full' },
  {
    path: 'user',
    component: UserHome,
    canActivate: [AuthGuard,UserGuard],
    children: [
      {
        path: '',
        component: UserFrontPage,
      },
      {
        path: 'events',
        component: UserEvents
      },
      {
        path: 'events/:id',
        component: UserEventById,
      },
      {
        path: 'profile',
        component: UserProfile
      }
    ]
  },
  {
    path: 'manager',
    component: ManagerHome,
    canActivate: [AuthGuard,ManagerGuard],
    children: [
      {
        path: '',
        component: ManagerFrontPage,
      },
      {
        path: 'events',
        component: ManagerEvents
      },
      {
        path: 'events/add',
        component: ManagerAddEvent
      },
      {
        path: 'events/:id',
        component: ManagerEventById,
      },
      {
        path: 'events/:id/tickets',
        component: ManagerTickets,
      },
      {
        path: 'profile',
        component: ManagerProfile
      }
    ]
  }
];

