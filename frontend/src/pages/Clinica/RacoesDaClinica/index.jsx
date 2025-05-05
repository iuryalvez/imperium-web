import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI.js";

const RacoesDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Rações"
      entidade="racoesDaClinica"
      api={racoesDaClinicaAPI}
      nomeBotao="Cadastrar Ração"
      nomeEntidade="Rações"
    />
  );
};

export default RacoesDaClinica;