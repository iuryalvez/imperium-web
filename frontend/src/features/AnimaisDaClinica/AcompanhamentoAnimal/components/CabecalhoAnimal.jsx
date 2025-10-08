// src/pages/AcompanhamentoAnimal/components/CabecalhoAnimal.jsx
import React from 'react';
import { Descriptions, Tag } from 'antd';

const CabecalhoAnimal = ({ animal }) => {
  if (!animal) return null;

  return (
    <Descriptions bordered column={{ xs: 1, sm: 2, md: 3 }}>
      <Descriptions.Item label="Nome">{animal.nome}</Descriptions.Item>
      <Descriptions.Item label="Espécie">{animal.especie}</Descriptions.Item>
      <Descriptions.Item label="Raça">{animal.raca.nome}</Descriptions.Item>
      <Descriptions.Item label="Sexo">{animal.sexo}</Descriptions.Item>
      <Descriptions.Item label="Idade">{`${animal.idade} anos`}</Descriptions.Item>
      <Descriptions.Item label="Fase da Vida">
        <Tag>{animal.faseVida}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="Peso Atual">{`${animal.pesoAtual} kg`}</Descriptions.Item>
      <Descriptions.Item label="Condição Corporal">
        <Tag color="orange">{animal.condicaoCorporal}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="Tutor(es)">
        {animal.tutores.length > 0 
          ? animal.tutores.map(t => t.nome).join(', ') 
          : 'Nenhum tutor associado'}
      </Descriptions.Item>
    </Descriptions>
  );
};

export default CabecalhoAnimal;