
export type CustomizePackagePageProps = {
    navigate: (path: string) => void;
    queryString: string;
};
export const destinations = [
    'Hanoi',
    'Halong Bay',
    'Sapa',
    'Cat Ba Island',
    'Danang',
    'Bana Hills',
    'Hue',
    'Hoi an',
    'Phong Nha',
    'Nha Trang',
    'Dalat',
    'Ho Chi Minh',
    'Vung Tau',
    'Phu quoc'
];
export function formatDisplayDate(value: string) {
    if (!value) {
        return '';
    }

    const [year, month, day] = value.split('-');
    return `${day}-${month}-${year}`;
}
export function getPackageDates(fromDate: string, toDate: string) {
    const start = fromDate ? new Date(`${fromDate}T00:00:00`) : new Date('2026-04-26T00:00:00');
    const end = toDate ? new Date(`${toDate}T00:00:00`) : new Date(start);
    const days: Date[] = [];

    for (let current = new Date(start); current <= end; current.setDate(current.getDate() + 1)) {
        days.push(new Date(current));
    }

    return days.length > 0 ? days : [start];
}
export function formatDay(date: Date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}-${month}-${year}`;
}

