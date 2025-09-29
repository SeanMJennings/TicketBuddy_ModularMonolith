CREATE TABLE "Event"."EventVenues" (
    "Id" integer NOT NULL,
    "Name" text NOT NULL,
    "Capacity" integer NOT NULL,
    CONSTRAINT "PK_EventVenues" PRIMARY KEY ("Id")
);

ALTER TABLE "Event"."Events"
    ADD COLUMN "Venue" integer NOT NULL;

INSERT INTO "Event"."EventVenues" ("Id", "Name", "Capacity")
VALUES 
    (0, 'The O2 Arena, London', 10),
    (1, 'Wembley Stadium, London', 11),
    (2, 'Manchester Arena', 12),
    (3, 'Utilita Arena, Birmingham', 13),
    (4, 'The SSE Hydro, Glasgow', 14),
    (5, 'First Direct Arena, Leeds', 15),
    (6, 'Principality Stadium, Cardiff', 16),
    (7, 'Emirates Old Trafford, Manchester', 17),
    (8, 'Royal Albert Hall, London', 18),
    (9, 'Roundhouse, London', 20);
