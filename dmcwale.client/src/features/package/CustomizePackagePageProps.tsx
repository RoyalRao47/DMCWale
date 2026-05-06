
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
export function formatDay(date: Date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}-${month}-${year}`;
}

