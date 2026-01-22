import type {Event} from '../domain/event';
import {type OidcUser, type User, UserType} from "../domain/user.ts";
import moment from "moment";
import type {Ticket} from "../domain/ticket.ts";
import jwt from 'jsonwebtoken';
import type { Venue as VenueEntity } from "../domain/venue.ts";
import type { Notification } from "../domain/notification.ts";

const JWT_SECRET = 'test-secret-key';
const JWT_EXPIRY = '1h';

export const Events : Event[] = [
    {
        Id: "1",
        EventName: "Concert at O2 Arena",
        StartDate: moment(new Date().setDate(new Date().getDate() + 1)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 1 + 2)),
        VenueId: "v1-uuid-0000-0000-000000000001",
        Price: 50.00,
        IsSoldOut: false
    },
    {
        Id: "2",
        EventName: "Football Match at Wembley Stadium",
        StartDate: moment(new Date().setDate(new Date().getDate() + 2)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 2 + 3)),
        VenueId: "v2-uuid-0000-0000-000000000002",
        Price: 75.00,
        IsSoldOut: true
    },
    {
        Id: "3",
        EventName: "Basketball Game at Manchester Arena",
        StartDate: moment(new Date().setDate(new Date().getDate() + 3)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 3 + 1)),
        VenueId: "v2-uuid-0000-0000-000000000002",
        Price: 60.00,
        IsSoldOut: false
    },
    {
        Id: "4",
        EventName: "Concert at Utilita Arena Birmingham",
        StartDate: moment(new Date().setDate(new Date().getDate() + 4)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 4 + 1)),
        VenueId: "v3-uuid-0000-0000-000000000003",
        Price: 55.00,
        IsSoldOut: false
    },
    {
        Id: "5",
        EventName: "Theatre Show at SSE Hydro Glasgow",
        StartDate: moment(new Date().setDate(new Date().getDate() + 5)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 5 + 1)),
        VenueId: "v1-uuid-0000-0000-000000000001",
        Price: 65.00,
        IsSoldOut: false
    }
]

export const Users : User[] = [
    {
        Id: "1",
        FullName: "John Doe",
        Email: "john.doe@customer.co.uk",
        UserType: UserType.Customer
    },
    {
        Id: "2",
        FullName: "Jane Doe",
        Email: "jane.doe@customer.co.uk",
        UserType: UserType.Customer
    },
    {
        Id: "3",
        FullName: "Will Chan",
        Email: "will.chan@customers.co.uk",
        UserType: UserType.Customer
    },
    {
        Id: "4",
        FullName: "Sean Connery",
        Email: "sean.connery@ticketbuddy.co.uk",
        UserType: UserType.Administrator
    },
    {
        Id: "5",
        FullName: "Roger Moore",
        Email: "roger.moore@ticketbuddy.co.uk",
        UserType: UserType.Administrator
    },
]

const createJwt = (payload: object): string => {
    return jwt.sign(payload, JWT_SECRET, { expiresIn: JWT_EXPIRY });
};


export const OidcUsers: OidcUser[] = Users.map(user => {
    const tokenPayload = {
        sub: user.Id,
        name: user.FullName,
        email: user.Email,
        email_verified: true,
        iat: Math.floor(Date.now() / 1000),
        realm_access: {
            roles: [user.UserType === UserType.Administrator ? 'ticketbuddy-admin' : 'ticketbuddy-customer']
        }
    };

    return {
        profile: {
            sub: user.Id,
            name: user.FullName,
            email: user.Email,
            email_verified: true
        },
        id_token: createJwt(tokenPayload),
        access_token: createJwt({ ...tokenPayload, scope: "openid profile email" }),
        token_type: "Bearer",
        scope: "openid profile email",
        expires_at: Math.floor(Date.now() / 1000) + 3600,
        session_state: null
    };
});

export const AnOidcAdminUser: OidcUser = OidcUsers.find(u => u.profile.email === Users[3].Email)!;

export const AnOidcCustomerUser: OidcUser = OidcUsers.find(u => u.profile.email === Users[0].Email)!;

export const TicketsForFirstEvent: Ticket[] = [
    {
        Id: "t1",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 1,
        Purchased: false
    },
    {
        Id: "t2",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 2,
        Purchased: false
    },
    {
        Id: "t3",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 3,
        Purchased: false
    },
    {
        Id: "t4",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 4,
        Purchased: false
    },
    {
        Id: "t5",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 5,
        Purchased: false
    },
    {
        Id: "t6",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 6,
        Purchased: false
    },
    {
        Id: "t7",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 7,
        Purchased: false
    },
    {
        Id: "t8",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 8,
        Purchased: false
    },
    {
        Id: "t9",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 9,
        Purchased: false
    },
    {
        Id: "t10",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 10,
        Purchased: true
    }
]

export const TicketBoughtForFirstEvent: Ticket[] = [
    {
        Id: "t1",
        EventId: Events[0].Id,
        Price: 50,
        SeatNumber: 1,
        Purchased: true
    }
]

export const Venues: VenueEntity[] = [
    {
        Id: "v1-uuid-0000-0000-000000000001",
        Name: "The Grand Theater",
        Address: {
            Street: "123 Main Street",
            City: "London",
            Postcode: "SW1A 1AA"
        },
        Capacity: 25
    },
    {
        Id: "v2-uuid-0000-0000-000000000002",
        Name: "Manchester Arena",
        Address: {
            Street: "21 Arena Way",
            City: "Manchester",
            Postcode: "M3 1AR"
        },
        Capacity: 50
    },
    {
        Id: "v3-uuid-0000-0000-000000000003",
        Name: "The Small Room",
        Address: {
            Street: "1 Tiny Lane",
            City: "London",
            Postcode: "HA9 0WS"
        },
        Capacity: 10
    }
]

export const NotificationsForFirstUser: Notification[] = [
    {
        Id: "n1-uuid-0000-0000-000000000001",
        UserId: Users[0].Id,
        Type: "TicketPurchased",
        Payload: JSON.stringify({
            eventId: Events[0].Id,
            ticketId: TicketsForFirstEvent[0].Id,
            eventName: Events[0].EventName
        }),
        IsRead: false,
        CreatedAt: "2026-01-22T10:30:00+00:00"
    },
    {
        Id: "n2-uuid-0000-0000-000000000002",
        UserId: Users[0].Id,
        Type: "TicketPurchased",
        Payload: JSON.stringify({
            eventId: Events[1].Id,
            ticketId: "t-purchased-uuid",
            eventName: Events[1].EventName
        }),
        IsRead: true,
        CreatedAt: "2026-01-21T14:00:00+00:00"
    }
]

export const createNotification = (overrides: Partial<Notification> = {}): Notification => ({
    Id: "n-test-uuid-0000-000000000000",
    UserId: Users[0].Id,
    Type: "TicketPurchased",
    Payload: JSON.stringify({
        eventId: Events[0].Id,
        ticketId: TicketsForFirstEvent[0].Id,
        eventName: Events[0].EventName
    }),
    IsRead: false,
    CreatedAt: "2026-01-22T10:30:00+00:00",
    ...overrides
})