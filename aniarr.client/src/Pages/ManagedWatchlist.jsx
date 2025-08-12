import { useEffect, useState } from 'react';
import React from 'react';


export default function ManagedWatchlist() {

    const [watchList, setWatchList] = useState([]);

    const LoadAnilistWatchlist = async () => {
        await fetch('WatchListItem?managed=true')
            .then(res => res.json())
            .then(data => setWatchList(data))
    }

    useEffect(() => {
        return () => LoadAnilistWatchlist();
    }, []);

    return (
        <div>
            <h2>Anilist Watchlist</h2>
            <div>
                <button
                    onClick={() => LoadAnilistWatchlist()}
                >Refresh</button>
            </div>
            <div>

                <div className="grid-container">
                    {watchList?.length > 0 ?
                        (
                            watchList.map(watchListItem => (
                                <>
                                    <div className="border-b py-2">{watchListItem.title}</div>
                                    <div>
                                        <ul className="mt-4">
                                            {watchListItem.aniListItems.map(aniListItem => (
                                                <li key={aniListItem.id} className="border-b py-2">{aniListItem.title}</li>
                                            ))}
                                        </ul>
                                    </div>
                                </>
                            ))
                        ) : (<p>Loading or no items available.</p>)}
                </div>
            </div>
        </div>
    );
}