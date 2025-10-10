import {describe, it} from "vitest";
import {should_load_events_on_render, should_navigate_to_tickets_page_when_find_tickets_clicked, should_show_sold_out_message_for_sold_out_events} from "./Home.steps.ts";

describe("Home", () => {
    it("should load events on render", should_load_events_on_render);
    it("should navigate to Tickets page when Find Tickets is clicked", should_navigate_to_tickets_page_when_find_tickets_clicked);
    it("should show sold out message for sold out events", should_show_sold_out_message_for_sold_out_events);
});