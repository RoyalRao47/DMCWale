import type { SelectHTMLAttributes } from 'react';
import * as React from 'react';

type SelectProps = SelectHTMLAttributes<HTMLSelectElement> & {
    label: string;
    options: Array<{ label: string; value: string }>;
};

export default function Select({ label, options, id, ...props }: SelectProps) {
    const selectId = id ?? props.name;

    return (
        <div className="field">
            <label htmlFor={selectId}>{label}</label>
            <select id={selectId} {...props}>
                {options.map(option => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
        </div>
    );
}
