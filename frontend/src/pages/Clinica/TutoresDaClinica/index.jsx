import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI.js";

const TutoresDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Tutores"
      entidade="tutoresDaClinica"
      nomeEntidade="Tutor"
      api={tutoresDaClinicaAPI}
      nomeBotao="Cadastrar Tutor"
    />
  );
};

export default TutoresDaClinica;