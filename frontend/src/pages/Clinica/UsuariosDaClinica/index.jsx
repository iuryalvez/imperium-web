import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";

const UsuariosDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Usuários"
      entidade="usuariosDaClinica"
      api={usuariosDaClinicaAPI}
      nomeBotao="Cadastrar Usuário"
      nomeEntidade="Usuário da Clínica"
    />
  );
};

export default UsuariosDaClinica;