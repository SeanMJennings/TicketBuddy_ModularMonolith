CREATE TABLE "Ticket"."Users" (
    "Id" uuid NOT NULL,
    "FullName" text NOT NULL,
    "Email" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

ALTER TABLE "Ticket"."Tickets"
    ADD COLUMN "UserId" uuid NULL;

ALTER TABLE "Ticket"."Tickets"
    ADD COLUMN "PurchasedAt" timestamptz NULL;
