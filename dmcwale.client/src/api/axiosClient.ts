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

function getHttpErrorMessage(status: number) {
    if (status === 401) {
        return 'Your session has expired. Please login again.';
    }

    if (status === 403) {
        return 'You do not have permission to perform this action.';
    }

    if (status === 404) {
        return 'Requested profile service was not found. Please restart the API server and try again.';
    }

    if (status >= 500) {
        return 'Profile service is unavailable right now. Please try again.';
    }

    return 'Request could not be completed.';
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
        const errors = Array.isArray(payload?.errors) ? payload.errors : [];
        const message = errors.length > 0
            ? errors.join(' ')
            : payload?.message ?? getHttpErrorMessage(response.status);
        throw new Error(message);
    }

    return payload as TResponse;
}
