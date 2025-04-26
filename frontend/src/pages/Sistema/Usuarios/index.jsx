import React from "react";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";
import AppLayout from "../../../components/layout/AppLayout.jsx";

const UsuariosDoSistema = () => {
  return (
    <AppLayout temSidebar={false}>
      <PaginaEntidadeComCadastro
        titulo="Lista de Usuários do Sistema"
        entidade="usuariosDoSistema"
        api={usuariosDoSistemaAPI}
        nomeBotao="Cadastrar Usuário do Sistema"
        nomeEntidade="Usuários do Sistema"
      />
    </AppLayout>
  );
};

export default UsuariosDoSistema;
