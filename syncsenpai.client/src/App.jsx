import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './Components/Sidebar';
import './App.css'

import Home from './Pages/Home';
import AniConfig from './Pages/AniConfig';
import WatchList from './Pages/WatchList';

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
                        <Route path="/aniconfig" element={<AniConfig />} />
                        <Route path="/watchlist" element={<WatchList />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
