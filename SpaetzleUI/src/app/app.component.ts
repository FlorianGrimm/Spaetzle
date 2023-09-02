import { Component, OnInit, OnDestroy } from '@angular/core';
import { SignalrService } from './signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'SpaetzleUI';
  subscription: Subscription = new Subscription;

  displayMessage: string = '';

  constructor(
    public signalrService: SignalrService
  ) {

  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  ngOnInit(): void {
    this.signalrService.hubDisplayMessage.subscribe({
      next: (message: string) => {
        console.log(`AppComponent.ngOnInit: message: ${message}`);
        this.displayMessage = message;
      },
      error: (error: any) => {

      }
    });

    this.signalrService.displayMessage("Hello").catch((error: any) => {
      console.log(`AppComponent.ngOnInit: error: ${error}`);
    });
  }
}
