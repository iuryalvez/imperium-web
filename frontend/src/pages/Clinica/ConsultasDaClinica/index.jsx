import React from "react";
import PaginaClinicaConsulta from "../../../components/templates/PaginaClinicaConsulta/index.jsx";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI.js";

const ConsultasDaClinica = () => {
  return (
    <PaginaClinicaConsulta
      titulo="Lista de Consultas"
      entidade="consultasDaClinica"
      api={consultasDaClinicaAPI}
      nomeBotao="Cadastrar Consulta"
      nomeEntidade="Consulta"
    />
  );
};

export default ConsultasDaClinica;