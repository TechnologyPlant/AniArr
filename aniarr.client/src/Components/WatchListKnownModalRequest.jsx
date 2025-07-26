import React, { useState, useEffect } from "react";
const WatchListKnownModalRequest = ({
  isOpen,
  onClose,
  closeModalAndRefresh,
  watchListItem,
}) => {
  if (!isOpen) return null;

  const [lookingUp, setLookingUp] = useState(false);
  const [lookupDetails, setLookupDetails] = useState({
    folder: "",
    monitored: false,
    seasons: [],
    title: "",
    tvdbId: 0,
    year: 0,
  });

  useEffect(() => {
    setLookupDetails(null);
    fetchAdditionalData();
  }, []);

  function handleClose() {
    onClose();
  }
  async function handleCloseModalAndRefresh() {
    await closeModalAndRefresh();
  }
  async function acknowledge() {
    const response = await fetch("WatchListItem", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(watchListItem),
    });
    if (response.ok) {
      alert("Saved watchlist items");
      await handleCloseModalAndRefresh();
    } else {
      alert("Error saving watchlist items");
    }
  }

  async function request() {
    const sonarrConfigResponse = await fetch("Sonarr/config");
    if (sonarrConfigResponse.ok) {
      const sonarrConfig = await sonarrConfigResponse.json();

      const sonarrRequest = {
        QualityProfileId: sonarrConfig.activeQualityProfile.id,
        RootFolderPath: sonarrConfig.ActiveRootFolder,
        tvdbId: lookupDetails.tvdbId,
        seasons: lookupDetails.seasons,
      };

      const requestResponse = await fetch("Sonarr/Request", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(sonarrRequest),
      });
      if (requestResponse.ok) {
        alert("Saved watchlist items");
        await acknowledge();
      } else {
        alert("Error saving watchlist items");
      }
    } else {
      alert("Error loading sonarr config");
    }
  }
  async function fetchAdditionalData() {
    try {
      setLookingUp(true);
      const response = await fetch(`/Sonarr/Lookup/${watchListItem.title}`);
      const data = await response.json();
      setLookupDetails(data);
      console.log(data);
    } catch (error) {
      setLookupDetails(null);
      console.error("Error fetching additional data:", error);
    } finally {
      setLookingUp(false);
    }
  }

  function handleSeasonCheckboxChange(seasonNumber) {
    setLookupDetails((lookupDetail) => ({
      ...lookupDetail,
      seasons: lookupDetail.seasons.map((season) =>
        season.seasonNumber == seasonNumber
          ? { ...season, monitored: !season.monitored }
          : season
      ),
    }));
  }

  function handleMonitoredCheckboxChange() {
    setLookupDetails((lookupDetail) => ({
      ...lookupDetail,
      monitored: !lookupDetail.monitored,
    }));
  }
  return (
    <div
      className="modal-background"
      style={{
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        display: isOpen ? "flex" : "none",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <div
        className="modal-panel"
        style={{ padding: "20px", borderRadius: "8px" }}
      >
        <h3>{watchListItem.title}</h3>
        <button onClick={fetchAdditionalData}>Fetch Data</button>

        {lookupDetails && (
          <div>
            <div>
              <span>Title: {lookupDetails.title}</span>
            </div>
            <div>
              <span>Year: {lookupDetails.year}</span>
            </div>
            <div>
              <span>Monitored? {lookupDetails.monitored}</span>
              <input
                type="checkbox"
                checked={lookupDetails.monitored}
                onChange={() => handleMonitoredCheckboxChange()}
              />
            </div>
            <div>
              {lookupDetails.seasons.map((season) => (
                <div>
                  <span>Season {season.seasonNumber}</span>
                  <input
                    key={season.seasonNumber}
                    type="checkbox"
                    checked={season.monitored}
                    onChange={() =>
                      handleSeasonCheckboxChange(season.seasonNumber)
                    }
                  />
                </div>
              ))}
            </div>
            <button onClick={acknowledge}>Acknowledge Entry</button>
            <button onClick={request}>Submit Request</button>
          </div>
        )}
        <button onClick={handleClose}>Close</button>
      </div>
    </div>
  );
};

export default WatchListKnownModalRequest;
