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
  Skeleton,
} from "antd";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";

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

const FormularioRelatorio = ({
  consulta,
  initialValues,
  onFinish,
  onCancel,
}) => {
  const [form] = Form.useForm();
  const [racoes, setRacoes] = useState([]);
  const [loadingRacoes, setLoadingRacoes] = useState(true);

  // --- Observadores para a lógica condicional ---
  // (Assumindo que 'sexoAnimal' virá no objeto 'consulta')
  const isFemea = consulta?.sexoAnimal === 2;
  const isGestante = Form.useWatch("isGestante", form);
  // Usamos um switch para controlar a visibilidade dos campos de lactação
  const estaLactando = Form.useWatch("estaLactando", form);

  useEffect(() => {
    // Busca rações compatíveis com a espécie do animal da consulta
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
      // Cria um valor para o switch 'estaLactando' com base nos dados iniciais
      const emLactacaoInicial = initialValues.isLactanteNumeroFilhotes > 0;
      form.setFieldsValue({
        ...initialValues,
        estaLactando: emLactacaoInicial,
      });
    } else {
      // Define valores iniciais padrão para um novo relatório
      form.setFieldsValue({
        isGestante: false,
        estaLactando: false,
        isLactanteNumeroFilhotes: 0,
      });
    }
  }, [initialValues, form]);

  // --- Effects para limpar dados quando as condições mudam ---
  useEffect(() => {
    // Se o animal não está gestante, limpa o campo de semanas
    if (!isGestante) {
      form.setFieldsValue({ semanaGestacao: undefined });
    }
  }, [isGestante, form]);

  useEffect(() => {
    // Se o animal não está em lactação, limpa os campos relacionados
    if (!estaLactando) {
      form.setFieldsValue({
        isLactanteNumeroFilhotes: 0,
        semanaLactacao: undefined,
      });
    }
  }, [estaLactando, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item name="peso" label="Peso (kg)" rules={[{ required: true }]}>
            <InputNumber
              style={{ width: "100%" }}
              min={0}
              step={0.1}
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
            name="condicaoCorporal"
            label="Condição Corporal (1-9)"
            rules={[{ required: true }]}
          >
            <Slider min={1} max={9} marks={marcasCondicaoCorporal} />
          </Form.Item>
        </Col>
      </Row>

      <Form.Item name="descricaoDieta" label="Descrição Adicional da Dieta">
        <Input.TextArea
          rows={4}
          placeholder="Ex: Adicionar 50g de alimento úmido pela manhã."
        />
      </Form.Item>

      {/* A seção de status reprodutivo só aparece se o animal for fêmea */}
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
              {/* O campo de semanas só aparece se o switch 'isGestante' estiver ligado */}
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
              {/* O switch 'estaLactando' controla a visibilidade dos campos abaixo */}
              <Form.Item
                name="estaLactando"
                label="Em Lactação?"
                valuePropName="checked"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {estaLactando && (
                <>
                  <Form.Item
                    name="isLactanteNumeroFilhotes"
                    label="Nº de Filhotes"
                    rules={[{ required: true, message: "Obrigatório" }]}
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
        <Button type="primary" htmlType="submit" loading={form.isSubmitting}>
          Salvar Relatório
        </Button>
      </div>
    </Form>
  );
};

export default FormularioRelatorio;
