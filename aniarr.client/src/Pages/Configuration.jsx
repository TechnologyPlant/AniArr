import { useState } from 'react';
import AnilistConfiguration from '../Components/AnilistConfiguration';
import FribbListConfiguration from '../Components/FribbListConfiguration';
import SonarrConfiguration from '../Components/SonarrConfiguration';

export default function Configuration() {
    const [activeTab, setActiveTab] = useState('anilist');

    return (
        <div>
            <Tabs activeTab={activeTab} setActiveTab={setActiveTab} />
            {activeTab === 'anilist' && <AnilistConfiguration />}
            {activeTab === 'fribbList' && <FribbListConfiguration />}
            {activeTab === 'sonarr' && <SonarrConfiguration />}
        </div>
    );
}

function Tabs({ activeTab, setActiveTab }) {
    return (
        <div>
            <button onClick={() => setActiveTab('anilist')} className={activeTab === 'anilist' ? 'active' : ''}>Anilist</button>
            <button onClick={() => setActiveTab('fribbList')} className={activeTab === 'fribbList' ? 'active' : ''}>FribbList</button>
            <button onClick={() => setActiveTab('sonarr')} className={activeTab === 'sonarr' ? 'active' : ''}>Sonarr</button>
        </div>
    );
}