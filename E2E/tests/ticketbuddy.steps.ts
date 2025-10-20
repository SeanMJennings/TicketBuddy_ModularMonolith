import {expect} from "@playwright/test";
import {
    getBackToEventsButton,
    getCompletePurchaseButton,
    getFindTicketButton,
    getFirstEvent,
    getProceedToPurchaseButton,
    getSeatByNumber, getTicketItems, getUserIcon
} from "./ticketbuddy.page";

export const user_can_buy_tickets = async ({ page }) => {
    await page.goto('http://localhost:5173');

    await expect(page).toHaveTitle(/TicketBuddy/);
    await getFindTicketButton(getFirstEvent(page)).click();
    await getSeatByNumber(page, 5).click();
    await getSeatByNumber(page, 6).click();
    await getProceedToPurchaseButton(page).click();
    await getCompletePurchaseButton(page).click();
    await getBackToEventsButton(page).click();
    await getUserIcon(page).click();
    const userTickets = getTicketItems(page);
    await expect(userTickets).toHaveCount(2);
    await expect(userTickets.nth(0)).toContainText('Seat 5');
    await expect(userTickets.nth(1)).toContainText('Seat 6');
}