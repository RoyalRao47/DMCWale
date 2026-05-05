import { JSX, useEffect } from 'react';
import { isAuthenticated } from '../../features/auth/authStore';

type ProtectedRouteProps = {
    children: JSX.Element;
    navigate: (path: string) => void;
};

export default function ProtectedRoute({ children, navigate }: ProtectedRouteProps) {
    const allowed = isAuthenticated();

    useEffect(() => {
        if (!allowed) {
            navigate('/login');
        }
    }, [allowed, navigate]);

    return allowed ? children : null;
}
