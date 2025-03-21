import React from 'react';
import TabelaEntidade from '../../organisms/TabelaEntidade';

const TemplateListaSistemaUsuarios = ({ usuarios, camposCadastro }) => {
  console.log("Usuarios do sistema na Template: ", usuarios);
  return (
    <TabelaEntidade entidade="usuariosDoSistema" dados={usuarios} camposCadastro={camposCadastro} />
  );
};

export default TemplateListaSistemaUsuarios;
