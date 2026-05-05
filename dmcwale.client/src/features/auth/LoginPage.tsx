import { FormEvent, useState } from 'react';
import { login } from '../../api/authApi';
import Button from '../../components/common/Button';
import Input from '../../components/common/Input';
import Loader from '../../components/common/Loader';
import { saveAuth } from './authStore';
import type { LoginRequest } from './auth.types';
import { validateLogin, type LoginErrors } from './login.schema';

type LoginPageProps = {
    navigate: (path: string) => void;
};

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
            <section className="login-hero">
                <div className="login-overlay">
                    <div className="container login-content">
                        <div className="login-copy">
                            <img src="/assets/DMCWale-logo.jpg" alt="DMCWale" className="login-logo" />
                            <h1>Go somewhere you have never been before</h1>
                            <p>
                                A B2B travel workspace for partners to search services, build packages,
                                manage payments, and keep booking execution moving.
                            </p>
                        </div>

                        <form className="login-card" onSubmit={handleSubmit} noValidate>
                            <h2>Client Login</h2>

                            {serverError ? <div className="form-alert">{serverError}</div> : null}

                            <Input
                                label="Agent/Supplier Code"
                                name="agentSupplierCode"
                                value={values.agentSupplierCode}
                                onChange={event => updateValue('agentSupplierCode', event.target.value)}
                                error={errors.agentSupplierCode}
                                autoComplete="organization"
                            />

                            <Input
                                label="Email/User Name"
                                name="email"
                                value={values.email}
                                onChange={event => updateValue('email', event.target.value)}
                                error={errors.email}
                                autoComplete="username"
                            />

                            <Input
                                label="Password"
                                name="password"
                                type="password"
                                value={values.password}
                                onChange={event => updateValue('password', event.target.value)}
                                error={errors.password}
                                autoComplete="current-password"
                            />

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
        </main>
    );
}
