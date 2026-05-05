import { getAuthToken } from '../features/auth/authStore';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

type ApiRequestOptions = RequestInit & {
    skipAuth?: boolean;
};

async function parseResponse(response: Response) {
    const contentType = response.headers.get('content-type') ?? '';
    if (!contentType.includes('application/json')) {
        return null;
    }

    return response.json();
}

export async function apiRequest<TResponse>(
    path: string,
    options: ApiRequestOptions = {}
): Promise<TResponse> {
    const token = getAuthToken();
    const headers = new Headers(options.headers);

    if (!headers.has('Content-Type') && options.body) {
        headers.set('Content-Type', 'application/json');
    }

    if (!options.skipAuth && token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    const response = await fetch(`${API_BASE_URL}${path}`, {
        ...options,
        headers
    });

    const payload = await parseResponse(response);

    if (!response.ok) {
        const message = payload?.message ?? 'Request could not be completed.';
        throw new Error(message);
    }

    return payload as TResponse;
}
