import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import { AuthProvider } from "./services/AuthContext";
import { NotificationProvider } from "./services/NotificationContext";

import LayoutPublic from "./components/layout/LayoutPublic.jsx";
import LayoutLogged from "./components/layout/LayoutLogged.jsx";
import LayoutAuthenticated from "./components/layout/LayoutAuthenticated.jsx";

import Login from "./features/Login/Login.jsx";
import Sobre from "./features/Sobre/index.jsx";

import SistemaInicioAdmin from "./features/Sistema/SistemaInicioAdmin/SistemaInicioAdmin.jsx";
import SistemaInicioUsuario from "./features/Sistema/SistemaInicioUsuario/SistemaInicioUsuario.jsx";
import ClinicasDoSistema from "./features/Sistema/ClinicasDoSistema/ClinicasDoSistema.jsx";
import UsuariosDoSistema from "./features/Sistema/UsuariosDoSistema/UsuariosDoSistema.jsx";
import PerfilPessoal from "./features/Sistema/PerfilPessoal/PerfilPessoal.jsx";

import ClinicaInicio from "./features/Clinica/ClinicaInicioHome/index.jsx";

import UsuariosDaClinica from "./features/Clinica/UsuariosDaClinica/UsuariosDaClinica.jsx";
import AnimaisDaClinica from "./features/Clinica/AnimaisDaClinica/AnimaisDaClinica.jsx";
import AcompanhamentoAnimal from "./features/Clinica/AnimaisDaClinica/AcompanhamentoAnimal/AcompanhamentoAnimal.jsx";
import TutoresDaClinica from "./features/Clinica/TutoresDaClinica/TutoresDaClinica.jsx";
import RacoesDaClinica from "./features/Clinica/RacoesDaClinica/RacoesDaClinica.jsx";
import ConsultasDaClinica from "./features/Clinica/ConsultasDaClinica/ConsultasDaClinica.jsx";
import PerfilClinica from "./features/Clinica/ClinicaDashboard/PerfilClinica.jsx";

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
                      path="/sistema/perfil"
                      element={<PerfilPessoal />}
                    />
                    <Route
                      path="/sistema/vinculos"
                      element={<SistemaInicioUsuario />}
                    />
                  </Route>
                </Route>

                {/* Rotas que exigem ser Admin */}
                <Route element={<ProtectedRoute requiresAdmin />}>
                  <Route element={<LayoutLogged />}>
                    <Route path="/sistema" element={<SistemaInicioAdmin />} />
                    <Route
                      path="/sistema/clinicas"
                      element={<ClinicasDoSistema />}
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
                    <Route path="/clinica" element={<ClinicaInicio />} />
                    <Route
                      path="/clinica/editar"
                      element={<PerfilClinica />}
                    />
                    <Route
                      path="/clinica/animais"
                      element={<AnimaisDaClinica />}
                    />
                    <Route
                      path="/clinica/animais/acompanhamento/:acompanhamentoId"
                      element={<AcompanhamentoAnimal />}
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
