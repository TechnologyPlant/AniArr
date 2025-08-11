import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './Components/Sidebar';

import ManagedWatchlist from './Pages/ManagedWatchlist';
import UnmanagedWatchlist from './Pages/UnmanagedWatchlist';
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
                        <Route path="/" element={<ManagedWatchlist />} />
                        <Route path="/managedWatchlist" element={<ManagedWatchlist />} />
                        <Route path="/configuration" element={<Configuration />} />
                        <Route path="/unmanagedWatchlist" element={<UnmanagedWatchlist />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
