import { Component, OnInit, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { DisplayMessage, SignalrService } from '../signalr.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-list-message',
  templateUrl: './list-message.component.html',
  styleUrls: ['./list-message.component.less']
})
export class ListMessageComponent implements OnInit, OnDestroy {
  subscription: Subscription = new Subscription();
  listDisplayMessage$: BehaviorSubject<DisplayMessage[]> = new BehaviorSubject<DisplayMessage[]>([]);
  tableDataSourceDisplayMessage = new MatTableDataSource<DisplayMessage>([]);

  constructor(
    public signalrService :SignalrService
  ) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  ngOnInit(): void {
    this.subscription.add(this.signalrService.listDisplayMessage$.subscribe({
      next: (value: DisplayMessage[]) => {
        this.listDisplayMessage$.next(value);
        this.tableDataSourceDisplayMessage.data = value;
      }
    }));
  }

  trackById(index: number, item: DisplayMessage): number {
    return item.id;
  }
}
