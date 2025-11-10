import { vi } from 'vitest';
import React from 'react';
import {AnOidcAdminUser} from "./testing/data.ts";

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: true,
            user: AnOidcAdminUser,
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});