import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventsById } from './events-by-id';

describe('EventsById', () => {
  let component: EventsById;
  let fixture: ComponentFixture<EventsById>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventsById]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventsById);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
