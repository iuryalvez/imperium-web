// src/pages/AcompanhamentoAnimal/components/DetalhesDieta.jsx

import React from 'react';
import { Card, Statistic, Row, Col, Descriptions, Typography, Divider } from 'antd';
import { ExperimentOutlined } from '@ant-design/icons';

const { Text } = Typography;

const DetalhesDieta = ({ dieta }) => {
  if (!dieta) return null;

  return (
    <Card size="small" type="inner" title="Resultado da Dieta Calculada">
      <Row gutter={[16, 16]}>
        <Col xs={24} md={12}>
          <Statistic
            title="Necessidade Energética"
            value={dieta.necessidadeEnergeticaKcal}
            precision={2}
            suffix="kcal/dia"
          />
        </Col>
        <Col xs={24} md={12}>
          <Statistic
            title="Quantidade de Alimento"
            value={dieta.quantidadeAlimentoGramas}
            precision={2}
            prefix={<ExperimentOutlined />}
            suffix="g/dia"
          />
        </Col>
      </Row>
      <Divider dashed />
      <Descriptions column={1} size="small">
        <Descriptions.Item label="Ração Base">{dieta.racaoUtilizada}</Descriptions.Item>
        <Descriptions.Item label="Tipo de Dieta">{dieta.tipoDieta}</Descriptions.Item>
        <Descriptions.Item label="Observações">{dieta.observacoes || 'Nenhuma'}</Descriptions.Item>
      </Descriptions>
      
      <Divider orientation='left' plain>Sugestão de Refeições</Divider>
      <Text>- 2 Refeições de <strong>{(dieta.quantidadeAlimentoGramas / 2).toFixed(1)}g</strong> cada</Text><br/>
      <Text>- 3 Refeições de <strong>{(dieta.quantidadeAlimentoGramas / 3).toFixed(1)}g</strong> cada</Text>
    </Card>
  );
};

export default DetalhesDieta;