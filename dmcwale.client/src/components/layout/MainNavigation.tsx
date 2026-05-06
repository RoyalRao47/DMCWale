import * as React from 'react';
import { mainNavigationItems } from './navigation.config';

export default function MainNavigation() {
    return (
        <nav className="nav-wrap" aria-label="Main navigation">
            <div className="container">
                <ul className="main-nav">
                    {mainNavigationItems.map(item => (
                        <li className={item.children ? 'has-dropdown' : ''} key={item.label}>
                            <a href="/home" onClick={event => event.preventDefault()}>
                                <span className="nav-icon" aria-hidden="true">{item.icon}</span>
                                {item.label}
                            </a>
                            {item.children ? (
                                <ul className="dropdown">
                                    {item.children.map(child => (
                                        <li key={child}>
                                            <a href="/home" onClick={event => event.preventDefault()}>
                                                ◉ {child}
                                            </a>
                                        </li>
                                    ))}
                                </ul>
                            ) : null}
                        </li>
                    ))}
                </ul>
            </div>
        </nav>
    );
}
