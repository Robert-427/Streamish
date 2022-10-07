import { useState } from "react";
import { VideoList } from "./VideoList";
import { VideoSearch } from "./VideoSearch";

export const VideoContainer = () => {
    const [searchTerms, setSearchTerms] = useState("")

    return <>
        <VideoSearch setterFunction={setSearchTerms} />
        <VideoList searchTermState={searchTerms} />
    </>
}
