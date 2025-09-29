ALTER TABLE "Event"."Events" RENAME COLUMN "Date" TO "StartDate";

ALTER TABLE "Event"."Events" 
    ADD COLUMN "EndDate" timestamptz NULL;

UPDATE "Event"."Events"
SET "EndDate" = "StartDate" + INTERVAL '1 day'
WHERE "EndDate" IS NULL;

ALTER TABLE "Event"."Events" 
    ALTER COLUMN "EndDate" SET NOT NULL;
