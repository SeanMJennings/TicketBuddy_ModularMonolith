CREATE SCHEMA IF NOT EXISTS "Notification";

CREATE TABLE "Notification"."Notifications" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Type" text NOT NULL,
    "Payload" jsonb NOT NULL,
    "IsRead" boolean NOT NULL DEFAULT false,
    "CreatedAt" timestamptz NOT NULL,
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_Notifications_UserId" ON "Notification"."Notifications" ("UserId");
CREATE INDEX "IX_Notifications_UserId_IsRead" ON "Notification"."Notifications" ("UserId", "IsRead");
