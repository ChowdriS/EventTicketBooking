import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EventService } from './event.service';
import { API_BASE_URL } from '../../misc/constants';

describe('EventService', () => {
  let service: EventService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EventService]
    });
    service = TestBed.inject(EventService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get events', () => {
    service.getEvents().subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/?pageNumber=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
  });

  it('should get event by ID', () => {
    const eventId = '123';
    service.getEventById(eventId).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/${eventId}`);
    expect(req.request.method).toBe('GET');
  });

  it('should get manager events', () => {
    service.getManagerEvents().subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/myevents?pageNumber=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
  });

  it('should add an event', () => {
    const payload = { name: 'Sample Event' };
    service.addEvent(payload).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
  });

  it('should update an event', () => {
    const id = 'event123';
    const payload = { name: 'Updated Event' };
    service.updateEvent(id, payload).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/${id}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
  });

  it('should delete an event', () => {
    const id = 'event123';
    service.deleteEvent(id).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/${id}`);
    expect(req.request.method).toBe('DELETE');
  });

  it('should get all event images', () => {
    service.getAllEventImages().subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/eventimage/getall`);
    expect(req.request.method).toBe('GET');
  });

  it('should get my event images', () => {
    service.getMyEventImages().subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/eventimage/myevent/getall`);
    expect(req.request.method).toBe('GET');
  });

  it('should delete an event image', () => {
    const imageId = 'img123';
    service.deleteEventImages(imageId).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/eventimage/delete/${imageId}`);
    expect(req.request.method).toBe('DELETE');
  });

  it('should filter events with parameters', () => {
    service.getFilteredEvents(1, '2', 'music', '2025-12-01', 1, 10).subscribe();
    const expectedUrl = `${API_BASE_URL}/events/filter?category=1&cityId=2&searchElement=music&date=2025-12-01&pageNumber=1&pageSize=10`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
  });

  it('should upload event image', () => {
    const id = 'event123';
    const payload = new FormData();
    service.uploadEventImage(id, payload).subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/eventimage/upload/${id}`);
    expect(req.request.method).toBe('POST');
  });

  it('should get cities', () => {
    service.getCities().subscribe();
    const req = httpMock.expectOne(`${API_BASE_URL}/events/cities`);
    expect(req.request.method).toBe('GET');
  });
});
