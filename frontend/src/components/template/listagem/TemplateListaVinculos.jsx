import React from 'react';
import ListaCards from '../../organisms/ListaCards';

const TemplateListaVinculos = ({ vinculos }) => {
  console.log("Vinculos na Template: ", vinculos);
  return (
    <ListaCards tipoEntidade="Vínculo" dados={vinculos} />
  );
};

export default TemplateListaVinculos;
