import { useEffect, useState } from 'react';
import '../App.css';
import React from 'react';


export default function AnilistWatchlist() {

    const [watchList, setWatchList] = useState([]);

    const LoadAnilistWatchlist = async () => {
         await fetch('userwatchlist')
            .then(res => res.json())
            .then(data => setWatchList(data.data.MediaListCollection.lists))
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
                        watchList.map(list => (
                            <div>
                                <span className="border-b py-2">{list.name}</span>

                                <ul className="mt-4">
                                    {list.entries.map(entry => (
                                        <li className="border-b py-2">{entry.media.title.english}</li>
                                    ))}
                                </ul>
                            </div>
                        ))
                    ) : (<p>Loading or no items available.</p>)}
            </div>
        </div>
    );
}