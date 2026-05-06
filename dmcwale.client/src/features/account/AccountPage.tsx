import { FormEvent, ReactNode, useEffect, useMemo, useState } from 'react';
import { getProfile, updateProfile } from '../../api/profileApi';
import Header from '../../components/layout/Header';
import { getStoredAuth, updateStoredAuth } from '../auth/authStore';
import type { ProfileDetails } from './profile.types';
import * as React from 'react';

type AccountPageProps = {
    navigate: (path: string) => void;
    activeSection?: 'profile' | 'customizePackage';
};

type ProfileFormValues = {
    fullName: string;
    email: string;
    mobile: string;
    city: string;
    address: string;
};

type ProfileErrors = Partial<Record<keyof ProfileFormValues, string>>;

const menuItems = [
    { label: 'My Profile', icon: '♙', path: '/account/profile' },
    { label: 'My Bookings', icon: '🛒', path: '/account/bookings' },
    { label: 'Customize Package', icon: '🛒', path: '/account/customize-package' },
    { label: 'Balance Sheet', icon: '☷', path: '/account/balance-sheet' },
    { label: 'Online Recharge', icon: '🛒', path: '/account/recharge' },
    { label: 'Settings', icon: '⚙', path: '/account/settings' }
];

function buildFallbackProfile(): ProfileDetails {
    const auth = getStoredAuth();
    const [firstName = '', ...lastNameParts] = (auth?.fullName ?? '').split(' ').filter(Boolean);

    return {
        firstName,
        lastName: lastNameParts.join(' '),
        mobile: '',
        salutation: '',
        countryCode: '',
        city: '',
        address1: '',
        address2: '',
        signature: '',
        username: auth?.email ?? '',
        email: auth?.email ?? '',
        agentSupplierCode: auth?.agentSupplierCode ?? '',
        roleName: auth?.role ?? '',
        profileImagePath: null
    };
}

function toFullName(profile: ProfileDetails) {
    return [profile.firstName, profile.lastName].filter(Boolean).join(' ').trim();
}

function splitFullName(fullName: string) {
    const parts = fullName.trim().split(/\s+/).filter(Boolean);
    return {
        firstName: parts[0] ?? '',
        lastName: parts.slice(1).join(' ')
    };
}

function validate(values: ProfileFormValues) {
    const errors: ProfileErrors = {};
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const mobilePattern = /^[0-9\s+\-()]{7,30}$/;

    if (!values.fullName.trim()) {
        errors.fullName = 'Full name is required.';
    } else if (values.fullName.trim().split(/\s+/).length < 2) {
        errors.fullName = 'Enter first and last name.';
    }

    if (!values.email.trim()) {
        errors.email = 'Email is required.';
    } else if (!emailPattern.test(values.email.trim())) {
        errors.email = 'Enter a valid email address.';
    }

    if (!values.mobile.trim()) {
        errors.mobile = 'Mobile is required.';
    } else if (!mobilePattern.test(values.mobile.trim())) {
        errors.mobile = 'Enter a valid mobile number.';
    }

    if (!values.city.trim()) {
        errors.city = 'City is required.';
    }

    if (!values.address.trim()) {
        errors.address = 'Address is required.';
    }

    return errors;
}

function AccountShell({
    navigate,
    profile,
    activeLabel,
    children
}: {
    navigate: (path: string) => void;
    profile: ProfileDetails;
    activeLabel: string;
    children: ReactNode;
}) {
    const accountTitle = `${profile.roleName || 'Agent'} Account`;

    return (
        <div className="app-page">
            <Header navigate={navigate} />
            <main className="workspace-page account-workspace">
                <div className="container account-grid">
                    <aside className="account-sidebar">
                        <div className="account-sidebar-title">{accountTitle}</div>
                        <nav aria-label="Agent account">
                            {menuItems.map(item => (
                                <button
                                    className={item.label === activeLabel ? 'account-nav-item active' : 'account-nav-item'}
                                    type="button"
                                    onClick={() => navigate(item.path)}
                                    key={item.label}
                                >
                                    <span aria-hidden="true">{item.icon}</span>
                                    {item.label}
                                </button>
                            ))}
                        </nav>
                    </aside>
                    {children}
                </div>
            </main>
        </div>
    );
}

const packageRows = [
    {
        title: 'DMC30810',
        amount: '408.05 USD',
        date: 'From: 26-04-2026\nTo: 29-04-2026',
        createdOn: '23-04-2026 12:07:37 PM',
        booking: 'Pending',
        action: 'Book Package'
    },
    {
        title: 'agnc2323',
        amount: '70.28 USD',
        date: 'From: 23-04-2026\nTo: 24-04-2026',
        createdOn: '23-04-2026 11:57:42 AM',
        booking: 'Pending',
        action: 'Book Package'
    },
    {
        title: 'hgjhgjhgjhgjgj',
        amount: '0.00 USD',
        date: 'From: 21-04-2026\nTo: 21-04-2026',
        createdOn: '21-04-2026 03:32:08 PM',
        booking: 'Pending',
        action: ''
    },
    {
        title: 'dmc23456',
        amount: '348.62 USD',
        date: 'From: 23-04-2026\nTo: 26-04-2026',
        createdOn: '21-04-2026 02:56:28 PM',
        booking: 'Confirm',
        action: 'Package Booked'
    },
    {
        title: 'people',
        amount: '170.65 USD',
        date: 'From: 22-04-2026\nTo: 25-04-2026',
        createdOn: '21-04-2026 02:24:24 PM',
        booking: 'Confirm',
        action: 'Package Booked'
    },
    {
        title: 'Yatender',
        amount: '594.14 USD',
        date: 'From: 22-04-2026\nTo: 25-04-2026',
        createdOn: '21-04-2026 12:57:39 PM',
        booking: 'Confirm',
        action: 'Package Booked'
    },
    {
        title: 'Shree',
        amount: '477.32 USD',
        date: 'From: 21-04-2026\nTo: 24-04-2026',
        createdOn: '20-04-2026 12:12:11 PM',
        booking: 'Vouchered',
        action: 'Book Package'
    }
];

function CustomizePackageList({ navigate, profile }: { navigate: (path: string) => void; profile: ProfileDetails }) {
    return (
        <AccountShell navigate={navigate} profile={profile} activeLabel="Customize Package">
            <section className="account-content">
                <div className="account-page-heading">
                    <h1>Customize Package</h1>
                    <button type="button" onClick={() => navigate('/home')}>+ Create New</button>
                </div>

                <div className="package-list-panel">
                    <div className="booking-filter-bar">
                        <strong>My Bookings</strong>
                        <label>
                            Search Result
                            <select defaultValue="Booking Date">
                                <option>Booking Date</option>
                                <option>Created Date</option>
                            </select>
                        </label>
                        <label>
                            From Date
                            <input type="date" />
                        </label>
                        <label>
                            To Date
                            <input type="date" />
                        </label>
                        <label>
                            Package Name
                            <input />
                        </label>
                        <label>
                            Booking Status
                            <select defaultValue="">
                                <option value="">Select Status</option>
                                <option>Pending</option>
                                <option>Confirm</option>
                                <option>Vouchered</option>
                            </select>
                        </label>
                        <button type="button">Search</button>
                    </div>

                    <div className="package-table-wrap">
                        <table className="package-table">
                            <thead>
                                <tr>
                                    <th>Package Title</th>
                                    <th>Net Amount</th>
                                    <th>Date</th>
                                    <th>Created On</th>
                                    <th>Booking</th>
                                    <th>Upload</th>
                                    <th>Email</th>
                                    <th>View</th>
                                    <th>Edit</th>
                                    <th>P. Invoice</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {packageRows.map(row => (
                                    <tr key={`${row.title}-${row.createdOn}`}>
                                        <td>{row.title}</td>
                                        <td>{row.amount}</td>
                                        <td>{row.date.split('\n').map(line => <div key={line}>{line}</div>)}</td>
                                        <td>{row.createdOn}</td>
                                        <td><span className={`booking-badge ${row.booking.toLowerCase()}`}>{row.booking}</span></td>
                                        <td><button type="button" className="upload-btn">Upload</button></td>
                                        <td><button type="button" className="table-icon">✉</button></td>
                                        <td><button type="button" className="table-icon">◉</button></td>
                                        <td><button type="button" className="table-icon">✎</button></td>
                                        <td>{row.booking === 'Confirm' ? <span className="invoice-icon">◼</span> : null}</td>
                                        <td>
                                            {row.action ? (
                                                <button
                                                    type="button"
                                                    className={row.action === 'Package Booked' ? 'booked-btn' : 'book-btn'}
                                                >
                                                    {row.action}
                                                </button>
                                            ) : null}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </AccountShell>
    );
}

export default function AccountPage({ navigate, activeSection = 'profile' }: AccountPageProps) {
    const [profile, setProfile] = useState<ProfileDetails>(buildFallbackProfile);
    const [values, setValues] = useState<ProfileFormValues>(() => {
        const fallback = buildFallbackProfile();
        return {
            fullName: toFullName(fallback),
            email: fallback.email,
            mobile: fallback.mobile,
            city: fallback.city,
            address: fallback.address1
        };
    });
    const [errors, setErrors] = useState<ProfileErrors>({});
    const [message, setMessage] = useState('');
    const [formError, setFormError] = useState('');
    const [loadWarning, setLoadWarning] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);

    const initials = (values.fullName || profile.username || 'A').charAt(0).toUpperCase();

    useEffect(() => {
        let isMounted = true;

        getProfile()
            .then(data => {
                if (!isMounted) {
                    return;
                }

                setProfile(data);
                setValues({
                    fullName: toFullName(data),
                    email: data.email,
                    mobile: data.mobile,
                    city: data.city,
                    address: data.address1
                });
            })
            .catch(error => {
                if (isMounted) {
                    setLoadWarning(error instanceof Error
                        ? error.message
                        : 'Profile could not be loaded. Login details are shown until the API is available.');
                }
            })
            .finally(() => {
                if (isMounted) {
                    setIsLoading(false);
                }
            });

        return () => {
            isMounted = false;
        };
    }, []);

    const headerName = useMemo(() => values.fullName || toFullName(profile) || 'Agent User', [profile, values.fullName]);

    if (activeSection === 'customizePackage') {
        return <CustomizePackageList navigate={navigate} profile={profile} />;
    }

    function updateValue(field: keyof ProfileFormValues, value: string) {
        setValues(current => ({ ...current, [field]: value }));
        setErrors(current => ({ ...current, [field]: undefined }));
        setFormError('');
        setLoadWarning('');
        setMessage('');
    }

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const validationErrors = validate(values);
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        const nameParts = splitFullName(values.fullName);
        setIsSaving(true);
        setFormError('');
        setMessage('');

        try {
            const response = await updateProfile({
                firstName: nameParts.firstName,
                lastName: nameParts.lastName,
                email: values.email.trim(),
                mobile: values.mobile.trim(),
                city: values.city.trim(),
                address1: values.address.trim(),
                salutation: profile.salutation,
                countryCode: profile.countryCode,
                address2: profile.address2,
                signature: profile.signature
            });

            setProfile(response.profile);
            setValues({
                fullName: toFullName(response.profile),
                email: response.profile.email,
                mobile: response.profile.mobile,
                city: response.profile.city,
                address: response.profile.address1
            });
            updateStoredAuth({
                fullName: toFullName(response.profile),
                email: response.profile.email
            });
            setMessage(response.message || 'Profile saved successfully.');
        } catch (error) {
            setFormError(error instanceof Error ? error.message : 'Profile could not be saved.');
        } finally {
            setIsSaving(false);
        }
    }

    return (
        <AccountShell navigate={navigate} profile={profile} activeLabel="My Profile">
            <section className="account-content">
                        <div className="account-page-heading">
                            <h1>My Profile</h1>
                        </div>

                        <div className="profile-panel">
                            <div className="profile-header">
                                <div className="profile-avatar">{initials}</div>
                                <div>
                                    <h2>{headerName}</h2>
                                    <p>{profile.username ? `${profile.username} | ` : ''}{values.email}</p>
                                </div>
                            </div>

                            {isLoading ? <div className="profile-status">Loading profile...</div> : null}
                            {loadWarning ? <div className="profile-warning">{loadWarning}</div> : null}
                            {message ? <div className="profile-success">{message}</div> : null}
                            {formError ? <div className="profile-error">{formError}</div> : null}

                            <form className="profile-form" onSubmit={handleSubmit} noValidate>
                                <label>
                                    Full Name
                                    <input
                                        value={values.fullName}
                                        onChange={event => updateValue('fullName', event.target.value)}
                                        className={errors.fullName ? 'invalid' : ''}
                                    />
                                    {errors.fullName ? <span>{errors.fullName}</span> : null}
                                </label>
                                <label>
                                    Email
                                    <input
                                        value={values.email}
                                        onChange={event => updateValue('email', event.target.value)}
                                        className={errors.email ? 'invalid' : ''} readOnly
                                    />
                                    {errors.email ? <span>{errors.email}</span> : null}
                                </label>
                                <label>
                                    Agent/Supplier Code
                                    <input value={profile.agentSupplierCode} readOnly />
                                </label>
                                <label>
                                    Role
                                    <input value={profile.roleName} readOnly />
                                </label>
                                <label>
                                    Mobile
                                    <input
                                        value={values.mobile}
                                        onChange={event => updateValue('mobile', event.target.value)}
                                        className={errors.mobile ? 'invalid' : ''}
                                    />
                                    {errors.mobile ? <span>{errors.mobile}</span> : null}
                                </label>
                                <label>
                                    City
                                    <input
                                        value={values.city}
                                        onChange={event => updateValue('city', event.target.value)}
                                        className={errors.city ? 'invalid' : ''}
                                    />
                                    {errors.city ? <span>{errors.city}</span> : null}
                                </label>
                                <label className="span-2">
                                    Address
                                    <input
                                        value={values.address}
                                        onChange={event => updateValue('address', event.target.value)}
                                        className={errors.address ? 'invalid' : ''}
                                    />
                                    {errors.address ? <span>{errors.address}</span> : null}
                                </label>
                                <div className="profile-actions">
                                    <button type="submit" disabled={isSaving || isLoading}>
                                        {isSaving ? 'Saving...' : 'Save Profile'}
                                    </button>
                                </div>
                            </form>
                        </div>
            </section>
        </AccountShell>
    );
}
