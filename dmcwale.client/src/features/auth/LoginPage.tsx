import { FormEvent, useState } from 'react';
import { login } from '../../api/authApi';
import Button from '../../components/common/Button';
import Loader from '../../components/common/Loader';
import { mainNavigationItems } from '../../components/layout/navigation.config';
import { saveAuth } from './authStore';
import type { LoginRequest } from './auth.types';
import { validateLogin, type LoginErrors } from './login.schema';
import * as React from 'react';

type LoginPageProps = {
    navigate: (path: string) => void;
};

const benefits = ['Instant Cashback', 'Personalized Voucher', 'No Minimum Balance', 'Online Payment'];

export default function LoginPage({ navigate }: LoginPageProps) {
    const [values, setValues] = useState<LoginRequest>({
        agentSupplierCode: '',
        email: '',
        password: ''
    });
    const [remember, setRemember] = useState(false);
    const [errors, setErrors] = useState<LoginErrors>({});
    const [serverError, setServerError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    function updateValue(field: keyof LoginRequest, value: string) {
        setValues(current => ({ ...current, [field]: value }));
        setErrors(current => ({ ...current, [field]: undefined }));
        setServerError('');
    }

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const validationErrors = validateLogin(values);

        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        setIsSubmitting(true);
        setServerError('');

        try {
            const response = await login(values);
            saveAuth(response, remember);
            navigate('/home');
        } catch (error) {
            setServerError(error instanceof Error ? error.message : 'Login could not be completed.');
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <main className="login-page">
            <header className="public-header">
                <div className="public-top-header">
                    <div className="container public-top-inner">
                        <a className="public-logo" href="/login" onClick={event => event.preventDefault()}>
                            <img src="/assets/DMCWale-logo.jpg" alt="DMCWale" />
                        </a>
                        <div className="public-top-right">
                            <div>(€) EUR</div>
                            <div>Help</div>
                            <button className="public-login-btn" type="button">
                                Login
                            </button>
                        </div>
                    </div>
                </div>

                <nav className="public-nav-wrap" aria-label="Public navigation">
                    <div className="container">
                        <ul className="public-main-nav">
                            {mainNavigationItems.map(item => (
                                <li className={item.children ? 'has-dropdown' : ''} key={item.label}>
                                    <a href="/login" onClick={event => event.preventDefault()}>
                                        <span className="nav-icon" aria-hidden="true">{item.icon}</span>
                                        {item.label}
                                    </a>
                                    {item.children ? (
                                        <ul className="dropdown">
                                            {item.children.map(child => (
                                                <li key={child}>
                                                    <a href="/login" onClick={event => event.preventDefault()}>
                                                        ◉ {child}
                                                    </a>
                                                </li>
                                            ))}
                                        </ul>
                                    ) : null}
                                </li>
                            ))}
                        </ul>
                    </div>
                </nav>
            </header>

            <section className="login-hero">
                <div className="login-overlay">
                    <div className="container login-content">
                        <div className="login-copy">
                            <h1>Go Somewhere you have never been Before!!</h1>
                            <p>
                                Largest B2B Marketplace connecting travel suppliers & partners with an extensive
                                inventory of worldwide tours, transfers and hotels to expand your business reach.
                            </p>
                            <div className="hero-buttons">
                                <Button type="button" variant="secondary">
                                    Register Supplier
                                </Button>
                                <Button type="button">Explore More</Button>
                            </div>
                        </div>

                        <form className="login-card" onSubmit={handleSubmit} noValidate>
                            <h2>Login</h2>

                            {serverError ? <div className="form-alert">{serverError}</div> : null}

                            <div className="login-form-group">
                                <input
                                    name="agentSupplierCode"
                                    value={values.agentSupplierCode}
                                    onChange={event => updateValue('agentSupplierCode', event.target.value)}
                                    placeholder="Agent/Supplier Code"
                                    autoComplete="organization"
                                    className={errors.agentSupplierCode ? 'has-error' : ''}
                                />
                                {errors.agentSupplierCode ? <span className="field-error">{errors.agentSupplierCode}</span> : null}
                            </div>

                            <div className="login-form-group">
                                <input
                                    name="email"
                                    value={values.email}
                                    onChange={event => updateValue('email', event.target.value)}
                                    placeholder="Email/User Name"
                                    autoComplete="username"
                                    className={errors.email ? 'has-error' : ''}
                                />
                                {errors.email ? <span className="field-error">{errors.email}</span> : null}
                            </div>

                            <div className="login-form-group">
                                <input
                                    name="password"
                                    type="password"
                                    value={values.password}
                                    onChange={event => updateValue('password', event.target.value)}
                                    placeholder="Password"
                                    autoComplete="current-password"
                                    className={errors.password ? 'has-error' : ''}
                                />
                                {errors.password ? <span className="field-error">{errors.password}</span> : null}
                            </div>

                            <div className="login-options">
                                <label>
                                    <input
                                        type="checkbox"
                                        checked={remember}
                                        onChange={event => setRemember(event.target.checked)}
                                    />
                                    Remember Me
                                </label>
                                <a href="/forgot-password" onClick={event => event.preventDefault()}>
                                    Forgot Password
                                </a>
                            </div>

                            <Button type="submit" className="submit-btn" disabled={isSubmitting}>
                                {isSubmitting ? <Loader /> : 'Login'}
                            </Button>
                        </form>
                    </div>
                </div>
            </section>

            <section className="benefit-section">
                <div className="container">
                    <h2>How it benefits our travel partners?</h2>
                    <div className="benefits">
                        {benefits.map(benefit => (
                            <div className="benefit-card" key={benefit}>
                                <h4>{benefit}</h4>
                            </div>
                        ))}
                    </div>
                </div>
            </section>
        </main>
    );
}
