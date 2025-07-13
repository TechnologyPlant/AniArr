import AnilistConfiguration from '../Components/AnilistConfiguration';
import FribbListConfiguration from '../Components/FribbListConfiguration';
import SonarrConfiguration from '../Components/SonarrConfiguration';

export default function Configuration() {
    return (
        <div>
            <AnilistConfiguration />
            <FribbListConfiguration/>
            <SonarrConfiguration />
        </div>
    );
}