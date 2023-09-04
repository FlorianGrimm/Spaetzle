import { Component, OnInit, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { DisplayMessage, SignalrService, SubscripeStreamRequest } from '../signalr.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-list-logs',
  templateUrl: './list-logs.component.html',
  styleUrls: ['./list-logs.component.less']
})
export class ListLogsComponent implements OnInit, OnDestroy {
  subscription: Subscription = new Subscription();
  constructor(
    public signalrService: SignalrService
  ) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  ngOnInit(): void {
    this.signalrService.displayMessage("logs");
    const value:SubscripeStreamRequest=
    {
      ...this.signalrService.subscripeStreamState,
      logs: true
    };
    this.signalrService.subscripeStream(value);
  }
}
