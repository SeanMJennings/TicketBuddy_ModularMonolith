import {describe, it} from "vitest";
import {
    should_render_event_creation_form,
    should_allow_user_to_create_new_event,
    should_display_list_of_events,
    should_navigate_back_to_events_list_when_back_button_is_clicked,
    should_allow_user_to_edit_existing_event,
    should_show_error_toast_when_event_update_fails,
    should_show_error_toast_when_event_creation_fails,
    should_not_allow_venue_change_when_editing_event,
    should_navigate_back_when_event_fetch_fails_in_edit_mode,
    should_show_generic_error_toast_when_server_error_has_no_errors_array,
    should_render_empty_list_when_api_fails
} from "./EventsManagement.steps.ts";

describe("Events Management", () => {
    it('should display list of events', should_display_list_of_events);
    it("should render event creation form when add event icon clicked", should_render_event_creation_form);
    it('should allow user to create a new event', should_allow_user_to_create_new_event);
    it('should navigate back to events list when back button is clicked', should_navigate_back_to_events_list_when_back_button_is_clicked);
    it('should allow user to edit an existing event', should_allow_user_to_edit_existing_event);
    it('should show error toast when event update fails', should_show_error_toast_when_event_update_fails);
    it('should show error toast when event creation fails', should_show_error_toast_when_event_creation_fails);
    it('should not allow venue change when editing event', should_not_allow_venue_change_when_editing_event);
    it('should navigate back when event fetch fails in edit mode', should_navigate_back_when_event_fetch_fails_in_edit_mode);
    it('should show generic error toast when server error has no errors array', should_show_generic_error_toast_when_server_error_has_no_errors_array);
    it('should render empty list when API fails', should_render_empty_list_when_api_fails);
});
