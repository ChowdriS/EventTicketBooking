import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimilarEvents } from './similar-events';

describe('SimilarEvents', () => {
  let component: SimilarEvents;
  let fixture: ComponentFixture<SimilarEvents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimilarEvents]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SimilarEvents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
