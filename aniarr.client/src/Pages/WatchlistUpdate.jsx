import { useEffect, useState } from 'react';
import '../App.css';
import WatchListKnownModalRequest from '../Components/WatchListKnownModalRequest';


// CSS for the grid container
const gridContainerStyles = {
    display: 'flex',
    gap: '20px',
    flexDirection: 'row',
    alignItems:'center',
    textAlign:'left'
};

export default function WatchlistUpdate() {

    const [watchList, setWatchList] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedWatchListItem, setSelectedWatchListItem] = useState('');
    async function LoadAnilistWatchlist() {
        await fetch('WatchListItem/new')
            .then(res => res.json())
            .then(data => setWatchList(data));
    }

    useEffect(() => {
        return () => LoadAnilistWatchlist();
    }, []);
    
    function openModal(watchListItem) {
        setSelectedWatchListItem(watchListItem);
        setIsModalOpen(true);
    }

    function closeModal() {
        setIsModalOpen(false);
    }
    async function closeModalAndRefresh() {
        closeModal();
        await LoadAnilistWatchlist();
    }
    return (
        <div>
            <h2>Anilist Watchlist</h2>
            <div>
                <button
                    onClick={() => LoadAnilistWatchlist()}
                >Refresh</button>
            </div>
            <div >
                {watchList?.length > 0 ?
                    (
                         watchList.map(watchListItem => (
                            <div key={watchListItem.id} style={{ ...gridContainerStyles }} onClick={() => openModal(watchListItem)}>
                            <span className="border-b py-2">{watchListItem.title}</span>
                                <ul className="mt-4">
                                    {watchListItem.aniListItems.map(aniListItem => (
                                        <li key={aniListItem.id} className="border-b py-2">{aniListItem.title}</li>
                                    ))}
                                </ul>
                            </div>
                        ))
                    ) : (<p>Loading or no items available.</p>)}
            </div>
            <WatchListKnownModalRequest isOpen={isModalOpen} onClose={closeModal} closeModalAndRefresh={closeModalAndRefresh} watchListItem={selectedWatchListItem} /> {/* Render the modal */}
        </div>
    );
}