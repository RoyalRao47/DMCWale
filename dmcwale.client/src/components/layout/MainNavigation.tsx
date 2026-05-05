const navItems = [
    'Home',
    'Activities',
    'Hotels',
    'Packages',
    'Transfer',
    'Visa',
    'Meals',
    'About Us',
    'Contact Us'
];

export default function MainNavigation() {
    return (
        <nav className="nav-wrap" aria-label="Main navigation">
            <div className="container">
                <ul className="main-nav">
                    {navItems.map(item => (
                        <li key={item}>
                            <a href="/home" onClick={event => event.preventDefault()}>
                                <span className="nav-icon" aria-hidden="true" />
                                {item}
                            </a>
                        </li>
                    ))}
                </ul>
            </div>
        </nav>
    );
}
