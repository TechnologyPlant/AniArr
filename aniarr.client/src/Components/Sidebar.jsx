import React from 'react';
import { Link } from 'react-router-dom';

const Sidebar = () => {
    return (
        <div>
            <h2>My App</h2>
            <ul>
                <li><Link to="/managedWatchlist">Managed Watchlist</Link></li>
                <li><Link to="/unmanagedWatchlist">Unmanaged Watchlist</Link></li>
                <li><Link to="/configuration">Configuration</Link></li>
            </ul>
        </div>
    );
};

export default Sidebar;
