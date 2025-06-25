import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { NotificationService } from './notification-service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  constructor(private notification: NotificationService) {
    this.startConnection();
  }

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-api-url/notificationhub')
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Error:', err));

    this.hubConnection.on('ReceiveNotification', (type: string, message: string) => {
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
