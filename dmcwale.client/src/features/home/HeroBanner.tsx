import { useState } from 'react';
import SearchForm from './SearchForm';
import SearchTabs from './SearchTabs';
import * as React from 'react';

export type SearchMode = 'activities' | 'buildPackage';

type HeroBannerProps = {
    navigate: (path: string) => void;
};

export default function HeroBanner({ navigate }: HeroBannerProps) {
    const [activeMode, setActiveMode] = useState<SearchMode>('activities');

    return (
        <section className="home-hero">
            <div className="home-hero-overlay">
                <div className="container search-stack">
                    <div className="home-hero-title">
                        <h1>Find Next Place To Visit</h1>
                        <p>Discover amzaing places at exclusive deals</p>
                    </div>
                    <SearchTabs activeMode={activeMode} onModeChange={setActiveMode} />
                    <SearchForm mode={activeMode} navigate={navigate} />
                </div>
            </div>
        </section>
    );
}
