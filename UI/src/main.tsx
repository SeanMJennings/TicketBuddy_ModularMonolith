import { createRoot } from 'react-dom/client'
import './index.css'
import App from './app/App.tsx'
import {BrowserRouter} from "react-router-dom";
import {ReactKeycloakProvider} from "@react-keycloak/web";
import {initOptions, keycloak} from "./oauth2/keycloak.ts";

createRoot(document.getElementById('root')!).render(
    <ReactKeycloakProvider authClient={keycloak} initOptions={initOptions}>
        <BrowserRouter>
            <App />
        </BrowserRouter>,
    </ReactKeycloakProvider>
)
