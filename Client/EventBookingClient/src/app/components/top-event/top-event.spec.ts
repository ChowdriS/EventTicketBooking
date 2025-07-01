import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopEvent } from './top-event';

describe('TopEvent', () => {
  let component: TopEvent;
  let fixture: ComponentFixture<TopEvent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopEvent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopEvent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
