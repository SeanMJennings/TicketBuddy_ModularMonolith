import {render, screen} from "@testing-library/react";
import {MemoryRouter, Route, Routes} from "react-router-dom";
import {ProtectedRoute} from "../ProtectedRoute";
import type {UserType} from "../../domain/user.ts";

let renderResult: ReturnType<typeof render>;

function TestProtectedRoute({ requiredType }: { requiredType?: UserType }) {
    return (
        <MemoryRouter initialEntries={["/protected"]}>
            <Routes>
                <Route path="/protected" element={
                    <ProtectedRoute requiredUserType={requiredType}>
                        <div data-testid="protected-content">Protected Content</div>
                    </ProtectedRoute>
                } />
                <Route path="/" element={<div data-testid="login-page">Login Page</div>} />
                <Route path="/unauthorized" element={<div data-testid="unauthorized-page">Unauthorized Page</div>} />
            </Routes>
        </MemoryRouter>
    );
}

export function renderProtectedRoute(options: { requiredType?: UserType }) {
    renderResult = render(<TestProtectedRoute {...options} />);
}

export function unrenderProtectedRoute() {
    renderResult.unmount();
}

export function protectedContentIsRendered(): boolean {
    return screen.queryByTestId("protected-content") !== null;
}

export function redirectedToLogin(): boolean {
    return screen.queryByTestId("login-page") !== null;
}

export function redirectedToUnauthorized(): boolean {
    return screen.queryByTestId("unauthorized-page") !== null;
}

