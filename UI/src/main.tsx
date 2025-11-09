import { createRoot } from 'react-dom/client'
import './index.css'
import App from './app/App.tsx'
import {BrowserRouter} from "react-router-dom";
import {AuthProvider} from 'react-oidc-context';
import {onSigninCallback, userManager} from "./oidc/config.ts";

createRoot(document.getElementById('root')!).render(
    <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
        <BrowserRouter>
            <App />
        </BrowserRouter>,
    </AuthProvider>
)
