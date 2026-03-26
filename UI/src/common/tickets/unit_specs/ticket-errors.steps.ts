import { expect, vi } from "vitest";
import { handleError } from "../ticket-errors";

vi.mock("react-toastify", () => ({
    toast: { error: vi.fn() },
}));

import { toast } from "react-toastify";

export function should_show_each_error_message_when_errors_is_an_array() {
    handleError({ errors: ["Seat already taken", "Ticket expired"], code: 422 });
    expect(toast.error).toHaveBeenCalledWith("Seat already taken");
    expect(toast.error).toHaveBeenCalledWith("Ticket expired");
}

export function should_show_generic_message_when_errors_is_not_an_array() {
    handleError({ errors: null as unknown as string[], code: 500 });
    expect(toast.error).toHaveBeenCalledWith("Failed to complete purchase. Please try again.");
}
