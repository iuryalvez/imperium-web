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
      navigate('/imperium/vinculos');
    }
  }, [isAdmin, navigate]);

  const botoesGerais = [
      { texto: 'Vínculos', rota: '/imperium/vinculos', ativo: true },
      { texto: 'Clínicas', rota: '/imperium/clinicas', ativo: true },
      { texto: 'Usuários', rota: '/imperium/usuarios', ativo: true },
      { texto: 'Animais', rota: '/imperium/animais', ativo: false },
      { texto: 'Perfis', rota: '/imperium/perfis', ativo: false }
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
      <div className="sistema-content">
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
