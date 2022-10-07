import React from "react";
import "./App.css";
import { VideoContainer } from "./components/VideoContainer";

function App() {
  return (
    <div className="App">
      <h1>Welcome to the video list</h1>
      <VideoContainer />
    </div>
  );
}

export default App;