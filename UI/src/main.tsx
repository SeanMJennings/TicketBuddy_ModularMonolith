import { createRoot } from 'react-dom/client'
import './index.css'
import { TheApp } from "./Root.tsx";

createRoot(document.getElementById('root')!).render(TheApp());
