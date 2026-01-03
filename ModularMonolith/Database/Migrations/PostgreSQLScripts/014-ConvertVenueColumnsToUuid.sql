ALTER TABLE "Event"."Events" DROP CONSTRAINT IF EXISTS "FK_Events_Venues_Venue";

DROP TABLE IF EXISTS "Event"."EventVenues";

ALTER TABLE "Event"."Events"
    ALTER COLUMN "Venue" TYPE uuid USING NULL;
          
ALTER TABLE "Event"."Events"
    ADD CONSTRAINT "FK_Events_Venues_Venue"
    FOREIGN KEY ("Venue")
    REFERENCES "Event"."Venues"("Id")
    ON DELETE RESTRICT;

DROP TABLE IF EXISTS "Ticket"."EventVenues";

CREATE TABLE "Ticket"."EventVenues" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Capacity" integer NOT NULL,
    CONSTRAINT "PK_Ticket_EventVenues" PRIMARY KEY ("Id")
);

ALTER TABLE "Ticket"."Events"
    ALTER COLUMN "Venue" TYPE uuid USING NULL;
