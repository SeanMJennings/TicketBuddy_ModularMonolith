import {Locator, Page} from "@playwright/test";

export const getFirstEvent = (page: Page) => page.getByTestId("event-item").first();
export const getFindTicketButton = (locator: Locator) => locator.locator("button").first();
export const getSeatByNumber = (page: Page, number: number) => page.locator(`[data-seat="${number}"]`);
export const getProceedToPurchaseButton = (page: Page) => page.getByRole('button', { name: 'Proceed to Purchase' });
export const getCompletePurchaseButton = (page: Page) => page.getByRole('button', { name: 'Complete Purchase' });
export const getBackToEventsButton = (page: Page) => page.getByRole('button', { name: 'Back to Events' });
export const getUserIcon = (page: Page) => page.getByTestId('user-icon');
export const getTicketItems = (page: Page) => page.locator('[data-testid="ticket-item"]');