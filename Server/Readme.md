# Event Ticket Booking API

## API Response Structure

All API responses follow a consistent JSON format:

```json
{
  "$id": "string",
  "success": boolean,
  "message": "string",
  "data": object | array | null,
  "errors": array | null
}
```

## Pagination Structure

Paginated responses include metadata in the `data` field:

```json
"data": {
  "$id": "string",
  "items": {
    "$id": "string",
    "$values": [ ... ]
  },
  "pageNumber": integer,
  "pageSize": integer,
  "totalItems": integer,
  "totalPages": integer
}
```

## API Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | `{{url}}/auth/login` | Login to the application |
| POST | `{{url}}/auth/refresh` | Refresh authentication token |
| GET | `{{url}}/auth/logout` | Logout of the application |
| GET | `{{url}}/events?pageNumber=1&pageSize=10` | Fetch events with pagination |
| POST | `{{url}}/events` | Add a new event |
| GET | `{{url}}/events/{{eventId_1}}` | Fetch a specific event |
| GET | `{{url}}/events/myevents?pageNumber=1&pageSize=10` | Fetch events managed by a specific manager |
| PUT | `{{url}}/events/{{eventId_1}}` | Update an event |
| DELETE | `{{url}}/events/{{eventId_2}}` | Delete an event |
| GET | `{{url}}/events/filter?searchElement=food&date=...` | Filter events with search criteria |
| GET | `{{url}}/payments/{{paymentId_1}}` | Fetch a specific payment |
| GET | `{{url}}/payments/user/{{userId_1}}` | Fetch payments for a specific user |
| GET | `{{url}}/payments/event/{{eventId_1}}` | Fetch payments for an event |
| GET | `{{url}}/payments/ticket/{{ticketId_1}}` | Fetch payment for a ticket |
| GET | `{{url}}/tickets/mine?pageNumber=1&pageSize=10` | Fetch user's tickets with pagination |
| POST | `{{url}}/tickets/book` | Book a ticket |
| GET | `{{url}}/tickets/{{ticketId_1}}` | Fetch a specific ticket |
| GET | `{{url}}/tickets/{{ticketId_1}}/export` | Export ticket as PDF |
| GET | `{{url}}/tickets/event/{{eventId_1}}?pageNumber=1&pageSize=10` | Fetch tickets for an event |
| DELETE | `{{url}}/tickets/{{ticketId_2}}/cancel` | Cancel a ticket |
| GET | `{{url}}/tickettype/event/{{eventId_1}}` | Fetch ticket types for an event |
| GET | `{{url}}/tickettype/{{ticketTypeId_1}}` | Fetch a specific ticket type |
| PUT | `{{url}}/tickettype/{{ticketTypeId_1}}` | Update a ticket type |
| DELETE | `{{url}}/tickettype/{{ticketTypeId_2}}` | Delete a ticket type |
| POST | `{{url}}/tickettype` | Add a new ticket type |
| POST | `{{url}}/users/admin` | Add an admin user |
| POST | `{{url}}/users/manager` | Add an event manager |
| POST | `{{url}}/users/register` | Register a new user |
| GET | `{{url}}/users/me` | Fetch current user details |
| PUT | `{{url}}/users/update` | Update user profile |
| PUT | `{{url}}/users/changepassword` | Change user password |
| DELETE | `{{url}}/users/delete/{{userId_2}}` | Delete a user |
| GET | `{{url}}/analytics/my-earnings` | Fetch manager's total earnings |
| GET | `{{url}}/analytics/booking-trends` | Fetch booking trends (admin only) |
| GET | `{{url}}/analytics/top-events` | Fetch top events (admin only) |

---

**Notes**:
1. `{{url}}` = Base URL (`http://localhost:{port}/api/v1`)
2. Placeholders like `{{eventId_1}}` require actual resource IDs
3. Pagination parameters: `pageNumber` (current page), `pageSize` (items per page)
