import * as React from 'react';
import Header from '../../components/layout/Header';
import HeroBanner from './HeroBanner';

type HomePageProps = {
    navigate: (path: string) => void;
};

export default function HomePage({ navigate }: HomePageProps) {
    return (
        <div className="home-page">
            <Header navigate={navigate} />
            <HeroBanner navigate={navigate} />
        </div>
    );
}
