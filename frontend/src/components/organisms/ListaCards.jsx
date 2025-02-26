import React from 'react';
import CardInfo from '../molecules/CardInfo';

const ListaCards = ({ tipoEntidade, dados }) => {
  console.log("Vinculos na ListaCards: ", dados);
  return (
    <div className="grid grid-cols-1">
      {dados.map((entidade) => (
        <CardInfo key={entidade.id} tipoEntidade={tipoEntidade} entidade={entidade} />
      ))}
    </div>
  );
};

export default ListaCards;
