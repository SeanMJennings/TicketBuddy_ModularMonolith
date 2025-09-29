ALTER TABLE "Event"."Events"
    ADD COLUMN "Price" decimal(18, 2) NULL;

ALTER TABLE "Ticket"."Events"
    ADD COLUMN "Price" decimal(18, 2) NULL;
