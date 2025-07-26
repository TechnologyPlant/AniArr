import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './Components/Sidebar';

import AnilistWatchlist from './Pages/AnilistWatchlist';
import WatchlistUpdate from './Pages/WatchlistUpdate';
import Configuration from './Pages/Configuration';

function App() {
    return (
        <Router>
            <div className='app-layout'>
                <div className='sidebar' >
                <Sidebar />
                </div>
                <div className='main-content'>
                    <Routes>
                        <Route path="/" element={<AnilistWatchlist />} />
                        <Route path="/anilistWatchlist" element={<AnilistWatchlist />} />
                        <Route path="/configuration" element={<Configuration />} />
                        <Route path="/watchlistUpdate" element={<WatchlistUpdate />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
