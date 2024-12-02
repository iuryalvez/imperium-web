import React from 'react';
import CardInfo from '../molecules/CardInfo';

const ListaCards = ({ tipoEntidade, lista }) => {
  console.log("Vinculos na ListaCards: ", lista);
  return (
    <div className="grid grid-cols-1">
      {lista.map((entidade) => (
        <CardInfo key={entidade.id} tipoEntidade={tipoEntidade} entidade={entidade} />
      ))}
    </div>
  );
};

export default ListaCards;
