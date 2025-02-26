import React from 'react';
import TabelaEntidade from '../../organisms/TabelaEntidade';

const TemplateListaUsuarios = ({ usuarios, usuario }) => {
  console.log("Usuários na Template: ", usuarios);
  return (
    <TabelaEntidade entidade="usuarios" dados={usuarios} api={usuario} />
  );
};

export default TemplateListaUsuarios;
