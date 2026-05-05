import { useCallback, useEffect, useMemo, useState } from 'react';
import ProtectedRoute from '../components/layout/ProtectedRoute';
import LoginPage from '../features/auth/LoginPage';
import { isAuthenticated } from '../features/auth/authStore';
import HomePage from '../features/home/HomePage';

function getCurrentPath() {
    return window.location.pathname === '/' ? '/login' : window.location.pathname;
}

export default function AppRoutes() {
    const [path, setPath] = useState(getCurrentPath);

    const navigate = useCallback((nextPath: string) => {
        window.history.pushState({}, '', nextPath);
        setPath(nextPath);
    }, []);

    useEffect(() => {
        function handlePopState() {
            setPath(getCurrentPath());
        }

        window.addEventListener('popstate', handlePopState);
        return () => window.removeEventListener('popstate', handlePopState);
    }, []);

    useEffect(() => {
        if (window.location.pathname === '/') {
            navigate(isAuthenticated() ? '/home' : '/login');
        }
    }, [navigate]);

    return useMemo(() => {
        if (path === '/home') {
            return (
                <ProtectedRoute navigate={navigate}>
                    <HomePage navigate={navigate} />
                </ProtectedRoute>
            );
        }

        return <LoginPage navigate={navigate} />;
    }, [navigate, path]);
}
