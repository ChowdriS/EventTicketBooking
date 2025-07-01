import { TestBed } from '@angular/core/testing';
import { TicketService } from './ticket.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { API_BASE_URL } from '../../misc/constants';
import { HttpErrorResponse } from '@angular/common/http';

describe('TicketService', () => {
  let service: TicketService;
  let httpMock: HttpTestingController;

  const mockResponse = {
    success: true,
    message: 'OK',
    data: {},
    $id: '1',
    errors: null
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TicketService]
    });
    service = TestBed.inject(TicketService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should book a ticket', () => {
    const payload = { eventId: '123', quantity: 2 };
    service.bookTicket(payload).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickets/book`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush(mockResponse);
  });

  it('should get my tickets', () => {
    service.getMyTickets(1, 5).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickets/mine?pageNumber=1&pageSize=5`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get tickets by event ID', () => {
    const eventId = 'abc123';
    service.getTicketsByEventId(eventId, 1, 10).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickets/event/${eventId}?pageNumber=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should cancel a ticket', () => {
    const ticketId = 't-001';
    service.cancelTicket(ticketId).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickets/${ticketId}/cancel`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });

  it('should export a ticket as blob', () => {
    const ticketId = 'ticket-123';
    const blobData = new Blob(['PDF content'], { type: 'application/pdf' });

    service.exportTicket(ticketId).subscribe(blob => {
      expect(blob).toEqual(blobData);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/tickets/${ticketId}/export`);
    expect(req.request.method).toBe('GET');
    expect(req.request.responseType).toBe('blob');
    req.flush(blobData);
  });

});
