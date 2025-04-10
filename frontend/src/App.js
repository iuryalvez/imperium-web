import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import { AuthProvider } from './services/AuthContext';
import { NotificationProvider } from './services/NotificationContext';

import LoggedRoute from './components/servicesComponents/LoggedRoute';
import AuthenticatedRoute from './components/servicesComponents/AuthenticatedRoute';

import Header from './components/organisms/Header/index.jsx';

import Login from './pages/Login/index.jsx';
import Sobre from './pages/Sobre/index.jsx';

import Sistema from './pages/Sistema/SistemaInicio/index.jsx';
import SistemaVinculos from './pages/Sistema/ListaDeVinculos/index.jsx';
import SistemaUsuarios from './pages/Sistema/Usuarios/index.jsx';
import SistemaClinicas from './pages/Sistema/Clinicas/index.jsx';

import Clinica from './pages/Clinica/ClinicaInicio/index.jsx';
import ClinicaInicio from './pages/Clinica/ClinicaInicio/index.jsx';
import ClinicaGerenciamento from './pages/Clinica/ClinicaGerenciamento/index.jsx';
import UsuariosDaClinica from './pages/Clinica/UsuariosDaClinica/index.jsx';
import TutoresDaClinica from './pages/Clinica/TutoresDaClinica/index.jsx';
import ConsultasDaClinica from './pages/Clinica/ConsultasDaClinica/index.jsx';
import AnimaisDaClinica from './pages/Clinica/AnimaisDaClinica/index.jsx';
import AdminRoute from './components/servicesComponents/AdminRoute.js';

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
            {/* <Route path="/cadastro" element={<Cadastro />} /> */}
            {/* <Route path='/teste'element={<Teste/>} /> */}
            
            {/* Rotas protegidas por isLogged */}
            <Route element={<LoggedRoute />}>
              <Route path='/sistema/vinculos' element={<SistemaVinculos />} />
            </Route>

            <Route element={<AdminRoute />}>
              <Route path='/sistema' element={<Sistema />} />
              <Route path='/sistema/clinicas' element={<SistemaClinicas />} />
              <Route path='/sistema/usuarios' element={<SistemaUsuarios />} />
            </Route>

            {/* Rotas protegidas dentro da AuthenticatedRoute */}
            <Route element={<AuthenticatedRoute />}>
              <Route path='/clinica/inicio' element={<ClinicaInicio />} />
              <Route path='/clinica' element={<Clinica />} />
              <Route path="/clinica/editar" element={<ClinicaGerenciamento />} />
              <Route path='/clinica/usuarios' element={<UsuariosDaClinica />} />
              <Route path="/clinica/tutores" element={<TutoresDaClinica />} />
              <Route path="/clinica/consultas" element={<ConsultasDaClinica />} />
              <Route path="/clinica/animais" element={<AnimaisDaClinica />} />
            </Route>
          </Routes>
        </AuthProvider>
      </NotificationProvider>
    </Router>
  );
};

export default App;
