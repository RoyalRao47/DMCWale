export function getPackageDates(fromDate: string, toDate: string) {
    const start = fromDate ? new Date(`${fromDate}T00:00:00`) : new Date('2026-04-26T00:00:00');
    const end = toDate ? new Date(`${toDate}T00:00:00`) : new Date(start);
    const days: Date[] = [];

    for (let current = new Date(start); current <= end; current.setDate(current.getDate() + 1)) {
        days.push(new Date(current));
    }

    return days.length > 0 ? days : [start];
}
