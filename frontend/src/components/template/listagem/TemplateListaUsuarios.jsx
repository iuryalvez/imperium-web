import React from 'react';
import TabelaEntidade from '../../organisms/TabelaEntidade';

const TemplateListaUsuarios = ({ usuarios, camposCadastro }) => {
  console.log("Usuários na Template: ", usuarios);
  return (
    <TabelaEntidade entidade="usuarios" dados={usuarios} camposCadastro={camposCadastro} />
  );
};

export default TemplateListaUsuarios;
