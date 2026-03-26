import { describe, it } from "vitest";
import {
    should_show_each_error_message_when_errors_is_an_array,
    should_show_generic_message_when_errors_is_not_an_array
} from "./ticket-errors.steps";

describe("ticket-errors", () => {
    it("should show each error message when errors is an array", should_show_each_error_message_when_errors_is_an_array);
    it("should show generic message when errors is not an array", should_show_generic_message_when_errors_is_not_an_array);
});
