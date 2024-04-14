import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import { fileURLToPath, URL } from "node:url";
import mkcert from 'vite-plugin-mkcert';

export default defineConfig({
    plugins: [vue(), mkcert()],

    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url)),
        },
        extensions: ['.vue', '.js']
    },
    server: {
        proxy: {
            '^/swagger': {
             target: 'https://localhost:7197/',
             secure: false
            }
         },
        port: 5173,
    }
});
