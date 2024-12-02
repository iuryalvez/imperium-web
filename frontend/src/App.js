import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import { AuthProvider } from './services/AuthContext';
import { NotificationProvider } from './services/NotificationContext';

import PrivateRoute from './components/servicesComponents/PrivateRoute';
import LoggedRoute from './components/servicesComponents/LoggedRoute';

import Header from './components/organisms/Header';

import Login from './pages/Login';
import Cadastro from './pages/AutoCadastro';
import Sobre from './pages/Sobre';
import Clinica from './pages/Clinica';
import Usuarios from './pages/Usuarios';
import ListaVinculos from './pages/ListaVinculos';

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
              <Route path="/sobre" Component={Sobre} />
              <Route path="/cadastro" Component={Cadastro} />
              {/* <Route path='/teste'element={<Teste/>} /> */}
              
              {/* Rotas protegidas por isLogged */}
              <Route element={<LoggedRoute />}>
                <Route path='/sistema/vinculos' element={<ListaVinculos />} />
              </Route>

              {/* Rotas protegidas dentro da PrivateRoute*/}
              <Route element={<PrivateRoute />}>
                <Route path='/clinica' element={<Clinica />} />
                <Route path='/clinica/usuarios' element={<Usuarios />} />
              </Route>
            </Routes>
        </AuthProvider>
      </NotificationProvider>
      
        {/* Rotas a ser implementadas
          <Route exact path="/" Component={Login} />
          <Route path="/dashboard" Component={Dashboard} /> 
          <Route path="/login" Component={Login} />
          <Route path="/tutores" Component={Tutores} />
          <Route path="/animais" Component={Animais} />
          <Route path="/animais/cadastrar" Component={CadastrarAnimal} />
          <Route path="/consultas" Component={Consultas} />
          <Route path="/gerenciamento" Component={Gerenciamento} />
          <Route path="/gerenciamento/usuarios" Component={Usuarios} />
          <Route path="/gerenciamento/perfil" Component={Perfil} /> 
        */}
      
    </Router>
  );
};

export default App;
