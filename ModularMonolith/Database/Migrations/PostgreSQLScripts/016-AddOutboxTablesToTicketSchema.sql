CREATE TABLE "Ticket"."InboxState" (
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
    CONSTRAINT "PK_InboxState" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_InboxState_MessageId_ConsumerId" UNIQUE ("MessageId", "ConsumerId")
);

CREATE TABLE "Ticket"."OutboxMessage" (
    "SequenceNumber" bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
    "EnqueueTime" timestamptz NULL,
    "SentTime" timestamptz NOT NULL,
    "Headers" text NULL,
    "Properties" text NULL,
    "InboxMessageId" uuid NULL,
    "InboxConsumerId" uuid NULL,
    "OutboxId" uuid NULL,
    "MessageId" uuid NOT NULL,
    "ContentType" varchar(256) NOT NULL,
    "MessageType" text NOT NULL,
    "Body" text NOT NULL,
    "ConversationId" uuid NULL,
    "CorrelationId" uuid NULL,
    "InitiatorId" uuid NULL,
    "RequestId" uuid NULL,
    "SourceAddress" varchar(256) NULL,
    "DestinationAddress" varchar(256) NULL,
    "ResponseAddress" varchar(256) NULL,
    "FaultAddress" varchar(256) NULL,
    "ExpirationTime" timestamptz NULL,
    CONSTRAINT "PK_OutboxMessage" PRIMARY KEY ("SequenceNumber")
);

CREATE TABLE "Ticket"."OutboxState" (
    "OutboxId" uuid NOT NULL,
    "LockId" uuid NOT NULL,
    "LockToken" uuid NULL,
    "RowVersion" bytea NULL,
    "Created" timestamptz NOT NULL,
    "Delivered" timestamptz NULL,
    "LastSequenceNumber" bigint NULL,
    CONSTRAINT "PK_OutboxState" PRIMARY KEY ("OutboxId")
);