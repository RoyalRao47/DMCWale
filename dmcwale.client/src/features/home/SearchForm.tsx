import type { FormEvent } from 'react';
import Button from '../../components/common/Button';
import Input from '../../components/common/Input';
import Select from '../../components/common/Select';
import type { SearchMode } from './HeroBanner';
import * as React from 'react';

const countOptions = ['0', '1', '2', '3', '4', '5', '6'].map(value => ({ label: value, value }));
const nationalityOptions = ['India', 'UAE', 'Vietnam', 'Thailand', 'Japan', 'Europe'].map(value => ({
    label: value,
    value
}));

type SearchFormProps = {
    mode: SearchMode;
    navigate: (path: string) => void;
};

export default function SearchForm({ mode, navigate }: SearchFormProps) {
    if (mode === 'activities') {
        return (
            <form className="search-panel activity-search-panel" onSubmit={event => event.preventDefault()}>
                <div className="destination-field">
                    <span className="destination-icon" aria-hidden="true">📍</span>
                    <input name="destination" placeholder="Destination" />
                </div>
                <input
                    className="date-search-input"
                    name="activityDate"
                    type="date"
                    defaultValue="2026-04-23"
                    aria-label="Activity date"
                />
                <Button type="submit" className="search-button">
                    Search
                </Button>
            </form>
        );
    }

    function handlePackageSearch(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const params = new URLSearchParams({
            fromDate: String(formData.get('fromDate') ?? ''),
            toDate: String(formData.get('toDate') ?? ''),
            adult: String(formData.get('adult') ?? '1'),
            childBelow4: String(formData.get('childBelow4') ?? '0'),
            childAbove4: String(formData.get('childAbove4') ?? '0'),
            nationality: String(formData.get('nationality') ?? 'India')
        });

        navigate(`/customize-package?${params.toString()}`);
    }

    return (
        <form className="search-panel package-search-panel" onSubmit={handlePackageSearch}>
            <Input label="From" name="fromDate" type="date" defaultValue="2026-04-26" />
            <Input label="To" name="toDate" type="date" defaultValue="2026-04-29" />
            <Select label="Adult" name="adult" defaultValue="1" options={countOptions} />
            <Select label="Chd Below (4 Yr)" name="childBelow4" defaultValue="0" options={countOptions} />
            <Select label="Chd Above (4-8 Yr)" name="childAbove4" defaultValue="0" options={countOptions} />
            <Select label="Nationality" name="nationality" defaultValue="India" options={nationalityOptions} />
            <Button type="submit" className="search-button">
                Search
            </Button>
        </form>
    );
}
