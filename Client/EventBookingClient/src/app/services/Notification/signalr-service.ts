import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { NotificationService } from './notification-service';
import { Auth } from '../Auth/auth';
import { Getrole, GetUserID } from '../../misc/Token';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  constructor(private notification: NotificationService, private authService:Auth) {
    // this.startConnection();
  }
  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('SignalR connection stopped.'))
        .catch(err => console.error('Error stopping SignalR connection:', err));
    }
  }

  startConnection() {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      console.log('SignalR connection already active.');
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5279/notificationHub")
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started.');
        const token = this.authService.getToken();
        const currentUserId = GetUserID(token);
        const role = Getrole(token);

        if (role === 'User') {
          this.hubConnection.invoke("JoinGroup", `user_${currentUserId}`);
        } else if (role === 'Manager') {
          this.hubConnection.invoke("JoinGroup", `manager_${currentUserId}`);
        } else if (role === 'Admin') {
          this.hubConnection.invoke("JoinGroup", 'admins');
        }
      })
      .catch(err => console.error('SignalR start error:', err));

    this.hubConnection.on('ReceiveNotification', (type: string, message: string) => {
      console.log(type," ",message);
      this.handleNotification(type, message);
    });
  }


  private handleNotification(type: string, message: string) {
    switch (type) {
      case 'info':
        this.notification.info(message);
        break;
      case 'success':
        this.notification.success(message);
        break;
      case 'error':
        this.notification.error(message);
        break;
      default:
        this.notification.info(message); 
    }
  }
}
