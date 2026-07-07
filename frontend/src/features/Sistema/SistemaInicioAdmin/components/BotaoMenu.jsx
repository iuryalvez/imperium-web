import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, Tooltip } from 'antd';
import './BotaoMenu.css';

const BotaoMenu = ({ texto, rota, ativo = true }) => {
  const navigate = useNavigate();

  // Se o botão estiver ATIVO
  if (ativo) {
    return (
      <Button
        type="primary" // Usará a cor colorPrimary do seu themeConfig
        className='botao-menu'
        onClick={() => navigate(rota)}
      >
        {texto}
      </Button>
    );
  }

  // Se o botão estiver INATIVO
  return (
    // Envolvemos o botão desabilitado em um Tooltip para explicar por que ele não funciona
    <Tooltip title="Função ainda não disponível">
      <Button
        type="primary"
        className='botao-menu'
        disabled // A prop 'disabled' aplica o estilo de desabilitado do Ant Design
      >
        {texto}
      </Button>
    </Tooltip>
  );
};

export default BotaoMenu;