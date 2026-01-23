import { describe, it } from "vitest";
import {
    should_parse_valid_user,
    should_parse_customer_user_type,
    should_parse_administrator_user_type,
    should_reject_user_with_missing_fields,
    should_reject_user_with_invalid_user_type,
    should_parse_valid_oidc_user,
    should_reject_oidc_user_with_missing_profile,
    should_reject_oidc_user_with_missing_tokens
} from "./user.steps";

describe("User Schema", () => {
    it("should parse a valid user object", should_parse_valid_user);
    it("should parse user with Customer user type", should_parse_customer_user_type);
    it("should parse user with Administrator user type", should_parse_administrator_user_type);
    it("should reject user with missing required fields", should_reject_user_with_missing_fields);
    it("should reject user with invalid user type", should_reject_user_with_invalid_user_type);
});

describe("OidcUser Schema", () => {
    it("should parse a valid OIDC user object", should_parse_valid_oidc_user);
    it("should reject OIDC user with missing profile", should_reject_oidc_user_with_missing_profile);
    it("should reject OIDC user with missing tokens", should_reject_oidc_user_with_missing_tokens);
});
