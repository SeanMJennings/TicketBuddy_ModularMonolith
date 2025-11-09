import {defineProject, mergeConfig} from "vitest/config";
import viteConfig from './vite.config';
import {loadEnv} from "vite";
import react from "@vitejs/plugin-react";

export default mergeConfig(viteConfig, defineProject({
    plugins: [react()],
    test: {
        environment: 'jsdom',
        env: loadEnv('testing', process.cwd(), ''),
        setupFiles: './src/setupTests.ts',
    }
}));