import { useEffect, useState } from 'react';
import './App.css';

export default function WatchList() {

    const [watchListEntries, setWatchListEntries] = useState();

    useEffect(() => {
        populateWatchList();
    }, []);


    const contents = watchListEntries === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Title</th>
                </tr>
            </thead>
            <tbody>
                {watchListEntries.map(entry =>
                    <tr key={entry.AniListId}>
                        <td>{entry.AniListTitle}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tableLabel">WatchList Entries</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );
    async function populateWatchList() {
        const response = await fetch('/watchlist');
        if (response.ok) {
            const data = await response.json();
            setWatchListEntries(data);
        }
    }
}