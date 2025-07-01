import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AnalyticsService } from './analytics.service';
import { API_BASE_URL } from '../../misc/constants';

describe('AnalyticsService', () => {
  let service: AnalyticsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AnalyticsService]
    });

    service = TestBed.inject(AnalyticsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch my earnings', () => {
    const dummyResponse = { totalEarnings: 1000 };

    service.getMyEarning().subscribe(data => {
      expect(data).toEqual(dummyResponse);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/analytics/my-earnings`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyResponse);
  });
});
