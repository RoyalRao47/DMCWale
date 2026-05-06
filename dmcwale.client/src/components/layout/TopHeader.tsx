import * as React from 'react';
import { clearAuth, getStoredAuth } from '../../features/auth/authStore';

type TopHeaderProps = {
    navigate: (path: string) => void;
};

export default function TopHeader({ navigate }: TopHeaderProps) {
    const auth = getStoredAuth();
    const walletAmount = auth?.walletAmount ?? 0;

    function handleLogout() {
        clearAuth();
        navigate('/login');
    }

    return (
        <div className="top-header">
            <div className="container top-header-inner">
                <button className="brand" type="button" onClick={() => navigate('/home')} aria-label="DMCWale home">
                    <img src="/assets/DMCWale-logo.jpg" alt="DMCWale" />
                </button>

                <div className="top-actions">
                    <button className="wallet-button" type="button">
                        Wallet <strong>{walletAmount.toLocaleString('en-US', { minimumFractionDigits: 2 })} USD</strong>
                    </button>
                    <div className="account-menu">
                        <button className="account-button" type="button" onClick={() => navigate('/account/profile')}>
                            My Account
                        </button>
                        <div className="account-dropdown">
                            <div className="account-name">{auth?.fullName || 'My Profile'}</div>
                            <button type="button" onClick={() => navigate('/account/profile')}>
                                My Profile
                            </button>
                            <button type="button" onClick={handleLogout}>
                                Logout
                            </button>
                        </div>
                    </div>
                    <button className="cart-button" type="button">
                        Cart (0)
                    </button>
                </div>
            </div>
        </div>
    );
}
