// src/pages/AcompanhamentoAnimal/index.jsx
import React from 'react';
import { Skeleton, Result, Button, Typography, Divider } from 'antd';
import { Link } from 'react-router-dom';
import { useAcompanhamentoAnimalManager } from './useAcompanhamentoAnimalManager';
import CabecalhoAnimal from './components/CabecalhoAnimal';

const { Title } = Typography;

const AcompanhamentoAnimal = () => {
  const { historico, loading } = useAcompanhamentoAnimalManager();

  if (loading) {
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  if (!historico) {
    return (
      <Result
        status="404"
        title="Histórico não encontrado"
        subTitle="Não foi possível carregar os dados do acompanhamento."
        extra={
          <Button type="primary">
            <Link to="/clinica/animais">Voltar para a Lista</Link>
          </Button>
        }
      />
    );
  }

  // O retorno da API aninha os dados do animal dentro do objeto principal retornado
  const dadosDoAnimal = {
      id: historico.id,
      nome: historico.nome,
      especie: historico.especie,
      sexo: historico.sexo,
      idade: historico.idade,
      faseVida: historico.faseVida,
      pesoAtual: historico.pesoAtual,
      condicaoCorporal: historico.condicaoCorporal,
      raca: historico.raca,
      tutores: historico.tutores,
  };

  return (
    <div>
      <Title level={2}>Acompanhamento de {historico.nome}</Title>
      
      <CabecalhoAnimal animal={dadosDoAnimal} />

      <Divider orientation="left">Histórico de Relatórios</Divider>
      
      {/* Próximo passo: Renderizar a lista de relatórios */}
      {/* historico.acompanhamento.relatorios */}
      <p>Aqui entrará a lista ou tabela com os relatórios do acompanhamento.</p>
    </div>
  );
};

export default AcompanhamentoAnimal;