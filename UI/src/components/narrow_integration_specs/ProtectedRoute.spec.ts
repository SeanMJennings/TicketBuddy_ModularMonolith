import {describe, it} from "vitest";
import {
    should_render_protected_content_for_authorized_user,
    should_not_render_protected_content_for_unauthenticated_user,
    should_not_render_protected_content_for_wrong_user_type
} from "./ProtectedRoute.steps";

describe("ProtectedRoute", () => {
    it("renders protected content for authorized user", should_render_protected_content_for_authorized_user);
    it("does not render protected content for unauthenticated user", should_not_render_protected_content_for_unauthenticated_user);
    it("does not render protected content for wrong user type", should_not_render_protected_content_for_wrong_user_type);
});

