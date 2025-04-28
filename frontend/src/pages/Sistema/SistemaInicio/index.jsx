import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../services/AuthContext.js';   
import BotaoMenu from '../../../components/atoms/BotaoMenu.jsx';
import BotaoOff from '../../../components/atoms/BotaoOff.jsx';

const SistemaInicio = () => {
  const { isAdmin, userName } = useAuth();
  const navigate = useNavigate();
  
  useEffect(() => {
    if (!isAdmin) {
      navigate('/sistema/vinculos');
    }
  }, [isAdmin, navigate]);

  const botoesGerais = [
      { texto: 'Clínicas', rota: '/sistema/clinicas', ativo: true },
      { texto: 'Usuários', rota: '/sistema/usuarios', ativo: true },
      { texto: 'Animais', rota: '/sistema/animais', ativo: false },
    //   { texto: 'Tutores', rota: '/sistema/tutores', ativo: false },
    //   { texto: 'Dashboard', rota: '/sistema/dashboard', ativo: false }
  ];

  const renderBotoes = () => {
      return botoesGerais.map(({ texto, rota, ativo }) =>
          ativo ? (
              <BotaoMenu key={rota} texto={texto} onClick={() => navigate(rota)} />
          ) : (
              <BotaoOff key={rota} texto={texto} onClick={() => navigate(rota)} />
          )
      );
  };

  return (
      <div className='info-content'>
          <h1 className="text-2xl font-bold mb-4 text-green-dark">
              Bem-vindo, {userName || 'Usuário'}!
          </h1>
          <div className="flex flex-wrap justify-center gap-10 pt-8">
              {renderBotoes()}
          </div>
      </div>
  );
};

export default SistemaInicio;
