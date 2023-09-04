import { Injectable, Inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
// import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { BehaviorSubject } from 'rxjs';

export type DisplayMessage = {
  id: number;
  msg: string;
}

export type SubscripeStreamRequest = {
  logs: boolean | undefined;
  traces: boolean | undefined;
  metrics: boolean | undefined;
};

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  hubUrl: string;
  connection: signalR.HubConnection | undefined;

  subscripeStreamState : SubscripeStreamRequest = { logs: undefined, traces: undefined, metrics: undefined};

  hubDisplayMessage$: BehaviorSubject<DisplayMessage> = new BehaviorSubject<DisplayMessage>({ id: 0, msg: '' });
  idDisplayMessage: number = 1;
  listDisplayMessage: DisplayMessage[] = [
    { id: 0, msg: 'Hello' },
    { id: 1, msg: 'World' },
    { id: 2, msg: 'Hello' },
    { id: 3, msg: 'World' },
    { id: 4, msg: 'Hello' },
    { id: 5, msg: 'World' },
  ];
  listDisplayMessage$: BehaviorSubject<DisplayMessage[]> = new BehaviorSubject<DisplayMessage[]>(this.listDisplayMessage);

  constructor(
    @Inject('BASE_URL') baseUrl: string
  ) {
    console.log(`SignalrService constructor: baseUrl: ${baseUrl}`);
    this.hubUrl = `${baseUrl}wsapi`;
  }

  public async initiateSignalrConnection(): Promise<void> {
    try {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl)
        //.withHubProtocol(new MessagePackHubProtocol())
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
      const displayMessage: DisplayMessage = { id: ++this.idDisplayMessage, msg: message };
      this.listDisplayMessage.push(displayMessage);
      this.hubDisplayMessage$.next(displayMessage);
      this.listDisplayMessage$.next(this.listDisplayMessage);
    });
  }

  displayMessage(message: string) {
    if (this.connection === undefined) { throw new Error('SignalrService.displayMessage: connection is undefined'); }
    return this.connection.invoke('DisplayMessage', message);
  }

  subscripeStream(value:SubscripeStreamRequest) {
    if (this.connection === undefined) { throw new Error('SignalrService.displayMessage: connection is undefined'); }
    return this.connection.invoke('SubscripeStream', value);
  }
}
