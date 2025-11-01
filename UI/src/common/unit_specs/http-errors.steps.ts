import {afterEach, beforeEach, expect, vi} from "vitest";
import {get} from "../http.ts";

const originalLocation = window.location;

beforeEach(() => {
    // @ts-expect-error
    delete window.location;
    // @ts-expect-error
    window.location = { assign: vi.fn() };
});

afterEach(() => {
    // @ts-expect-error
    window.location = originalLocation;
})

export async function redirects_to_error_on_5xx_response() {
    global.fetch = vi.fn().mockResolvedValue({
        ok: false,
        status: 500,
        json: async () => ({ error: 'Server error' })
    });
    await expect(get('/test')).rejects.toThrow();
    expect(window.location.assign).toHaveBeenCalledWith('/error');
}

export async function redirects_to_error_on_network_failure() {
    global.fetch = vi.fn().mockRejectedValue(new Error('Network error'));
    await get('/test');
    expect(window.location.assign).toHaveBeenCalledWith('/error');
}