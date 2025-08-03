import { useEffect, useState } from 'react';
import WatchListKnownModalRequest from '../Components/WatchListKnownModalRequest';


// CSS for the grid container
const gridContainerStyles = {
    display: 'flex',
    gap: '20px',
    flexDirection: 'row',
    alignItems: 'center',
    textAlign: 'left'
};

export default function WatchlistUpdate() {

    const [existingWatchList, setExistingWatchList] = useState([]);
    const [newWatchList, setNewWatchList] = useState([]);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedWatchListItem, setSelectedWatchListItem] = useState('');
    async function LoadAnilistWatchlist() {
        await fetch('WatchListItem/new')
            .then(res => res.json())
            .then(async data => {
                setExistingWatchList([]);
                setNewWatchList([]);

                data.map(async watchListItem => {
                    const response = await fetch(`Sonarr/Series/${watchListItem.tvdbId}`, {
                        method: "Get",
                    });
                    if (response.ok) {
                        setExistingWatchList(prev => [...prev, watchListItem]);
                    } else {
                        setNewWatchList(prev => [...prev, watchListItem]);
                    }
                })

            });
    }

    useEffect(() => {
        return () => LoadAnilistWatchlist();
    }, []);

    function openModal(existingWatchListItem) {
        setSelectedWatchListItem(existingWatchListItem);
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
            <h2>Existing Sonarr Entries</h2>
            <div >
                {existingWatchList?.length > 0 ?
                    (
                        existingWatchList.map(watchListItem => (
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
            <h2>New Sonarr Entries</h2>
            <div >
                {newWatchList?.length > 0 ?
                    (
                        newWatchList.map(watchListItem => (
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