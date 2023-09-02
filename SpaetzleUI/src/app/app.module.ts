import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SignalrService } from './signalr.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  providers: [
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
    SignalrService,
    {
      provide: APP_INITIALIZER,
      useFactory: (signalrService: SignalrService) => () => signalrService.initiateSignalrConnection(),
      deps: [SignalrService],
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

