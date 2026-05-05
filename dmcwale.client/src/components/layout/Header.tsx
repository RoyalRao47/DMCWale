import MainNavigation from './MainNavigation';
import TopHeader from './TopHeader';

type HeaderProps = {
    navigate: (path: string) => void;
};

export default function Header({ navigate }: HeaderProps) {
    return (
        <header>
            <TopHeader navigate={navigate} />
            <MainNavigation />
        </header>
    );
}
