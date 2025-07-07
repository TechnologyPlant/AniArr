import { useEffect, useState } from 'react';
import '../App.css';
import React from 'react';
import { ToastContainer, toast } from 'react-toastify';

export default function SonarrConfiguration() {

    const [sonarrConfig, setSonarrConfig] = useState({
        host: "",
        port: "",
        apiKey: ""
    });
    const [isConnected, setIsConnected] = useState(false);

    const saveUsername = async () => {
        const response = await fetch('AnilistConfig', {
            method: "PATCH",
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

    const testSonarrConfig = async () => {
        const response = await fetch('SonarrConfig/test', {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(sonarrConfig)
        });

        if (response.ok) {
            toast('Test success');
            setIsConnected(true);
        }

        else {
            toast('Test fail');
            setIsConnected(false);
        }

    }

    const saveSonarrConfig = async () => {
        const response = await fetch('SonarrConfig', {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(sonarrConfig)
        });

        if (response.ok) {
            toast('Saved SonarrConfig');
        }
        else {
            toast('Failed to save SonarrConfig');
        }

    }


    useEffect(() => {
        const getSonarrConfig = async () => {
            await fetch('SonarrConfig')
                .then(res => res.json())
                .then(config => {
                    setSonarrConfig(config);
                    setIsConnected(true);
                })
        }

        getSonarrConfig();
    }, [])

    const handleChange = (e) => {
        const { name, value } = e.target;
        setSonarrConfig(prev => ({ ...prev, [name]: value }));
    };
    return (
        <div>
            <ToastContainer
                position="bottom-right"
            />
            <div>
                <h2>Sonarr Configuration</h2>
            </div>
            <form>
                <input
                    name="host"
                    value={sonarrConfig.host}
                    onChange={handleChange}
                    placeholder="Host"
                    required
                />
                <input
                    name="port"
                    value={sonarrConfig.port}
                    onChange={handleChange}
                    placeholder="Port"
                    required
                />
                <input
                    name="apiKey"
                    value={sonarrConfig.apiKey}
                    onChange={handleChange}
                    placeholder="API Key"
                    required
                />

                <button onClick={testSonarrConfig} type="button">
                    Test
                </button>

            </form>
        </div>
    );
}