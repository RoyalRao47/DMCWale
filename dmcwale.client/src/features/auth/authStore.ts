import type { AuthUser } from './auth.types';

const AUTH_STORAGE_KEY = 'dmcwale.client.auth';

export function getStoredAuth(): AuthUser | null {
    const raw = sessionStorage.getItem(AUTH_STORAGE_KEY) ?? localStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) {
        return null;
    }

    try {
        return JSON.parse(raw) as AuthUser;
    } catch {
        clearAuth();
        return null;
    }
}

export function saveAuth(auth: AuthUser, remember: boolean) {
    clearAuth();
    const storage = remember ? localStorage : sessionStorage;
    storage.setItem(AUTH_STORAGE_KEY, JSON.stringify(auth));
}

export function clearAuth() {
    sessionStorage.removeItem(AUTH_STORAGE_KEY);
    localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getAuthToken() {
    return getStoredAuth()?.token ?? '';
}

export function isAuthenticated() {
    return Boolean(getAuthToken());
}
