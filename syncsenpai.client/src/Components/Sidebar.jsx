import React from 'react';
import { Link } from 'react-router-dom';

const Sidebar = () => {
    return (
        <div>
            <h2>My App</h2>
            <ul>
                <li><Link to="/">Home</Link></li>
                <li><Link to="/aniconfig">AniConfig</Link></li>
                <li><Link to="/watchlist">WatchList</Link></li>
            </ul>
        </div>
    );
};

export default Sidebar;
