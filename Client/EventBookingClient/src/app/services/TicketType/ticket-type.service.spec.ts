import { TestBed } from '@angular/core/testing';
import { TicketTypeService } from './ticket-type.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { API_BASE_URL } from '../../misc/constants';
import { ApiResponse } from '../../models/api-response.model';

describe('TicketTypeService', () => {
  let service: TicketTypeService;
  let httpMock: HttpTestingController;

  const mockApiResponse: ApiResponse = {
    $id: '1',
    success: true,
    message: 'OK',
    data: {},
    errors: null
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TicketTypeService]
    });

    service = TestBed.inject(TicketTypeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch ticket types by event ID', () => {
    const eventId = 'abc123';
    const mockTicketTypes = [{ id: '1', name: 'VIP' }];

    service.getByEventId(eventId).subscribe(res => {
      expect(res.length).toBe(1);
      expect(res[0].name).toBe('VIP');
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickettype/event/${eventId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockTicketTypes);
  });

  it('should add a new ticket type', () => {
    const payload = { name: 'Regular', price: 100 };

    service.addTicketType(payload).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickettype`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush(mockApiResponse);
  });

  it('should update a ticket type', () => {
    const ticketTypeId = 'type123';
    const payload = { name: 'Updated' };

    service.updateTicketType(ticketTypeId, payload).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickettype/${ticketTypeId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
    req.flush(mockApiResponse);
  });

  it('should delete a ticket type', () => {
    const ticketTypeId = 'type123';

    service.deleteTicketType(ticketTypeId).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickettype/${ticketTypeId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockApiResponse);
  });
});
