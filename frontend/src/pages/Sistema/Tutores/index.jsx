import React from "react";
import { tutoresAPI } from "../../../api/sistema/tutoresAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";

const Tutores = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Tutores do Sistema"
      entidade="tutoresDoSistema"
      api={tutoresAPI}
      nomeBotao="Cadastrar Tutor"
      nomeEntidade="Tutores do Sistema"
    />
  );
};

export default Tutores;
