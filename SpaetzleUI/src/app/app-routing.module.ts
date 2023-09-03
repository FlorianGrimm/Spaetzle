import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListMessageComponent } from './list-message/list-message.component';
import { ListLogsComponent } from './list-logs/list-logs.component';
import { ListTracesComponent } from './list-traces/list-traces.component';
import { ListMetricsComponent } from './list-metrics/list-metrics.component';

const routes: Routes = [
  {
    path: '',
    pathMatch:'full',
    children:[],
    component: HomeComponent
  },
  {
    path: 'messages',
    component: ListMessageComponent
  },
  {
    path: 'logs',
    component:ListLogsComponent
  },
  {
    path: 'traces',
    component: ListTracesComponent
  },
  {
    path: 'metrics',
    component: ListMetricsComponent
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
