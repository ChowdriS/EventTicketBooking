import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventById } from './event-by-id';

describe('EventById', () => {
  let component: EventById;
  let fixture: ComponentFixture<EventById>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventById]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventById);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
