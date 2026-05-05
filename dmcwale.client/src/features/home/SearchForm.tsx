import Button from '../../components/common/Button';
import Input from '../../components/common/Input';
import Select from '../../components/common/Select';
import type { SearchMode } from './HeroBanner';

const countOptions = ['0', '1', '2', '3', '4', '5', '6'].map(value => ({ label: value, value }));
const nationalityOptions = ['India', 'UAE', 'Vietnam', 'Thailand', 'Japan', 'Europe'].map(value => ({
    label: value,
    value
}));

type SearchFormProps = {
    mode: SearchMode;
};

export default function SearchForm({ mode }: SearchFormProps) {
    if (mode === 'activities') {
        return (
            <form className="search-panel activity-search-panel">
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

    return (
        <form className="search-panel package-search-panel">
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
