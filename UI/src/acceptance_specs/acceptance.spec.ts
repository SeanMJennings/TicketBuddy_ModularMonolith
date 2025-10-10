import {describe, it} from "vitest";
import {should_allow_a_user_to_purchase_a_ticket} from "./acceptance.steps.ts";

describe('ticket buddy', () => {
    it('should allow a user to purchase a ticket', should_allow_a_user_to_purchase_a_ticket);
})