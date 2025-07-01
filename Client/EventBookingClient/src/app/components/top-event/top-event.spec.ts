// import { ComponentFixture, TestBed } from '@angular/core/testing';
// import { TopEvent } from './top-event';
// import { Router } from '@angular/router';
// import { Auth } from '../../services/Auth/auth';
// import { AppEvent } from '../../models/event.model';
// import { EventStatus, EventTypeEnum, TicketTypeEnum, EventCategory } from '../../models/enum';
// import { signal } from '@angular/core';
// import { HttpClientTestingModule } from '@angular/common/http/testing';
// import { ActivatedRoute } from '@angular/router';

// // Create a mock version of Getrole that we can spy on
// const mockGetrole = (token: string | null): string => {
//   return token ? 'Manager' : 'Invalid Role';
// };

// const mockActivatedRoute = {
//   snapshot: {
//     paramMap: {
//       get: (key: string) => '123' // Return a sample ID
//     }
//   }
// };

// describe('TopEvent Component', () => {
//   let component: TopEvent;
//   let fixture: ComponentFixture<TopEvent>;
//   let mockRouter: jasmine.SpyObj<Router>;
//   let mockAuth: jasmine.SpyObj<Auth>;

//   const mockRole = 'Manager';
//   const mockToken = 'mock-token';

//   const mockEvent: AppEvent = new AppEvent({
//     id: 'evt-123',
//     title: 'Angular Meetup',
//     description: 'A test meetup',
//     location: 'Remote',
//     eventDate: new Date('2025-08-15'),
//     eventStatus: EventStatus.Active,
//     category: EventCategory.Tech,
//     eventType: EventTypeEnum.Seatable,
//     images: ['img1'],
//     ticketTypes: [
//       {
//         id: 'tt-1',
//         typeName: TicketTypeEnum.VIP,
//         price: 200,
//         totalQuantity: 100,
//         bookedQuantity: 40,
//         description: 'VIP Access',
//         isDeleted: false,
//         imageUrl: ''
//       }
//     ],
//     bookedSeats: []
//   });

//   beforeEach(async () => {
//     mockRouter = jasmine.createSpyObj('Router', ['navigate'], { url: '/dashboard' });
//     mockAuth = jasmine.createSpyObj('Auth', ['getToken']);
//     mockAuth.getToken.and.returnValue(mockToken);

//     // Replace the original Getrole with our mock version
//     spyOn(window as any, 'Getrole').and.callFake(mockGetrole);

//     await TestBed.configureTestingModule({
//       imports: [TopEvent,HttpClientTestingModule,],
//       providers: [
//         { provide: ActivatedRoute, useValue: mockActivatedRoute }
//         { provide: Router, useValue: mockRouter },
//         { provide: Auth, useValue: mockAuth }
//       ]
//     }).compileComponents();

//     fixture = TestBed.createComponent(TopEvent);
//     component = fixture.componentInstance;
//   });

//   it('should create component and load user role', () => {
//     component.ngOnInit();
//     expect(component).toBeTruthy();
//     expect(component.role).toBe(mockRole);
//     expect(mockAuth.getToken).toHaveBeenCalled();
//   });

//   it('should detect if an event is cancelled', () => {
//     const cancelledEvent = new AppEvent({ ...mockEvent, eventStatus: EventStatus.Cancelled });
//     expect(component.isCancelled(cancelledEvent)).toBeTrue();
//   });

//   it('should return false for non-cancelled event', () => {
//     expect(component.isCancelled(mockEvent)).toBeFalse();
//   });

//   it('should navigate to event details correctly', () => {
//     const image = { eventId: 'evt-123' };
//     component.routeToEvent(image);
//     expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard', 'events', 'evt-123']);
//   });

//   it('should convert event status enum to string', () => {
//     expect(component.eventStatusToString(EventStatus.Completed)).toBe('Completed');
//   });

//   it('should convert event type enum to string', () => {
//     expect(component.eventTypeToString(EventTypeEnum.NonSeatable)).toBe('NonSeatable');
//   });

//   it('should convert ticket type enum to string', () => {
//     expect(component.ticketTypeToString(TicketTypeEnum.EarlyBird)).toBe('EarlyBird');
//   });

//   it('should render event title and ticket info from input signal', () => {
//     component.topEvent = signal(mockEvent);
//     fixture.detectChanges();

//     const compiled = fixture.nativeElement as HTMLElement;
//     expect(compiled.textContent).toContain('Angular Meetup');
//     expect(compiled.textContent).toContain('VIP');
//     expect(compiled.textContent).toContain('₹200');
//     expect(compiled.textContent).toContain('Available: 60');
//   });

//   it('should show gray fallback if image array is empty', () => {
//     const eventWithoutImage = new AppEvent({ ...mockEvent, images: [] });
//     component.topEvent = signal(eventWithoutImage);
//     fixture.detectChanges();

//     const compiled = fixture.nativeElement as HTMLElement;
//     const fallback = compiled.querySelector('.bg-gray-500');
//     expect(fallback).toBeTruthy();
//   });

//   it('should show "Add Event" and "View All Events" for Manager role', () => {
//     component.role = 'Manager';
//     component.topEvent = signal(mockEvent);
//     fixture.detectChanges();

//     const compiled = fixture.nativeElement as HTMLElement;
//     const buttons = compiled.querySelectorAll('button');
//     const buttonTexts = Array.from(buttons).map(btn => btn.textContent?.trim());

//     expect(buttonTexts).toContain('View All Events');
//     expect(buttonTexts).toContain('Add Event');
//   });
// });