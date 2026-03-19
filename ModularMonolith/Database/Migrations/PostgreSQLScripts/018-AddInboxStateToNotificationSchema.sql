CREATE TABLE "Notification"."InboxState" (
    "Id" bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
    "MessageId" uuid NOT NULL,
    "ConsumerId" uuid NOT NULL,
    "LockId" uuid NOT NULL,
    "RowVersion" bytea NULL,
    "Received" timestamptz NOT NULL,
    "ReceiveCount" integer NOT NULL,
    "ExpirationTime" timestamptz NULL,
    "Consumed" timestamptz NULL,
    "Delivered" timestamptz NULL,
    "LastSequenceNumber" bigint NULL,
    CONSTRAINT "PK_InboxState_Notification" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_InboxState_Notification_MessageId_ConsumerId" UNIQUE ("MessageId", "ConsumerId")
);