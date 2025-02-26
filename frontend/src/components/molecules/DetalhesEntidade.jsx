import React from 'react';

const DetalhesEntidade = ({ entidade, tipoEntidade }) => {
  const renderDetalhes = () => {
    switch (tipoEntidade) {
      case 'Animal':
        return (
          <>
            <h3>Animal: {entidade.Nome}</h3>
            <p><strong>Idade:</strong> {entidade.Idade}</p>
            <p><strong>Peso:</strong> {entidade.Peso} kg</p>
            <p><strong>Usuário Responsável:</strong> {entidade.Tutor.Nome || 'Não atribuído'}</p>
          </>
        );
      case 'Clínica':
        return (
          <>
            <h3>Clínica: {entidade.NomeSocial}</h3>
            <p><strong>Nome Fantasia:</strong> {entidade.NomeFantasia}</p>
            <p><strong>Endereço:</strong> {entidade.Endereco}</p>
            <p><strong>CNPJ:</strong> {entidade.CNPJ}</p>
          </>
        );
      case 'Usuário':
        return (
          <>
            <p><strong>Nome:</strong> {entidade.Nome}</p>
            <p><strong>Email:</strong> {entidade.Email}</p>
            <p><strong>CPF:</strong> {entidade.CPF}</p>
          </>
        );
      default:
        return <p>Tipo de entidade desconhecido.</p>;
    }
  };

  return (
    <div className="detalhes-entidade">
      {renderDetalhes()}
    </div>
  );
};

export default DetalhesEntidade;
