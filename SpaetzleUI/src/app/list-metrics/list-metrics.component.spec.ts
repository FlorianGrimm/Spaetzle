import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListMetricsComponent } from './list-metrics.component';

describe('ListMetricsComponent', () => {
  let component: ListMetricsComponent;
  let fixture: ComponentFixture<ListMetricsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListMetricsComponent]
    });
    fixture = TestBed.createComponent(ListMetricsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
