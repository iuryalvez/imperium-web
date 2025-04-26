import React from "react";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";
import AppLayout from "../../../components/layout/AppLayout.jsx";

const UsuariosDaClinica = () => {
  return (
    <AppLayout temSidebar>
      <PaginaEntidadeComCadastro
        titulo="Lista de Usuários"
        entidade="usuariosDaClinica"
        api={usuariosDaClinicaAPI}
        nomeBotao="Cadastrar Usuário"
        nomeEntidade="Usuários"
      />
    </AppLayout>
  );
};

export default UsuariosDaClinica;