export type NavigationItem = {
    label: string;
    icon: string;
    children?: string[];
};

export const mainNavigationItems: NavigationItem[] = [
    { label: 'Home', icon: '🏠' },
    {
        label: 'Activities',
        icon: '🎒',
        children: [
            'Half Day Tour',
            'Tour',
            'Show',
            'Island Tours',
            'Water Park',
            'Theme Park',
            'Transfer & Tickets',
            'Lunch & Dinner',
            'Full Day Tour',
            'Cruise',
            'Guide',
            'Transfer only',
            'Activity',
            'Tickets only'
        ]
    },
    { label: 'Hotels', icon: '🏢' },
    {
        label: 'Packages',
        icon: '🧳',
        children: ['Honeymoon', 'Group Tours', 'Family', 'Budget Friendly', 'Adventure', 'Weekend Getaways']
    },
    {
        label: 'Transfer',
        icon: '🚕',
        children: [
            'Airport to Halong Bay Transfers',
            'Airport to Hotel Transfer',
            'Disposal Car',
            'Halong Bay to Airport Transfer',
            'Hotel to Airport Transfers',
            'Hotel to Halong Bay Transfer',
            'One City to Another City Transfer',
            'One Hotel to Another Hotel Transfer',
            'Point to Point Transfer'
        ]
    },
    { label: 'Visa', icon: '▥' },
    { label: 'Meals', icon: '🍽' },
    { label: 'About Us', icon: 'ℹ' },
    { label: 'Contact Us', icon: '📞' }
];

export const serviceTabs = [
    { label: 'Activities', icon: '🎒' },
    { label: 'Hotels', icon: '🏢' },
    { label: 'Packages', icon: '🧳' },
    { label: 'Transfer', icon: '🚕' },
    { label: 'Visa', icon: '▥' },
    { label: 'Meal', icon: '🍽' },
    { label: 'Build Your Package', icon: '🧳' }
];
