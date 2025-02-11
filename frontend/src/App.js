import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import { AuthProvider } from './services/AuthContext';
import { NotificationProvider } from './services/NotificationContext';


import LoggedRoute from './components/servicesComponents/LoggedRoute';
import AuthenticatedRoute from './components/servicesComponents/AuthenticatedRoute';

import Header from './components/organisms/Header';

import Login from './pages/Login.jsx';
import Cadastro from './pages/AutoCadastro';
import Sobre from './pages/Sobre';
import Clinica from './pages/Clinica.jsx';
import ListaUsuarios from './pages/ListaUsuarios.jsx';
import ListaUsuariosSistema from './pages/ListaUsuariosSistema.jsx';
import Tutores from './pages/Tutores.jsx';
import Gerenciamento from './pages/Gerenciamento.jsx';
import Consultas from './pages/Consultas.jsx';
import ListaVinculos from './pages/ListaVinculos';
import Home from './pages/Home.jsx';
import Animais from './pages/Animais.jsx';
import Sistema from './pages/Sistema.jsx';

const App = () => {
  return (
    <Router>
      <NotificationProvider>
        <AuthProvider>
          <Header />
            <Routes>
              {/* Rotas públicas */}
              <Route exact path='/' element={<Login />} />
              <Route path='/login' element={<Login />} />
              <Route path="/sobre" element={<Sobre />} />
              <Route path="/cadastro" element={<Cadastro />} />
              {/* <Route path='/teste'element={<Teste/>} /> */}
              
              {/* Rotas protegidas por isLogged */}
              <Route element={<LoggedRoute />}>
                <Route path='/sistema' element={<Sistema />} />
                <Route path='/sistema/vinculos' element={<ListaVinculos />} />
                <Route path='/sistema/clinicas' element={<Sistema />} />
                <Route path='/sistema/usuarios' element={<ListaUsuariosSistema />} />
              </Route>

              {/* Rotas protegidas dentro da AuthenticatedRoute */}
              <Route element={<AuthenticatedRoute />}>
                <Route path='/clinica/home' element={<Home />} />
                <Route path='/clinica' element={<Clinica />} />
                <Route path="/clinica/editar" element={<Gerenciamento />} />
                <Route path='/clinica/usuarios' element={<ListaUsuarios />} />
                <Route path="/clinica/tutores" element={<Tutores />} />
                <Route path="/clinica/consultas" element={<Consultas />} />
                <Route path="/clinica/animais" element={<Animais />} />
              </Route>
            </Routes>
        </AuthProvider>
      </NotificationProvider>
      
        {/* Rotas a ser implementadas
          <Route exact path="/" Component={Login} />
          <Route path="/dashboard" Component={Dashboard} /> 
          <Route path="/login" Component={Login} />
          <Route path="/animais" Component={Animais} />
          <Route path="/animais/cadastrar" Component={CadastrarAnimal} />
          <Route path="/gerenciamento/usuarios" Component={Usuarios} />
          <Route path="/gerenciamento/perfil" Component={Perfil} /> 
        */}
      
    </Router>
  );
};

export default App;
