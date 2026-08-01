// src/pages/Acompanhamento/components/FormularioNovaDieta.jsx

import React from "react";
import { Form, Input, InputNumber, Select, Button, Row, Col } from "antd";

// Essas opções devem corresponder aos valores do seu Enum 'TipoDieta' no backend
const opcoesTipoDieta = [
  { value: 0, label: "Manutenção" },
  { value: 1, label: "Crescimento" },
  { value: 2, label: "Lactação" },
  { value: 3, label: "Gestação" },
  // Adicione outros tipos se houver
];

const FormularioNovaDieta = ({ onFinish, onCancel, avaliacaoId, racoes }) => {
  const [form] = Form.useForm();

  const racoesOptions = racoes.map((r) => ({ label: r.nome, value: r.id }));

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={onFinish}
      initialValues={{ avaliacaoId: avaliacaoId }}
    >
      {/* Campo escondido para enviar o ID do relatório */}
      <Form.Item name="avaliacaoId" hidden>
        <Input />
      </Form.Item>

      <Row gutter={16}>
        <Col span={12}>
          <Form.Item
            name="racaoId"
            label="Ração Base"
            rules={[{ required: true, message: "Selecione a ração." }]}
          >
            <Select
              showSearch
              optionFilterProp="label"
              options={racoesOptions}
              placeholder="Selecione a ração"
            />
          </Form.Item>
        </Col>
        <Col span={12}>
          <Form.Item
            name="tipoDieta"
            label="Tipo da Dieta"
            rules={[{ required: true, message: "Selecione o tipo." }]}
          >
            <Select options={opcoesTipoDieta} placeholder="Selecione o tipo" />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col span={8}>
          <Form.Item name="pesoAtual" label="Peso Atual (kg)">
            <InputNumber
              placeholder="Ex: 12.5"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col span={8}>
          <Form.Item name="pesoObjetivo" label="Peso Objetivo (kg)">
            <InputNumber
              placeholder="Ex: 11.0"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
        <Col span={8}>
          <Form.Item name="duracaoEstimada" label="Duração (dias)">
            <InputNumber style={{ width: "100%" }} placeholder="Ex: 90" />
          </Form.Item>
        </Col>
      </Row>

      <Form.Item name="observacoes" label="Observações">
        <Input.TextArea rows={3} placeholder="Instruções adicionais" />
      </Form.Item>

      <div className="flex justify-end gap-2">
        <Button onClick={onCancel}>Cancelar</Button>
        <Button type="primary" htmlType="submit">
          Calcular e Salvar Dieta
        </Button>
      </div>
    </Form>
  );
};

export default FormularioNovaDieta;
