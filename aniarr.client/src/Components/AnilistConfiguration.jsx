import { useEffect, useState } from 'react';
import React from 'react';
import { ToastContainer, toast } from 'react-toastify';

export default function AnilistConfiguration() {

    const [username, setUsername] = useState('');

    const saveUsername = async () => {
        const response = await fetch('AnilistConfig', {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(username)
        });

        if (response.ok) {
            toast('Saved Anilist username');
        }
        else {
            toast('Failed to save Anilist username');
        }
    }


    useEffect(() => {
        const getUsername = async () => {
            await fetch('AnilistConfig')
                .then(res => res.json())
                .then(config => setUsername(config.userName))
        }

        getUsername();
    }, [])

    return (
        <div>
            <ToastContainer
            position="bottom-right"
            />
            <div>
            <h2>Anilist Configuration</h2>
            </div>
            <div>
                <input
                    type="text"
                    onChange={(e) => setUsername(e.target.value)}
                    value={username}
                    placeholder="Set anilist username"
                />
            </div>
            <div>
                <button
                    onClick={()=>saveUsername()}
                >Save user</button>
            </div>
        </div>
    );
}