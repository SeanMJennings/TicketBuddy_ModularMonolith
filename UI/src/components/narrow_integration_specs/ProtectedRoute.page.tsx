import {render, screen} from "@testing-library/react";
import {MemoryRouter, Route, Routes} from "react-router-dom";
import {ProtectedRoute} from "../ProtectedRoute";
import type {UserType} from "../../domain/user.ts";

let renderResult: ReturnType<typeof render>;

export function renderProtectedRoute({ requiredType }: { requiredType?: UserType }) {
    renderResult = render(
        <MemoryRouter initialEntries={["/protected"]}>
            <Routes>
                <Route path="/protected" element={
                    <ProtectedRoute requiredUserType={requiredType}>
                        <div data-testid="protected-content">Protected Content</div>
                    </ProtectedRoute>
                } />
                <Route path="/" element={<div data-testid="home-page">Home Page</div>} />
            </Routes>
        </MemoryRouter>
    );
}


export function unrenderProtectedRoute() {
    renderResult.unmount();
}

export function protectedContentIsRendered(): boolean {
    return screen.queryByTestId("protected-content") !== null;
}

export function redirectedToHomePage(): boolean {
    return screen.queryByTestId("home-page") !== null;
}

