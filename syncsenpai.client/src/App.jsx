import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './Components/Sidebar';
import './App.css'

import AnilistWatchlist from './Pages/AnilistWatchlist';
import WatchList from './Pages/WatchList';
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
                        <Route path="/" element={<WatchList />} />
                        <Route path="/anilistWatchlist" element={<AnilistWatchlist />} />
                        <Route path="/watchlist" element={<WatchList />} />
                        <Route path="/configuration" element={<Configuration />} />
                        <Route path="/watchlistUpdate" element={<WatchlistUpdate />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
