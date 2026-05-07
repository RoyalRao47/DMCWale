import * as React from 'react';
import Header from '../../components/layout/Header';

type ThemePreviewPageProps = {
    navigate: (path: string) => void;
};

type ThemeOption = {
    id: string;
    name: string;
    label: string;
    summary: string;
    bestFor: string;
    className: string;
    heroImage: string;
    destinations: Array<{
        name: string;
        meta: string;
        price: string;
        image: string;
    }>;
};

const themeOptions: ThemeOption[] = [
    {
        id: 'premium-travel',
        name: 'Premium Travel',
        label: 'Recommended',
        summary: 'A polished customer-facing travel portal with immersive destination imagery, clean search controls, and warm conversion buttons.',
        bestFor: 'Best for B2B agents who need a premium first impression and fast package search.',
        className: 'theme-premium-travel',
        heroImage: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1800&q=80',
        destinations: [
            {
                name: 'Dubai Luxury Escape',
                meta: '5 nights | Hotel + Transfers',
                price: 'From USD 740',
                image: 'https://images.unsplash.com/photo-1512453979798-5ea266f8880c?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Thailand Family Trail',
                meta: '6 nights | Activities included',
                price: 'From USD 620',
                image: 'https://images.unsplash.com/photo-1528181304800-259b08848526?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Vietnam Group Deal',
                meta: '4 nights | Agent special',
                price: 'From USD 410',
                image: 'https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=700&q=80'
            }
        ]
    },
    {
        id: 'bold-holiday',
        name: 'Bold Holiday',
        label: 'Most eye catching',
        summary: 'A colorful holiday marketplace look with strong offers, lively cards, and high-energy calls to action.',
        bestFor: 'Best when you want agents to notice deals, promotions, and seasonal packages immediately.',
        className: 'theme-bold-holiday',
        heroImage: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1800&q=80',
        destinations: [
            {
                name: 'Japan Cherry Season',
                meta: '7 nights | Limited seats',
                price: 'Save 12%',
                image: 'https://images.unsplash.com/photo-1522383225653-ed111181a951?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Europe Summer Route',
                meta: '9 nights | Multi-city',
                price: 'Hot deal',
                image: 'https://images.unsplash.com/photo-1467269204594-9661b134dd2b?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Dubai Theme Parks',
                meta: '3 nights | Family offer',
                price: 'Kids special',
                image: 'https://images.unsplash.com/photo-1582672060674-bc2bd808a8b5?auto=format&fit=crop&w=700&q=80'
            }
        ]
    },
    {
        id: 'clean-agent',
        name: 'Clean Agent Desk',
        label: 'Easiest to use',
        summary: 'A practical agent-first interface with compact filters, clear inventory results, and restrained colors.',
        bestFor: 'Best for daily users who compare hotels, transfers, activities, and package prices all day.',
        className: 'theme-clean-agent',
        heroImage: 'https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=1800&q=80',
        destinations: [
            {
                name: 'Bangkok City Break',
                meta: 'Inventory matched | 18 hotels',
                price: 'USD 388',
                image: 'https://images.unsplash.com/photo-1563492065599-3520f775eeed?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Halong Bay Add-on',
                meta: 'Manual fallback ready',
                price: 'USD 126',
                image: 'https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Airport Transfers',
                meta: 'Private sedan | Van | Coach',
                price: 'USD 32',
                image: 'https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?auto=format&fit=crop&w=700&q=80'
            }
        ]
    },
    {
        id: 'luxury-dark',
        name: 'Luxury Dark Hero',
        label: 'Premium look',
        summary: 'A dramatic travel storefront with rich imagery, glass-like search, and strong destination storytelling.',
        bestFor: 'Best for luxury packages and high-value travel products where the visual first impression matters.',
        className: 'theme-luxury-dark',
        heroImage: 'https://images.unsplash.com/photo-1499678329028-101435549a4e?auto=format&fit=crop&w=1800&q=80',
        destinations: [
            {
                name: 'Maldives Style Villa',
                meta: 'Private stay | Honeymoon',
                price: 'USD 1,240',
                image: 'https://images.unsplash.com/photo-1514282401047-d79a71a590e8?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Dubai Premium Suite',
                meta: '5 star | Private transfer',
                price: 'USD 980',
                image: 'https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=700&q=80'
            },
            {
                name: 'Europe Signature Tour',
                meta: 'Curated itinerary',
                price: 'USD 1,760',
                image: 'https://images.unsplash.com/photo-1502602898657-3e91760cbb34?auto=format&fit=crop&w=700&q=80'
            }
        ]
    }
];

export default function ThemePreviewPage({ navigate }: ThemePreviewPageProps) {
    return (
        <div className="theme-preview-page">
            <Header navigate={navigate} />

            <main>
                <section className="theme-preview-intro">
                    <div className="container theme-preview-intro-inner">
                        <div>
                            <span className="theme-eyebrow">React frontend theme preview</span>
                            <h1>Choose the customer-facing travel design direction</h1>
                            <p>
                                Header and navigation stay the same. These previews focus on the public frontend body:
                                hero, search, destination cards, package highlights, and agent-friendly content sections.
                            </p>
                        </div>
                        <div className="theme-jump-list" aria-label="Theme options">
                            {themeOptions.map(theme => (
                                <a key={theme.id} href={`#${theme.id}`}>
                                    {theme.name}
                                </a>
                            ))}
                        </div>
                    </div>
                </section>

                {themeOptions.map(theme => (
                    <section className={`frontend-theme-option ${theme.className}`} id={theme.id} key={theme.id}>
                        <div className="container">
                            <div className="frontend-theme-heading">
                                <div>
                                    <span>{theme.label}</span>
                                    <h2>{theme.name}</h2>
                                    <p>{theme.summary}</p>
                                </div>
                                <strong>{theme.bestFor}</strong>
                            </div>

                            <div className="theme-browser-frame">
                                <div className="theme-browser-bar">
                                    <i></i>
                                    <i></i>
                                    <i></i>
                                    <span>dmcwale.com/home</span>
                                </div>

                                <div
                                    className="theme-live-hero"
                                    style={{ backgroundImage: `linear-gradient(var(--theme-hero-tint), var(--theme-hero-tint)), url("${theme.heroImage}")` }}
                                >
                                    <div className="theme-live-copy">
                                        <span>Vietnam | Dubai | Thailand | Europe | Japan</span>
                                        <h3>Build faster travel packages with verified rates</h3>
                                        <p>
                                            Search activities, hotels, transfers, and custom packages from one clean travel workspace.
                                        </p>
                                    </div>

                                    <form className="theme-search-card" onSubmit={event => event.preventDefault()}>
                                        <label>
                                            Destination
                                            <input defaultValue="Dubai" aria-label="Destination" />
                                        </label>
                                        <label>
                                            Travel date
                                            <input defaultValue="2026-04-26" type="date" aria-label="Travel date" />
                                        </label>
                                        <label>
                                            Guests
                                            <select defaultValue="2 Adults" aria-label="Guests">
                                                <option>2 Adults</option>
                                                <option>4 Adults</option>
                                                <option>Group</option>
                                            </select>
                                        </label>
                                        <button type="submit">Search</button>
                                    </form>
                                </div>

                                <div className="theme-content-band">
                                    <div className="theme-section-title">
                                        <div>
                                            <span>Featured inventory</span>
                                            <h4>Ready-to-sell travel products</h4>
                                        </div>
                                        <button type="button">View all</button>
                                    </div>

                                    <div className="theme-destination-grid">
                                        {theme.destinations.map(destination => (
                                            <article className="theme-destination-card" key={destination.name}>
                                                <img src={destination.image} alt="" />
                                                <div>
                                                    <span>{destination.meta}</span>
                                                    <h5>{destination.name}</h5>
                                                    <strong>{destination.price}</strong>
                                                </div>
                                            </article>
                                        ))}
                                    </div>

                                    <div className="theme-support-grid">
                                        <div>
                                            <span>Package Builder</span>
                                            <strong>Day-wise itinerary with hotels, transfers, activities, and manual fallback.</strong>
                                        </div>
                                        <div>
                                            <span>Agent Tools</span>
                                            <strong>Quotation versions, payment links, vouchers, and booking status tracking.</strong>
                                        </div>
                                        <div>
                                            <span>Supplier Ready</span>
                                            <strong>Paid bookings can move cleanly into execution and confirmation workflows.</strong>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                ))}
            </main>
        </div>
    );
}
