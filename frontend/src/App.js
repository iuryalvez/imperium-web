import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Home from './components/pages/Home';
import Sobre from './components/pages/Sobre';
import Login from './components/pages/Login';
import Cadastro from './components/pages/Cadastro';
import Animais from './components/layout/Animais';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route exact path="/" Component={Home} />
        <Route path="/cadastro" Component={Cadastro} />
        <Route path="/login" Component={Login} />
        <Route path="/sobre" Component={Sobre} />
        <Route path="/home" Component={Home} />
        <Route path="/animais" Component={Animais} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
