CREATE SCHEMA IF NOT EXISTS "User";

CREATE TABLE "User"."Users" (
    "Id" uuid NOT NULL,
    "FullName" text NOT NULL,
    "Email" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);
