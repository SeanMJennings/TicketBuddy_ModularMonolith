import { vi } from 'vitest';
import React from 'react';

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: true,
            user: null,
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});