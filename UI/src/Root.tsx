import {AuthProvider} from "react-oidc-context";
import {onSigninCallback, onSignoutCallback, userManager} from "./oidc/config.ts";
import {BrowserRouter} from "react-router-dom";
import App from "./app/App.tsx";

export const TheApp = () => {
    return (
        <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback} onSignoutCallback={onSignoutCallback}>
            <BrowserRouter>
                <App />
            </BrowserRouter>,
        </AuthProvider>
    )
}