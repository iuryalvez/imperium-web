import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import ProtectedRoute from './components/ProtectedRoute';

import Home from './pages/Home';
import Sobre from './pages/Sobre';
import Login from './pages/Login';
import Cadastro from './pages/Cadastro';
import Dashboard from './pages/Dashboard';
import Tutores from './pages/Tutores';
import Animais from './pages/Animais';
import CadastrarAnimal from './pages/CadastrarAnimal';
import Consultas from './pages/Consultas';
import Gerenciamento from './pages/Gerenciamento';
import Usuarios from './pages/Usuarios';
import Perfil from './pages/Perfil';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route exact path="/" Component={Login} />
        <Route path="/cadastro" Component={Cadastro} />
        <Route path="/login" Component={Login} />
        <Route path="/sobre" Component={Sobre} />
        <Route path="/home" Component={Home} />
        <Route path="/dashboard" Component={Dashboard} />
        <Route path="/tutores" Component={Tutores} />
        <Route path="/animais" Component={Animais} />
        <Route path="/animais/cadastrar" Component={CadastrarAnimal} />
        <Route path="/consultas" Component={Consultas} />
        <Route path="/gerenciamento" Component={Gerenciamento} />
        <Route path="/gerenciamento/usuarios" Component={Usuarios} />
        <Route path="/gerenciamento/perfil" Component={Perfil} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
