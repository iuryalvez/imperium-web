import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { animaisDaClinicaAPI } from "../../../api/clinica/animaisDaClinicaAPI.js";

const AnimaisDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Animais"
      entidade="animaisDaClinica"
      api={animaisDaClinicaAPI}
      nomeBotao="Cadastrar Animal"
      nomeEntidade="Animais"
    />
  );
};

export default AnimaisDaClinica;