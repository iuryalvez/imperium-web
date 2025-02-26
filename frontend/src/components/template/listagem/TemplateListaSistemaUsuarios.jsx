import React from 'react';
import TabelaEntidade from '../../organisms/TabelaEntidade';

const TemplateListaSistemaUsuarios = ({ usuarios, usuario }) => {
  console.log("Usuarios do sistema na Template: ", usuarios);
  return (
    <TabelaEntidade entidade="usuarios" dados={usuarios} api={usuario} />
  );
};

export default TemplateListaSistemaUsuarios;
