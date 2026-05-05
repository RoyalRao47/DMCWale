import Button from '../../components/common/Button';
import Input from '../../components/common/Input';
import Select from '../../components/common/Select';

const countOptions = ['0', '1', '2', '3', '4', '5', '6'].map(value => ({ label: value, value }));
const nationalityOptions = ['India', 'UAE', 'Vietnam', 'Thailand', 'Japan', 'Europe'].map(value => ({
    label: value,
    value
}));

export default function SearchForm() {
    return (
        <form className="search-panel">
            <Input label="From" name="fromDate" type="text" defaultValue="26-04-2026" />
            <Input label="To" name="toDate" type="text" defaultValue="29-04-2026" />
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
