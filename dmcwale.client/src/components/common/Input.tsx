import type { InputHTMLAttributes } from 'react';
import * as React from 'react';

type InputProps = InputHTMLAttributes<HTMLInputElement> & {
    label: string;
    error?: string;
};

export default function Input({ label, error, id, className = '', ...props }: InputProps) {
    const inputId = id ?? props.name;

    return (
        <div className={`field ${className}`.trim()}>
            <label htmlFor={inputId}>{label}</label>
            <input id={inputId} className={error ? 'has-error' : ''} {...props} />
            {error ? <span className="field-error">{error}</span> : null}
        </div>
    );
}
