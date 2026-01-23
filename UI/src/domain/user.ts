import { z } from "zod";

export enum UserType {
    Customer = "Customer",
    Administrator = "Administrator",
}

export const UserSchema = z.object({
    Id: z.string(),
    FullName: z.string(),
    Email: z.string(),
    UserType: z.nativeEnum(UserType)
});

const OidcProfileSchema = z.object({
    sub: z.string(),
    name: z.string(),
    email: z.string(),
    email_verified: z.boolean()
});

export const OidcUserSchema = z.object({
    profile: OidcProfileSchema,
    id_token: z.string(),
    access_token: z.string(),
    token_type: z.string(),
    scope: z.string(),
    expires_at: z.number(),
    session_state: z.null()
});

export type User = z.infer<typeof UserSchema>;
export type OidcUser = z.infer<typeof OidcUserSchema>;