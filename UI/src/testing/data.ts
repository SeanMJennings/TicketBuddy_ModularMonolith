import {type Event, Venue} from '../domain/event';
import {type User, UserType} from "../domain/user.ts";
import moment from "moment";
import type {Ticket} from "../domain/ticket.ts";
import jwt from 'jsonwebtoken';

const JWT_SECRET = 'test-secret-key';
const JWT_EXPIRY = '1h';

export const Events : Event[] = [
    {
        Id: "1",
        EventName: "Concert at O2 Arena",
        StartDate: moment(new Date().setDate(new Date().getDate() + 1)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 1 + 2)),
        Venue: Venue.EmiratesOldTraffordManchester,
        Price: 50.00,
        IsSoldOut: false
    },
    {
        Id: "2",
        EventName: "Football Match at Wembley Stadium",
        StartDate: moment(new Date().setDate(new Date().getDate() + 2)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 2 + 3)),
        Venue: Venue.WembleyStadiumLondon,
        Price: 75.00,
        IsSoldOut: true
    },
    {
        Id: "3",
        EventName: "Basketball Game at Manchester Arena",
        StartDate: moment(new Date().setDate(new Date().getDate() + 3)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 3 + 1)),
        Venue: Venue.ManchesterArena,
        Price: 60.00,
        IsSoldOut: false
    },
    {
        Id: "4",
        EventName: "Concert at Utilita Arena Birmingham",
        StartDate: moment(new Date().setDate(new Date().getDate() + 4)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 4 + 1)),
        Venue: Venue.UtilitaArenaBirmingham,
        Price: 55.00,
        IsSoldOut: false
    },
    {
        Id: "5",
        EventName: "Theatre Show at SSE Hydro Glasgow",
        StartDate: moment(new Date().setDate(new Date().getDate() + 5)),
        EndDate: moment(new Date().setDate(new Date().getDate() + 5 + 1)),
        Venue: Venue.UtilitaArenaBirmingham,
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


export const OidcUsers: {
    profile: { sub: string; name: string; email: string; email_verified: boolean };
    id_token: string;
    access_token: string;
    token_type: string;
    scope: string;
    expires_at: number;
    session_state: null
}[] = Users.map(user => {
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

export const AnOidcAdminUser: {
    profile: { sub: string; name: string; email: string; email_verified: boolean };
    id_token: string;
    access_token: string;
    token_type: string;
    scope: string;
    expires_at: number;
    session_state: null
} = OidcUsers.find(u => u.profile.email === Users[3].Email)!;

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