CREATE SCHEMA IF NOT EXISTS "Event";

CREATE TABLE "Event"."Events" (
    "Id" uuid NOT NULL,
    "EventName" text NOT NULL,
    CONSTRAINT "PK_Events" PRIMARY KEY ("Id")
);
