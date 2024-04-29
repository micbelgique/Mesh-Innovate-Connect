import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./Components/Home";
import CreateConference from "./Components/CreateConference";
import Conference from "./Components/Conference";

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/CreateConference" element={<CreateConference />} />
        <Route path="/Conference" element={<Conference />} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;