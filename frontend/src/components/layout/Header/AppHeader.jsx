import React, { useMemo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Layout, Menu, Spin } from 'antd';
import { useAuth } from '../../../services/AuthContext'; // Ajuste o caminho
import logo from '../../../assets/logo-unificada.png'; // Ajuste o caminho

const { Header } = Layout;

const AppHeader = () => {
  const { isLoading, isLogged, isAuthenticated, logout, trocarVinculo } = useAuth();
  const navigate = useNavigate();

  // Usamos useMemo para calcular os itens do menu apenas quando o estado de auth mudar.
  const menuItems = useMemo(() => {
    // Caso de Loading
    if (isLoading) {
      return [<Spin key="loading" />];
    }
    
    // Menu para usuário autenticado COM vínculo
    if (isAuthenticated) {
      return [
        { key: 'perfil', label: <Link to="/clinica/perfil">Perfil</Link> },
        { key: 'trocar', label: 'Trocar Vínculo', onClick: trocarVinculo },
        { key: 'sair', label: 'Sair', onClick: logout, danger: true }, // 'danger' para estilizar em vermelho
      ];
    }

    // Menu para usuário logado SEM vínculo
    if (isLogged) {
      return [
        { key: 'sair', label: 'Sair', onClick: logout, danger: true },
      ];
    }

    // Menu Público (usuário deslogado)
    return [
      { key: 'sobre', label: <Link to="/sobre">Sobre</Link> },
      { key: 'login', label: <Link to="/login">Login</Link> },
    ];
  }, [isLoading, isLogged, isAuthenticated, logout, trocarVinculo]);

  return (
    // O Header do Ant Design já pega a cor de fundo do tema.
    <Header
      style={{
        position: 'fixed',
        top: 0,
        zIndex: 50,
        width: '100%',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '0 24px',
        backgroundColor: 'var(--cinza-background)', // Mantém sua cor se preferir
        borderBottom: '1px solid #f0f0f0',
      }}
    >
      <div className="logo" onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>
        <img src={logo} alt="Logo" style={{ height: '40px' }} />
      </div>
      
      {/* O Menu do Ant Design substitui o <ul> e <li> */}
      <Menu
        theme="light" // Ou "dark"
        mode="horizontal"
        items={menuItems}
        disabled={isLoading}
        style={{ borderBottom: 'none', minWidth: 0, flex: 'auto', justifyContent: 'flex-end', backgroundColor: 'transparent' }}
      />
    </Header>
  );
};

export default AppHeader;