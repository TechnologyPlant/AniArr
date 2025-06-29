import { useEffect, useState } from 'react';
import './App.css';
import React from 'react';


export default function AniConfig() {

    const [username, setUsername] = useState('');
    const [watchList, setWatchList] = useState([]);


    useEffect(() => {
        const delayDebounce = setTimeout(() => {
            if (username.trim() !== '') {
                fetch(`userwatchlist/${username}`)
                    .then(res =>res.json())
                    .then(data => setWatchList(data.data.MediaListCollection.lists))
                    .catch(err => console.error('Fetch error:', err));
            } else {
                setWatchList([]); // Clear results if input is empty
            }
        }, 500); // debounce to avoid too many requests

        return () => clearTimeout(delayDebounce);
    }, [username]);

    return (
        <div>
            <div>
                <input
                    type="text"
                    onChange={(e) => setUsername(e.target.value)}
                    value={username}
                    placeholder="Set anilist username'"
                />
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
                    ): (<p>Loading or no items available.</p>)}
            </div>
        </div>
    );
}