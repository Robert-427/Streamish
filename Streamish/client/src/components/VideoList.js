import React, { useEffect, useState } from "react";
import Video from './Video';
import { getAllVideosWithComments } from "../modules/videoManager";

export const VideoList = ({searchTermState}) => {
  const [videos, setVideos] = useState([]);
  const [filteredVideos, setFilteredVideos] = useState([]);

  const getVideos = () => {
    getAllVideosWithComments().then(videos => setVideos(videos));
    getAllVideosWithComments().then(videos => setFilteredVideos(videos));
  };

  useEffect(() => {
    getVideos();
  }, []);

  useEffect(
    () => {
        const searchedVideos = videos.filter(video => {
            return video.title.toLowerCase().startsWith(searchTermState.toLowerCase())
        })
        setFilteredVideos(searchedVideos)
    },
    [ searchTermState ]
  )

  return (
    <div className="container">
      <div className="row justify-content-center">
        {filteredVideos.map((video) => (
          <Video video={video} key={video.id} commentArray={video.comments}/>
        ))}
      </div>
    </div>
  );
};

