// src/pages/AcompanhamentoAnimal/components/FormularioNovaDieta/index.jsx

import React, { useEffect } from "react";
import {
  Form,
  Input,
  Select,
  Row,
  Col,
  InputNumber,
  Alert,
  Divider,
} from "antd";
import { useWatch } from "antd/es/form/Form";
import { InfoCircleOutlined } from "@ant-design/icons";

import FormManutencao from "./FormManutencao";
import FormCrescimento from "./FormCrescimento";
import FormLactacao from "./FormLactacao";
import FormGestacao from "./FormGestacao";

const opcoesTipoDieta = [
  { value: 1, label: "Manutenção" },
  { value: 2, label: "Crescimento" },
  { value: 3, label: "Lactação" },
  { value: 4, label: "Gestação" },
];

const FormularioDieta = ({ onFinish, initialValues, racoes, dadosRaca }) => {
  const [form] = Form.useForm();
  const tipoDietaSelecionado = useWatch("tipoDieta", form);

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue({
        racaoId: initialValues.racaoId,
        tipoDieta: initialValues.tipoDieta,
        pesoAtual: initialValues.peso,
        numeroFilhotes: initialValues.isLactanteNumeroFilhotes,
        semanaLactacao: initialValues.semanaLactacao,
      });
    }
  }, [initialValues, form]);

  const racoesOptions =
    racoes?.map((r) => ({ label: r.nome, value: r.id })) || [];

  const renderSpecificForm = () => {
    switch (tipoDietaSelecionado) {
      case 1:
        return <FormManutencao />;
      case 2:
        return <FormCrescimento />;
      case 3:
        return <FormLactacao />;
      case 4:
        return <FormGestacao />;
      default:
        return null;
    }
  };

  return (
    <Form
      id="form-dieta-animal"
      form={form}
      layout="vertical"
      onFinish={onFinish}
    >
      <Row gutter={16}>
        <Col span={12}>
          <Form.Item
            name="racaoId"
            label="Ração Base"
            rules={[{ required: true }]}
          >
            <Select
              showSearch
              optionFilterProp="label"
              options={racoesOptions}
            />
          </Form.Item>
        </Col>
        <Col span={12}>
          <Form.Item
            name="tipoDieta"
            label="Tipo da Dieta"
            rules={[{ required: true }]}
          >
            <Select options={opcoesTipoDieta} />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" className="mt-0 mb-4 text-gray-500 text-sm">
        Parâmetros de Peso
      </Divider>

      {dadosRaca?.nome && (dadosRaca.pesoMin || dadosRaca.pesoMax) && (
        <Alert
          className="mb-4"
          type="info"
          showIcon
          icon={<InfoCircleOutlined />}
          message={`Referência Literária: ${dadosRaca.nome}`}
          description={`O peso ideal desta raça, ajustado para o sexo do paciente, varia entre ${dadosRaca.pesoMin || "?"}kg e ${dadosRaca.pesoMax || "?"}kg.`}
        />
      )}

      <Row gutter={16}>
        <Col span={12}>
          <Form.Item
            name="pesoAtual"
            label="Peso Atual (kg)"
            rules={[{ required: true }]}
          >
            <InputNumber
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>

        <Col span={12}>
          <Form.Item
            name="pesoIdeal"
            label="Peso Referência / Ideal (kg)"
            tooltip="Este peso ficará salvo no cadastro do paciente e será a nova Fonte da Verdade para os cálculos energéticos da dieta, sobrescrevendo a referência geral da raça."
          >
            <InputNumber
              placeholder="Ex: 15,0"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="mt-2">{renderSpecificForm()}</div>

      <Form.Item name="observacoes" label="Observações da Dieta">
        <Input.TextArea
          rows={3}
          placeholder="Instruções para o tutor (Ex: Adicionar água morna)"
        />
      </Form.Item>
    </Form>
  );
};

export default FormularioDieta;
