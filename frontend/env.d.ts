/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_ADMINUI_BASE_URL: string;
    readonly VITE_DATA_CLEANER_RUN_TIME_UTC: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv
}