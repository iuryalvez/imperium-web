import React, { useState, useEffect } from "react";
import {
  Form,
  Button,
  Input,
  InputNumber,
  Slider,
  Switch,
  Select,
  Row,
  Col,
  Divider,
  message,
  Popconfirm,
} from "antd";
import { DeleteOutlined } from "@ant-design/icons";
import { racoesDaClinicaAPI } from "../../../../api/clinica/racoesDaClinicaAPI";

const opcoesGrauAtividade = [
  { value: 1, label: "Baixo" },
  { value: 2, label: "Moderado" },
  { value: 3, label: "Alto" },
];

const marcasCondicaoCorporal = {
  1: "1",
  2: "2",
  3: "3",
  4: "4",
  5: "5",
  6: "6",
  7: "7",
  8: "8",
  9: "9",
};

const opcoesTipoDieta = [
  { value: 1, label: "Manutenção" },
  { value: 2, label: "Crescimento" },
  { value: 3, label: "Lactação" },
  { value: 4, label: "Gestação" },
];

const FormularioRelatorio = ({
  consulta,
  initialValues,
  onFinish,
  onDelete,
  onCancel,
}) => {
  const [form] = Form.useForm();
  const [racoes, setRacoes] = useState([]);
  const [loadingRacoes, setLoadingRacoes] = useState(true);

  const isGestante = Form.useWatch("isGestante", form);
  const isLactante = Form.useWatch("isLactante", form);

  const isFemea = consulta?.sexoAnimal === 2 ? true : false;

  useEffect(() => {
    if (consulta?.especieAnimal) {
      racoesDaClinicaAPI
        .listar(consulta.especieAnimal)
        .then((data) =>
          setRacoes(data.map((r) => ({ label: r.nome, value: r.id })))
        )
        .catch(() => message.error("Erro ao carregar lista de rações."))
        .finally(() => setLoadingRacoes(false));
    }
  }, [consulta]);

  useEffect(() => {
    if (initialValues) {
      const processedValues = { ...initialValues };
      // Ajusta valores booleanos para switches
      processedValues.isGestante = initialValues.isGestante;
      processedValues.isLactante = initialValues.isLactanteNumeroFilhotes > 0;
      form.setFieldsValue(processedValues);
    } else {
      form.setFieldsValue({
        isGestante: false,
        isLactanteNumeroFilhotes: 0, // Garante o valor padrão correto
      });
    }
  }, [initialValues, form]);

  // useEffect(() => {
  //   if (!isGestante) {
  //     form.setFieldsValue({ semanaGestacao: undefined });
  //   }
  // }, [isGestante, form]);

  // useEffect(() => {
  //   if (!isLactante) {
  //     form.setFieldsValue({
  //       isLactanteNumeroFilhotes: 0,
  //       semanaLactacao: undefined,
  //     });
  //   }
  // }, [isLactante, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item name="peso" label="Peso (kg)" rules={[{ required: true }]}>
            <InputNumber
              placeholder="0,00"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
          <Form.Item
            name="grauAtividade"
            label="Grau de Atividade"
            rules={[{ required: true }]}
          >
            <Select
              options={opcoesGrauAtividade}
              placeholder="Selecione o grau de atividade"
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            name="racaoId"
            label="Ração Utilizada"
            rules={[{ required: true }]}
          >
            <Select
              showSearch
              placeholder="Selecione a ração"
              options={racoes}
              loading={loadingRacoes}
              optionFilterProp="label"
            />
          </Form.Item>
          <Form.Item
            name="tipoDieta"
            label="Tipo da Dieta"
            rules={[
              { required: true, message: "O tipo da dieta é obrigatório." },
            ]}
          >
            <Select
              options={opcoesTipoDieta}
              placeholder="Selecione o tipo da dieta"
            />
          </Form.Item>
        </Col>
      </Row>
      <Form.Item
        name="condicaoCorporal"
        label="Condição Corporal (1-9)"
        rules={[{ required: true }]}
      >
        <Slider min={1} max={9} marks={marcasCondicaoCorporal} />
      </Form.Item>

      <Form.Item name="descricaoDieta" label="Descrição Adicional da Dieta">
        <Input.TextArea
          rows={4}
          placeholder="Ex: Adicionar 50g de alimento úmido pela manhã."
        />
      </Form.Item>

      {isFemea && (
        <>
          <Divider orientation="left" plain>
            Status Reprodutivo
          </Divider>
          <Row gutter={24}>
            <Col xs={24} sm={12}>
              <Form.Item
                name="isGestante"
                label="Em Gestação?"
                valuePropName="checked"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isGestante && (
                <Form.Item
                  name="semanaGestacao"
                  label="Semanas de Gestação"
                  rules={[{ required: true, message: "Obrigatório" }]}
                >
                  <InputNumber
                    style={{ width: "100%" }}
                    min={1}
                    placeholder="Nº de semanas"
                  />
                </Form.Item>
              )}
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item
                name="isLactante"
                label="Em Lactação?"
                valuePropName="checked"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isLactante && (
                <>
                  <Form.Item
                    name="isLactanteNumeroFilhotes"
                    label="Nº de Filhotes"
                    rules={[
                      { required: true, message: "Obrigatório" },
                      {
                        validator: (_, value) =>
                          value > 0
                            ? Promise.resolve()
                            : Promise.reject(
                                new Error(
                                  "O nº de filhotes deve ser maior que 0"
                                )
                              ),
                      },
                    ]}
                  >
                    <InputNumber
                      style={{ width: "100%" }}
                      min={1}
                      placeholder="Quantos filhotes?"
                    />
                  </Form.Item>
                  <Form.Item name="semanaLactacao" label="Semanas de Lactação">
                    <InputNumber
                      style={{ width: "100%" }}
                      min={1}
                      placeholder="Nº de semanas"
                    />
                  </Form.Item>
                </>
              )}
            </Col>
          </Row>
        </>
      )}

      <div className="flex justify-end gap-2 mt-6">
        <Button onClick={onCancel}>Cancelar</Button>
        {initialValues && (
          <Popconfirm
            title="Excluir relatório"
            description="Tem certeza que deseja excluir este relatório? Esta ação não pode ser desfeita."
            onConfirm={() => onDelete(initialValues.id)}
            okText="Sim, Excluir"
            cancelText="Cancelar"
          >
            <Button icon={<DeleteOutlined />} danger>
              Excluir
            </Button>
          </Popconfirm>
        )}
        <Button type="primary" htmlType="submit">
          Salvar Relatório
        </Button>
      </div>
    </Form>
  );
};

export default FormularioRelatorio;
