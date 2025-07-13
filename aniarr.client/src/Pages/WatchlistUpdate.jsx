import { useEffect, useState } from 'react';
import '../App.css';
import React from 'react';


export default function WatchlistUpdate() {

    const [watchList, setWatchList] = useState([]);

    const LoadAnilistWatchlist = async () => {
        await fetch('userwatchlistupdate')
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
                        watchList.map(list => (
                            <div>
                                <h3 className="border-b py-2">{list.title}</h3>
                                <ul className="mt-4">
                                    {
                                        list.aniListItems.map(item => 
                                            <li className="border-b py-2">{item.title}</li>
                                        )
                                    }
                                </ul>
                            </div>
                        ))
                    ) : (<p>Loading or no items available.</p>)}
            </div>
        </div>
    );
}