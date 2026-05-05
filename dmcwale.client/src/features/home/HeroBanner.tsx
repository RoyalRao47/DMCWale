import SearchForm from './SearchForm';
import SearchTabs from './SearchTabs';

export default function HeroBanner() {
    return (
        <section className="home-hero">
            <div className="home-hero-overlay">
                <div className="container search-stack">
                    <SearchTabs />
                    <SearchForm />
                </div>
            </div>
        </section>
    );
}
