import { useEffect, useState } from 'react';
import React from 'react';
import { ToastContainer, toast } from 'react-toastify';

export default function SonarrConfiguration() {

    const [sonarrConnectionDetails, setSonarrConnectionDetails] = useState({
        host: "",
        port: "",
        apiKey: ""
    });
    const [isConnected, setIsConnected] = useState(false);

    // State for the full SonarrConfig returned from test
    const [fullSonarrConfig, setFullSonarrConfig] = useState({
        sonarrConnectionDetails: {
            host: "",
            port: "",
            apiKey: ""
        },
        sonarrTags: [],
        qualityProfiles: [],
        rootFolders: [],
        activeSonarrTag: null,
        activeQualityProfile: null,
        activeRootFolder: null
    });

    const testSonarrConfig = async () => {
        const saveConnectionDetails = await fetch('Sonarr/ConnectionDetails', {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(sonarrConnectionDetails)
        });

        if (saveConnectionDetails.ok) {

            const response = await fetch('Sonarr/ExternalDetails');
            if (response.ok) {
                const result = await response.json();
                // Update the full config with the populated lists and connection details
                setFullSonarrConfig({
                    sonarrConnectionDetails: {
                        host: result.sonarrConnectionDetails.host || "",
                        port: result.sonarrConnectionDetails.port || "",
                        apiKey: result.sonarrConnectionDetails.apiKey || ""
                    },
                    sonarrTags: result.sonarrTags || [],
                    qualityProfiles: result.qualityProfiles || [],
                    rootFolders: result.rootFolders || [],
                    activeSonarrTag: result.activeSonarrTag || null,
                    activeQualityProfile: result.activeQualityProfile || null,
                    activeRootFolder: result.activeRootFolder || null
                });
                toast.success('Test success - Configuration loaded');
                setIsConnected(true);
            }
            else {
                toast.error('Test failed - Check your Sonarr settings');
                setIsConnected(false);
            }
        }
        else {
            toast.error('Test failed - Check your Sonarr settings');
            setIsConnected(false);
        }
    }

    const saveSonarrConfig = async () => {
        setFullSonarrConfig(prev => ({ ...prev, sonarrConnectionDetails: sonarrConnectionDetails || null }));
        const response = await fetch('Sonarr/config', {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(fullSonarrConfig)
        });

        if (response.ok) {
            toast.success('Sonarr configuration saved');
        }
        else {
            toast.error('Failed to save Sonarr configuration');
        }
    }

    useEffect(() => {
        const getSonarrConfig = async () => {
            try {
                const response = await fetch('Sonarr/config');
                if (response.ok) {
                    const config = await response.json();
                    setSonarrConnectionDetails(config?.sonarrConnectionDetails);
                    setFullSonarrConfig({
                        sonarrConnectionDetails: {
                            host: config?.sonarrConnectionDetails?.host || "",
                            port: config?.sonarrConnectionDetails?.port || "",
                            apiKey: config?.sonarrConnectionDetails?.sonarrApiKey || ""
                        },
                        sonarrTags: config?.sonarrTags || [],
                        qualityProfiles: config?.qualityProfiles || [],
                        rootFolders: config?.rootFolders || [],
                        activeSonarrTag: config?.activeSonarrTag || null,
                        activeQualityProfile: config?.activeQualityProfile || null,
                        activeRootFolder: config?.activeRootFolder || null
                    });
                    setIsConnected(true);
                }
            } catch (error) {
                console.error('Failed to load Sonarr config:', error);
                setIsConnected(false);
            }
        }

        getSonarrConfig();
    }, [])

    const handleChange = (e) => {
        const { name, value } = e.target;
        setSonarrConnectionDetails(prev => ({ ...prev, [name]: value }));
        setFullSonarrConfig(prev => ({ ...prev,sonarrConnectionDetails: {...prev.sonarrConnectionDetails,[name]: value}}));
    };

    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        if (name === 'activeSonarrTag') {
            const selectedTag = fullSonarrConfig.sonarrTags.find(tag => tag.id.toString() === value.toString());
            setFullSonarrConfig(prev => ({ ...prev, activeSonarrTag: selectedTag || null }));
        } else if (name === 'activeQualityProfile') {
            const selectedProfile = fullSonarrConfig.qualityProfiles.find(profile => profile.id.toString() === value.toString());
            setFullSonarrConfig(prev => ({ ...prev, activeQualityProfile: selectedProfile || null }));
        } else if (name === 'activeRootFolder') {
            const selectedFolder = fullSonarrConfig.rootFolders.find(folder => folder.id.toString() === value.toString());
            setFullSonarrConfig(prev => ({ ...prev, activeRootFolder: selectedFolder || null }));
        }
    };

    // Validation function to check if all required fields are filled
    const isConnectionFormValid = () => {
        return (sonarrConnectionDetails?.host ?? "").trim() !== "" &&
            (sonarrConnectionDetails?.port ?? "").trim() !== "" &&
            (sonarrConnectionDetails?.apiKey ?? "").trim() !== "";
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
                <div>
                    <label>Host:</label>
                    <input
                        name="host"
                        value={sonarrConnectionDetails?.host || ""}
                        onChange={handleChange}
                        placeholder="Host"
                        required
                    />
                </div>
                <div>
                    <label>Port:</label>
                    <input
                        name="port"
                        value={sonarrConnectionDetails?.port || ""}
                        onChange={handleChange}
                        placeholder="Port"
                        required
                    />
                </div>
                <div>
                    <label>API Key:</label>
                    <input
                        name="apiKey"
                        value={sonarrConnectionDetails?.apiKey || ""}
                        onChange={handleChange}
                        placeholder="API Key"
                        required
                    />
                </div>

                <button
                    onClick={testSonarrConfig}
                    type="button"
                    disabled={!isConnectionFormValid()}
                >
                    Test Connection
                </button>

                {isConnected && (
                    <>
                        <div>
                            <label>Active Tag:</label>
                            <select
                                name="activeSonarrTag"
                                value={fullSonarrConfig.activeSonarrTag?.id || ''}
                                onChange={handleSelectChange}
                            >
                                <option value="">Select a tag</option>
                                {fullSonarrConfig.sonarrTags.map(tag => (
                                    <option key={tag.id} value={tag.id}>
                                        {tag.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label>Quality Profile:</label>
                            <select
                                name="activeQualityProfile"
                                value={fullSonarrConfig.activeQualityProfile?.id || ''}
                                onChange={handleSelectChange}
                            >
                                <option value="">Select a quality profile</option>
                                {fullSonarrConfig.qualityProfiles.map(profile => (
                                    <option key={profile.id} value={profile.id}>
                                        {profile.name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label>Root Folder:</label>
                            <select
                                name="activeRootFolder"
                                value={fullSonarrConfig.activeRootFolder?.id || ''}
                                onChange={handleSelectChange}
                            >
                                <option value="">Select a root folder</option>
                                {fullSonarrConfig.rootFolders.map(folder => (
                                    <option key={folder.id} value={folder.id}>
                                        {folder.path}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <button onClick={saveSonarrConfig} type="button">
                            Save Configuration
                        </button>
                    </>
                )}
            </form>
        </div>
    );
}