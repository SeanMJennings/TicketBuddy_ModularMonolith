import { test } from '@playwright/test';
import {user_can_buy_tickets} from "./ticketbuddy.steps";

test('user can buy tickets', user_can_buy_tickets);