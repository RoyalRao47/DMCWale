const serviceTabs = [
    'Activities',
    'Hotels',
    'Packages',
    'Transfer',
    'Visa',
    'Meal',
    'Build Your Package'
];

export default function SearchTabs() {
    return (
        <div className="service-tabs" role="tablist" aria-label="Services">
            {serviceTabs.map(tab => (
                <button
                    className={tab === 'Build Your Package' ? 'service-tab active' : 'service-tab'}
                    type="button"
                    role="tab"
                    aria-selected={tab === 'Build Your Package'}
                    key={tab}
                >
                    <span className="tab-icon" aria-hidden="true" />
                    {tab}
                </button>
            ))}
        </div>
    );
}
