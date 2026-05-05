import type { LoginRequest } from './auth.types';

export type LoginErrors = Partial<Record<keyof LoginRequest, string>>;

export function validateLogin(values: LoginRequest): LoginErrors {
    const errors: LoginErrors = {};

    if (!values.agentSupplierCode.trim()) {
        errors.agentSupplierCode = 'Agent/Supplier Code is required.';
    }

    if (!values.email.trim()) {
        errors.email = 'Email/User Name is required.';
    }

    if (!values.password) {
        errors.password = 'Password is required.';
    }

    return errors;
}
