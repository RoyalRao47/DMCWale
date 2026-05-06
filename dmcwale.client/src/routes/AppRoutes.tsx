import { useCallback, useEffect, useMemo, useState } from 'react';
import ProtectedRoute from '../components/layout/ProtectedRoute';
import AccountPage from '../features/account/AccountPage';
import LoginPage from '../features/auth/LoginPage';
import { isAuthenticated } from '../features/auth/authStore';
import HomePage from '../features/home/HomePage';
import CustomizePackagePage from '../features/package/CustomizePackagePage.1';
import * as React from 'react';

function getCurrentPath() {
    const path = window.location.pathname === '/' ? '/login' : window.location.pathname;
    return `${path}${window.location.search}`;
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
        const [routePath, queryString = ''] = path.split('?');

        if (routePath === '/home') {
            return (
                <ProtectedRoute navigate={navigate}>
                    <HomePage navigate={navigate} />
                </ProtectedRoute>
            );
        }

        if (routePath === '/customize-package') {
            return (
                <ProtectedRoute navigate={navigate}>
                    <CustomizePackagePage navigate={navigate} queryString={queryString} />
                </ProtectedRoute>
            );
        }

        if (routePath.startsWith('/account')) {
            return (
                <ProtectedRoute navigate={navigate}>
                    <AccountPage
                        navigate={navigate}
                        activeSection={routePath === '/account/customize-package' ? 'customizePackage' : 'profile'}
                    />
                </ProtectedRoute>
            );
        }

        return <LoginPage navigate={navigate} />;
    }, [navigate, path]);
}
