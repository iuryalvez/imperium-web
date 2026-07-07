import { Typography, Space } from 'antd';

import './SistemaInicioAdmin.css';

import { useAuth } from '../../../services/AuthContext.js';
import BotaoMenu from './components/BotaoMenu.jsx';

const { Title } = Typography;

const SistemaInicio = () => {
  const { userName } = useAuth();
  
  const botoesGerais = [
      { texto: 'Clínicas', rota: '/sistema/clinicas', ativo: true },
      { texto: 'Usuários', rota: '/sistema/usuarios', ativo: true },
      { texto: 'Animais', rota: '/sistema/animais', ativo: false },
  ];

  return (
      <div>
          <Title level={2} className="titulo-pagina-inicio">
              Bem-vindo, {userName || 'Usuário'}!
          </Title>

          <div className="container-botoes-menu">
            <Space size="large" wrap style={{ justifyContent: 'center' }}>
              {botoesGerais.map((botao) => (
                <BotaoMenu
                  key={botao.rota}
                  texto={botao.texto}
                  rota={botao.rota}
                  ativo={botao.ativo}
                />
              ))}
            </Space>
          </div>
      </div>
  );
};

export default SistemaInicio;