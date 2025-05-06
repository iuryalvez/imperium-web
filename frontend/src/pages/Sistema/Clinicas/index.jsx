import React from "react";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import PaginaSistemaEntidadeComCadastro from "../../../components/templates/PaginaSistemaEntidadeComCadastro/index.jsx";

const Clinicas = () => {
  return (
    <PaginaSistemaEntidadeComCadastro
      titulo="Lista de Clínicas"
      entidade="clinicas"
      api={clinicasAPI}
      nomeBotao="Cadastrar Clínica"
      nomeEntidade="Clínica"
    />
  );
};

export default Clinicas;
