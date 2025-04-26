import React from "react";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";

const Clinicas = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Clínicas"
      entidade="clinicas"
      api={clinicasAPI}
      nomeBotao="Cadastrar Clínica"
      nomeEntidade="Clínicas"
    />
  );
};

export default Clinicas;
