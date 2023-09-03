import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListTracesComponent } from './list-traces.component';

describe('ListTracesComponent', () => {
  let component: ListTracesComponent;
  let fixture: ComponentFixture<ListTracesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListTracesComponent]
    });
    fixture = TestBed.createComponent(ListTracesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
