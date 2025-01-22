import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import BotaoSecundario from '../atoms/BotaoSecundario';

const CardInfo = ({ tipoEntidade, entidade }) => {
  const { escolherVinculo } = useAuth();
  const navigate = useNavigate();

  console.log("Vinculos no CardInfo: ", entidade);
  
  const handleRedirect = async () => {
    switch (tipoEntidade) {
      case 'Clinica':
        navigate(`/clinicas/${entidade.id}`);
        break;
      case 'Animal':
        navigate(`/animais/${entidade.id}`);
        break;
      case 'Tutor':
        navigate(`/tutores/${entidade.id}`);
        break;
      case 'Usuario':
        navigate(`/usuarios/${entidade.id}`);
        break;
      case 'Vínculo':
        try {
          await escolherVinculo(entidade.id);
        } catch (error) {
          console.error('Erro ao escolher vínculo:', error);
          alert('Não foi possível selecionar o vínculo. Tente novamente.');
        }
        break;
      default:
        console.warn('Tipo de entidade desconhecido:', tipoEntidade);
        alert('Tipo de entidade não reconhecido.');
    }
  };

  const renderCardContent = () => {
    switch (tipoEntidade) {
      case 'Clinica':
        return (
          <>
            <p className="text-green-dark">Nome Fantasia: {entidade.nomeFantasia}</p>
            <p className="text-green-dark">CNPJ: {entidade.cNPJ}</p>
          </>
        );
      case 'Animal':
        return (
          <>
            <p className="text-green-dark">Nome: {entidade.nome}</p>
            <p className="text-green-dark">Espécie: {entidade.especie}</p>
            <p className="text-green-dark">Sexo: {entidade.sexo}</p>
          </>
        );
      case 'Tutor':
        return (
          <>
            <p className="text-green-dark">Nome: {entidade.nome}</p>
            <p className="text-green-dark">E-mail: {entidade.email}</p>
          </>
        );
      case 'Usuario':
        return (
          <>
            <p className="text-green-dark">Nome: {entidade.nome}</p>
            <p className="text-green-dark">E-mail: {entidade.email}</p>
            <p className="text-green-dark">Tipo de usuário: {entidade.perfil}</p>
            {entidade.senha && (
              <p className="text-green-dark">Senha: {entidade.senha}</p>
            )}
          </>
        );
      case 'Vinculo':
        return (
          <>
            <p className="text-green-dark">
              Clínica: {entidade.nomeClinica || 'N/A'}
            </p>
            <p className="text-green-dark">
              Tipo de Usuário: {entidade.nomePerfil}
            </p>
          </>
        );
      default:
        return <p className="text-green-dark">Entidade desconhecida</p>;
    }
  };

  console.log(entidade); // Certifique-se de que os dados estão no formato esperado
  
  return (
    <div className="flex items-center justify-between p-4 mb-4 bg-white shadow-lg rounded-lg w-full">
      <div className="flex flex-col space-y-2">
        <h3 className="font-bold text-lg text-green-dark">
          {tipoEntidade} - ID: {entidade.id}
        </h3>
        <div className="pl-4">{renderCardContent()}</div>
      </div>
      <BotaoSecundario texto="Acessar" onClick={handleRedirect} />
    </div>
  );
};

export default CardInfo;
