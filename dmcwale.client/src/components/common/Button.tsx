import type { ButtonHTMLAttributes, ReactNode } from 'react';
import * as React from 'react';

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
    children: ReactNode;
    variant?: 'primary' | 'secondary' | 'ghost';
};

export default function Button({ children, variant = 'primary', className = '', ...props }: ButtonProps) {
    return (
        <button className={`btn btn-${variant} ${className}`.trim()} {...props}>
            {children}
        </button>
    );
}
