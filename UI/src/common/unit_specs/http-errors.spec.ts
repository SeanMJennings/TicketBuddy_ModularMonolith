import {describe, it} from "vitest";
import {redirects_to_error_on_5xx_response, redirects_to_error_on_network_failure} from "./http-errors.steps.ts";

describe('http client error handling', () => {
    it('redirects to error page on 5xx response', redirects_to_error_on_5xx_response);
    it('redirects to error page on network failure', redirects_to_error_on_network_failure);
});