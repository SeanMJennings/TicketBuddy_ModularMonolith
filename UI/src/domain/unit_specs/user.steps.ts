import { expect } from "vitest";
import { UserSchema, OidcUserSchema } from "../user";

export function should_parse_valid_user() {
    const validUser = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        FullName: "John Smith",
        Email: "john.smith@example.com",
        UserType: "Customer"
    };

    const result = UserSchema.safeParse(validUser);

    expect(result.success).toBe(true);
    expect(result.data?.Id).toBe(validUser.Id);
    expect(result.data?.FullName).toBe(validUser.FullName);
    expect(result.data?.Email).toBe(validUser.Email);
    expect(result.data?.UserType).toBe(validUser.UserType);
}

export function should_parse_customer_user_type() {
    const customerUser = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        FullName: "Jane Doe",
        Email: "jane.doe@example.com",
        UserType: "Customer"
    };

    const result = UserSchema.safeParse(customerUser);

    expect(result.success).toBe(true);
    expect(result.data?.UserType).toBe("Customer");
}

export function should_parse_administrator_user_type() {
    const adminUser = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        FullName: "Admin User",
        Email: "admin@example.com",
        UserType: "Administrator"
    };

    const result = UserSchema.safeParse(adminUser);

    expect(result.success).toBe(true);
    expect(result.data?.UserType).toBe("Administrator");
}

export function should_reject_user_with_missing_fields() {
    const invalidUser = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        FullName: "John Smith"
    };

    const result = UserSchema.safeParse(invalidUser);

    expect(result.success).toBe(false);
}

export function should_reject_user_with_invalid_user_type() {
    const invalidUser = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        FullName: "John Smith",
        Email: "john.smith@example.com",
        UserType: "InvalidType"
    };

    const result = UserSchema.safeParse(invalidUser);

    expect(result.success).toBe(false);
}

export function should_parse_valid_oidc_user() {
    const validOidcUser = {
        profile: {
            sub: "auth0|abc123",
            name: "John Smith",
            email: "john.smith@example.com",
            email_verified: true
        },
        id_token: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
        access_token: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
        token_type: "Bearer",
        scope: "openid profile email",
        expires_at: 1706025600,
        session_state: null
    };

    const result = OidcUserSchema.safeParse(validOidcUser);

    expect(result.success).toBe(true);
    expect(result.data?.profile.sub).toBe(validOidcUser.profile.sub);
    expect(result.data?.profile.name).toBe(validOidcUser.profile.name);
    expect(result.data?.profile.email).toBe(validOidcUser.profile.email);
    expect(result.data?.profile.email_verified).toBe(true);
    expect(result.data?.id_token).toBe(validOidcUser.id_token);
    expect(result.data?.access_token).toBe(validOidcUser.access_token);
    expect(result.data?.token_type).toBe(validOidcUser.token_type);
    expect(result.data?.scope).toBe(validOidcUser.scope);
    expect(result.data?.expires_at).toBe(validOidcUser.expires_at);
    expect(result.data?.session_state).toBe(null);
}

export function should_reject_oidc_user_with_missing_profile() {
    const invalidOidcUser = {
        id_token: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
        access_token: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
        token_type: "Bearer",
        scope: "openid profile email",
        expires_at: 1706025600,
        session_state: null
    };

    const result = OidcUserSchema.safeParse(invalidOidcUser);

    expect(result.success).toBe(false);
}

export function should_reject_oidc_user_with_missing_tokens() {
    const invalidOidcUser = {
        profile: {
            sub: "auth0|abc123",
            name: "John Smith",
            email: "john.smith@example.com",
            email_verified: true
        },
        token_type: "Bearer",
        scope: "openid profile email",
        expires_at: 1706025600,
        session_state: null
    };

    const result = OidcUserSchema.safeParse(invalidOidcUser);

    expect(result.success).toBe(false);
}
