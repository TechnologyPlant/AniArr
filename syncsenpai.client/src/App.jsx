import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './Components/Sidebar';
import './App.css'

import Home from './Pages/Home';
import AnilistWatchlist from './Pages/AnilistWatchlist';
import WatchList from './Pages/WatchList';
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
                        <Route path="/" element={<Home />} />
                        <Route path="/anilistWatchlist" element={<AnilistWatchlist />} />
                        <Route path="/watchlist" element={<WatchList />} />
                        <Route path="/configuration" element={<Configuration />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
