// src/pages/AcompanhamentoAnimal/components/FormularioNovaDieta/index.jsx

import React from "react";
import { Form, Input, Select, Button, Row, Col } from "antd";
import { useWatch } from "antd/es/form/Form";

// Importando os novos formulários específicos
import FormManutencao from "./FormManutencao";
import FormCrescimento from "./FormCrescimento";
import FormLactacao from "./FormLactacao";
import FormGestacao from "./FormGestacao";

const opcoesTipoDieta = [
  { value: 0, label: "Manutenção" },
  { value: 1, label: "Crescimento" },
  { value: 2, label: "Lactação" },
  { value: 3, label: "Gestação" },
];

const FormularioDieta = ({ onFinish, onCancel, relatorioId, racoes }) => {
  const [form] = Form.useForm();

  // Observa o valor do campo 'tipoDieta' em tempo real
  const tipoDietaSelecionado = useWatch("tipoDieta", form);

  const racoesOptions = racoes.map((r) => ({ label: r.nome, value: r.id }));

  const renderSpecificForm = () => {
    switch (tipoDietaSelecionado) {
      case 0: // Manutenção
        return <FormManutencao />;
      case 1: // Crescimento
        return <FormCrescimento />;
      case 2: // Lactação
        return <FormLactacao />;
      case 3: // Gestação
        return <FormGestacao />;
      default:
        return null; // Nenhum formulário específico para mostrar
    }
  };

  return (
    <Form
      id="form-dieta-animal"
      form={form}
      layout="vertical"
      onFinish={onFinish}
      initialValues={{ relatorioId: relatorioId }}
    >
      <Form.Item name="relatorioId" hidden>
        <Input />
      </Form.Item>

      {/* Campos Comuns a Todos os Formulários */}
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

      {/* Área Dinâmica para o Formulário Específico */}
      {renderSpecificForm()}

      <Form.Item name="observacoes" label="Observações">
        <Input.TextArea rows={3} placeholder="Instruções adicionais" />
      </Form.Item>
    </Form>
  );
};

export default FormularioDieta;
