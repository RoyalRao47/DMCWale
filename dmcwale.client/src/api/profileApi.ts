import { apiRequest } from './axiosClient';
import type { ProfileDetails, UpdateProfileRequest, UpdateProfileResponse } from '../features/account/profile.types';

export function getProfile() {
    return apiRequest<ProfileDetails>('/api/profile');
}

export function updateProfile(request: UpdateProfileRequest) {
    return apiRequest<UpdateProfileResponse>('/api/profile', {
        method: 'PUT',
        body: JSON.stringify(request)
    });
}
