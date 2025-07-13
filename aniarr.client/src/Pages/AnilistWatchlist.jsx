import { useEffect, useState } from 'react';
import '../App.css';
import React from 'react';


export default function AnilistWatchlist() {

    const [watchList, setWatchList] = useState([]);

    const LoadAnilistWatchlist = async () => {
        await fetch('WatchListItem')
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
                {watchList?.length > 0 ?
                    (
                        watchList.map(watchListItem => (
                            <div>
                                <span className="border-b py-2">{watchListItem.title}</span>

                                <ul className="mt-4">
                                    {watchListItem.aniListItems.map(aniListItem => (
                                        <li className="border-b py-2">{aniListItem.title}</li>
                                    ))}
                                </ul>
                            </div>
                        ))
                    ) : (<p>Loading or no items available.</p>)}
            </div>
        </div>
    );
}