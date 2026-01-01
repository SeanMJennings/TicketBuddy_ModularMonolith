CREATE TABLE "Event"."Venues" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Address_Street" text NOT NULL,
    "Address_City" text NOT NULL,
    "Address_Postcode" text NOT NULL,
    "Capacity" integer NOT NULL,
    CONSTRAINT "PK_Venues" PRIMARY KEY ("Id")
);
