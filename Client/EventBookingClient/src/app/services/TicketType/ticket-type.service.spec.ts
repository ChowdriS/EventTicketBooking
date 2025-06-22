import { TestBed } from '@angular/core/testing';

import { TicketType } from './ticket-type.service';

describe('TicketType', () => {
  let service: TicketType;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TicketType);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
