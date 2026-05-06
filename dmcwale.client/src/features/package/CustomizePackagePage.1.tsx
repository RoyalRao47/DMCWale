import * as React from 'react';
import Header from '../../components/layout/Header';
import { CustomizePackagePageProps, formatDisplayDate, formatDay, destinations } from './CustomizePackagePageProps';
import { getPackageDates } from "./getPackageDates";


export default function CustomizePackagePage({ navigate, queryString }: CustomizePackagePageProps) {
    const params = new URLSearchParams(queryString);
    const fromDate = params.get('fromDate') ?? '2026-04-26';
    const toDate = params.get('toDate') ?? '2026-04-29';
    const adults = Number(params.get('adult') ?? '1');
    const childAbove = Number(params.get('childAbove4') ?? '0');
    const childBelow = Number(params.get('childBelow4') ?? '0');
    const totalPax = adults + childAbove + childBelow;
    const days = getPackageDates(fromDate, toDate);

    return (
        <div className="app-page">
            <Header navigate={navigate} />
            <main className="workspace-page package-workspace">
                <div className="container package-grid">
                    <section className="package-main">
                        <div className="package-summary">
                            <div className="package-name-box">
                                <span>Package Name</span>
                                <strong>Build Your Package</strong>
                            </div>
                            <div className="package-stat">
                                <span>From Date</span>
                                <strong>{formatDisplayDate(fromDate)}</strong>
                            </div>
                            <div className="package-stat">
                                <span>To Date</span>
                                <strong>{formatDisplayDate(toDate)}</strong>
                            </div>
                            <div className="package-stat">
                                <span>Adults</span>
                                <strong className="count-pill green">{adults}</strong>
                            </div>
                            <div className="package-stat">
                                <span>Child Above (4-8 Yrs)</span>
                                <strong className="count-pill amber">{childAbove}</strong>
                            </div>
                            <div className="package-stat">
                                <span>Child Below 4 Yrs</span>
                                <strong className="count-pill green">{childBelow}</strong>
                            </div>
                            <div className="package-stat">
                                <span>Total Pax</span>
                                <strong>{totalPax}</strong>
                            </div>
                        </div>

                        <div className="itinerary-card">
                            {days.map((day, index) => (
                                <div className="itinerary-row" key={day.toISOString()}>
                                    <div className="day-label">Day {index + 1} - {formatDay(day)}</div>
                                    <select defaultValue="">
                                        <option value="">Select Destination</option>
                                        {destinations.map(destination => (
                                            <option key={destination} value={destination}>{destination}</option>
                                        ))}
                                    </select>
                                    <button type="button" className="action indigo">Add Day Details</button>
                                    <button type="button" className="action teal">Add Transfer</button>
                                    <button type="button" className="action green">Add Sightseeing</button>
                                    <button type="button" className="action purple">Add Hotel</button>
                                    {index === 0 ? <button type="button" className="action black">Add Visa</button> : null}
                                </div>
                            ))}
                        </div>

                        <div className="agent-package-form">
                            <label>
                                DMC Code
                                <input placeholder="Enter Package" />
                            </label>
                            <label>
                                Travel Agency Name
                                <input placeholder="Enter Package" />
                            </label>
                            <label>
                                City Name
                                <input placeholder="Enter Package" />
                            </label>
                            <label>
                                Agent Number
                                <input placeholder="Enter Agent" />
                            </label>
                            <label>
                                Agent Email
                                <input placeholder="Enter Email" />
                            </label>
                            <label>
                                Markup%
                                <input defaultValue="0" />
                            </label>
                            <button type="button">Save</button>
                        </div>
                    </section>

                    <aside className="package-sidebar">
                        <div className="cost-card">
                            <strong>0.00 USD</strong>
                            <span>Total Package Cost</span>
                        </div>
                        <button type="button" className="download red">Download PDF</button>
                        <button type="button" className="download blue">Download Word</button>
                        <div className="send-mail-card">
                            <label>
                                Email
                                <input placeholder="Email" />
                            </label>
                            <button type="button">Send Mail</button>
                        </div>
                    </aside>
                </div>
            </main>
        </div>
    );
}
