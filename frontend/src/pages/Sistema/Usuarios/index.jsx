import React from "react";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";

const UsuariosDoSistema = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Usuários do Sistema"
      entidade="usuariosDoSistema"
      api={usuariosDoSistemaAPI}
      nomeBotao="Cadastrar Usuário do Sistema"
      nomeEntidade="Usuários do Sistema"
    />
  );
};

export default UsuariosDoSistema;
