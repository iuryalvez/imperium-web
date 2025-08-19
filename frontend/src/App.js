import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import { AuthProvider } from "./services/AuthContext";
import { NotificationProvider } from "./services/NotificationContext";

import LayoutPublic from "./components/layout/LayoutPublic.jsx";
import LayoutLogged from "./components/layout/LayoutLogged.jsx";
import LayoutAuthenticated from "./components/layout/LayoutAuthenticated.jsx";

import Login from "./pages/Login/Login.jsx";
import Sobre from "./pages/Sobre/index.jsx";


import SistemaClinicas from "./pages/Sistema/Clinicas/index.jsx";

import Clinica from "./pages/Clinica/ClinicaInicio/index.jsx";
import ClinicaInicio from "./pages/Clinica/ClinicaInicio/index.jsx";
import ClinicaGerenciamento from "./pages/Clinica/ClinicaGerenciamento/index.jsx";
import UsuariosDaClinica from "./pages/Clinica/UsuariosDaClinica/index.jsx";
import TutoresDaClinica from "./pages/Clinica/TutoresDaClinica/index.jsx";
import AnimaisDaClinica from "./pages/Clinica/AnimaisDaClinica/index.jsx";

import Sistema from "./features/SistemaInicioAdmin/SistemaInicioAdmin.jsx";
import ListaDeVinculos from "./pages/Sistema/ListaDeVinculos/ListaDeVinculos.jsx";

import UsuariosDoSistema from "./features/UsuariosDoSistema/UsuariosDoSistema.jsx";
import RacoesDaClinica from "./features/RacoesDaClinica/RacoesDaClinica.jsx";
import ConsultasDaClinica from "./features/ConsultasDaClinica/ConsultasDaClinica.jsx";

import ProtectedRoute from "./components/servicesComponents/ProtectedRoute.js";

import { ConfigProvider, App as AntApp } from "antd";
import { themeConfig } from "./theme/ant-theme.js";
import ptBR from "antd/locale/pt_BR";
import { validateMessages } from "./theme/validateMessages.js";

const App = () => {
  return (
    <ConfigProvider
      locale={ptBR} // Aplica a tradução para português
      theme={themeConfig} // Aplica sua paleta de cores e estilos
      form={{ validateMessages: validateMessages }} // Sobrescreve mensagens de validação
    >
      {/* O componente AntApp é necessário para que message, notification, etc. peguem o tema */}
      <AntApp>
        <Router>
          <NotificationProvider>
            <AuthProvider>
              <Routes>
                {/* Rotas públicas */}
                <Route element={<ProtectedRoute isPublic />}>
                  <Route element={<LayoutPublic />}>
                    <Route path="/" element={<Login />} />
                    <Route path="login" element={<Login />} />
                    <Route path="sobre" element={<Sobre />} />
                  </Route>
                </Route>

                {/* Rotas para usuário logado (sem vínculo escolhido) */}
                <Route element={<ProtectedRoute requiresLogged />}>
                  <Route element={<LayoutLogged />}>
                    <Route
                      path="/sistema/vinculos"
                      element={<ListaDeVinculos />}
                    />
                  </Route>
                </Route>

                {/* Rotas que exigem ser Admin */}
                <Route element={<ProtectedRoute requiresAdmin />}>
                  <Route element={<LayoutLogged />}>
                    <Route path="/sistema" element={<Sistema />} />
                    <Route
                      path="/sistema/clinicas"
                      element={<SistemaClinicas />}
                    />
                    <Route
                      path="/sistema/usuarios"
                      element={<UsuariosDoSistema />}
                    />
                  </Route>
                </Route>

                {/* Rotas que exigem vínculo (isAuthenticated) */}
                <Route element={<ProtectedRoute requiresAuth />}>
                  <Route element={<LayoutAuthenticated />}>
                    <Route path="/clinica/inicio" element={<ClinicaInicio />} />
                    <Route path="/clinica" element={<Clinica />} />
                    <Route
                      path="/clinica/editar"
                      element={<ClinicaGerenciamento />}
                    />
                    <Route
                      path="/clinica/animais"
                      element={<AnimaisDaClinica />}
                    />
                    <Route
                      path="/clinica/tutores"
                      element={<TutoresDaClinica />}
                    />
                    <Route
                      path="/clinica/usuarios"
                      element={<UsuariosDaClinica />}
                    />
                    <Route
                      path="/clinica/consultas"
                      element={<ConsultasDaClinica />}
                    />
                    <Route
                      path="/clinica/racoes"
                      element={<RacoesDaClinica />}
                    />
                  </Route>
                </Route>
              </Routes>
            </AuthProvider>
          </NotificationProvider>
        </Router>
      </AntApp>
    </ConfigProvider>
  );
};

export default App;
