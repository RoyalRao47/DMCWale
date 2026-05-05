import { apiRequest } from './axiosClient';
import type { LoginRequest, LoginResponse } from '../features/auth/auth.types';

export function login(request: LoginRequest) {
    return apiRequest<LoginResponse>('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify(request),
        skipAuth: true
    });
}
