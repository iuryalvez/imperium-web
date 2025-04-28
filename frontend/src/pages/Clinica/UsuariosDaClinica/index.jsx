import React from "react";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";

const UsuariosDaClinica = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Usuários"
      entidade="usuariosDaClinica"
      api={usuariosDaClinicaAPI}
      nomeBotao="Cadastrar Usuário"
      nomeEntidade="Usuários"
    />
  );
};

export default UsuariosDaClinica;