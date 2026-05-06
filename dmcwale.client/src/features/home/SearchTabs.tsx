import * as React from 'react';
import { serviceTabs } from '../../components/layout/navigation.config';
import type { SearchMode } from './HeroBanner';

type SearchTabsProps = {
    activeMode: SearchMode;
    onModeChange: (mode: SearchMode) => void;
};

function getMode(label: string): SearchMode | null {
    if (label === 'Activities') {
        return 'activities';
    }

    if (label === 'Build Your Package') {
        return 'buildPackage';
    }

    return null;
}

export default function SearchTabs({ activeMode, onModeChange }: SearchTabsProps) {
    return (
        <div className="service-tabs" role="tablist" aria-label="Services">
            {serviceTabs.map(tab => {
                const mode = getMode(tab.label);
                const isActive = mode === activeMode;
                const isClickable = mode !== null;
                const className = [
                    'service-tab',
                    isActive ? 'active' : '',
                    mode === null ? 'disabled' : '',
                    isClickable ? 'clickable' : ''
                ].filter(Boolean).join(' ');

                return (
                    <button
                        className={className}
                        type="button"
                        role="tab"
                        aria-selected={isActive}
                        aria-disabled={mode === null}
                        disabled={mode === null}
                        onClick={mode ? () => onModeChange(mode) : undefined}
                        key={tab.label}
                    >
                        <span className="tab-icon" aria-hidden="true">{tab.icon}</span>
                        {tab.label}
                    </button>
                );
            })}
        </div>
    );
}
