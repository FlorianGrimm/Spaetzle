import { Injectable, Inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  hubUrl: string;
  connection: signalR.HubConnection | undefined;

  hubDisplayMessage: BehaviorSubject<string>;;

  constructor(
    @Inject('BASE_URL') baseUrl: string
  ) {
    console.log(`SignalrService constructor: baseUrl: ${baseUrl}`);
    this.hubUrl = `${baseUrl}wsapi`;
    this.hubDisplayMessage = new BehaviorSubject<string>('');
  }

  public async initiateSignalrConnection(): Promise<void> {
    try {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl)
        .withHubProtocol(new MessagePackHubProtocol())
        .withAutomaticReconnect()
        .build();
      await connection.start(); // ({ withCredentials: false });
      this.connection = connection;

      this.wireSignalrClientMethods();

      console.log(`SignalR connection success! connectionId: ${this.connection.connectionId}`);
    }
    catch (error) {
      console.log(`SignalR connection error: ${error}`);
    }
  }

  private wireSignalrClientMethods(): void {
    if (this.connection === undefined) { throw new Error('SignalrService.setSignalrClientMethods: connection is undefined'); }
    this.connection.on('DisplayMessage', (message: string) => {
      this.hubDisplayMessage.next(message);
    });
  }

  displayMessage(message: string){
    if (this.connection === undefined) { throw new Error('SignalrService.displayMessage: connection is undefined'); }
    return this.connection.invoke('DisplayMessage', message);
  }
}
