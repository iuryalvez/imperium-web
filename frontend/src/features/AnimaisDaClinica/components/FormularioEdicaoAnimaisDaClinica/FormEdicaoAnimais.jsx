import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  Select,
  DatePicker,
  Switch,
  Row,
  Col,
  Button,
  message,
} from "antd";
import dayjs from "dayjs";
import { tutoresDaClinicaAPI } from "../../../../api/clinica/tutoresDaClinicaAPI";
import { racasAPI } from "../../../../api/sistema/racasAPI";

const opcoesEspecie = [
  { value: 1, label: "Cão" },
  { value: 2, label: "Gato" },
];
const opcoesSexo = [
  { value: 1, label: "Macho" },
  { value: 2, label: "Fêmea" },
];

const FormEdicaoAnimal = ({
  onFinish,
  onCancel,
  isSubmitting,
  initialValues,
}) => {
  const [form] = Form.useForm();
  const [racas, setRacas] = useState([]);
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(false);

  const especieSelecionada = Form.useWatch("especie", form);

  // Efeito para buscar raças e tutores
  useEffect(() => {
    const carregarDadosDeSuporte = async () => {
      setLoading(true);
      try {
        // Busca tutores e todas as raças em paralelo
        const [tutoresData, racasData] = await Promise.all([
          tutoresDaClinicaAPI.listar(),
          racasAPI.listar(), // Supondo um método que busca todas as raças
        ]);

        setTutores(
          tutoresData.map((t) => ({
            label: `${t.nome} (${t.cpf || "N/A"})`,
            value: t.id,
          }))
        );
        setRacas(
          racasData.map((r) => ({
            label: r.nome,
            value: r.id,
            especie: r.especie,
          }))
        );
      } catch (err) {
        message.error("Falha ao carregar dados de suporte (raças/tutores).");
      } finally {
        setLoading(false);
      }
    };
    carregarDadosDeSuporte();
  }, []);

  // Efeito para preencher o formulário com os valores iniciais
  useEffect(() => {
    // Só preenche o formulário se tivermos valores iniciais E se o carregamento tiver terminado.
    if (initialValues && !loading) {
      form.setFieldsValue({
        ...initialValues,
        nascimento: initialValues.nascimento
          ? dayjs(initialValues.nascimento)
          : null,
        tutorIds: initialValues.tutorIds || [],
      });
    }
  }, [initialValues, loading, form]); 

  // Efeito para limpar a raça ao trocar de espécie
  useEffect(() => {
    // Evita limpar o campo na primeira renderização
    if (form.isFieldsTouched(["especie"])) {
      form.setFieldsValue({ racaId: undefined });
    }
  }, [especieSelecionada, form]);

  const especieParaFiltro = especieSelecionada !== undefined ? especieSelecionada : initialValues?.especie;

  return (
    <Form form={form} layout="vertical" onFinish={onFinish} >
      {/* Campos ocultos necessários para a edição */}
      <Form.Item name="id" hidden>
        <Input />
      </Form.Item>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item
            name="nome"
            label="Nome do Animal"
            rules={[{ required: true }]}
          >
            <Input placeholder="Ex: Bob" />
          </Form.Item>
          <Form.Item
            name="especie"
            label="Espécie"
            rules={[{ required: true }]}
          >
            <Select options={opcoesEspecie} placeholder="Selecione a espécie" />
          </Form.Item>
          <Form.Item name="racaId" label="Raça" rules={[{ required: true }]}>
            <Select
              placeholder={!especieParaFiltro ? "Primeiro, selecione a espécie" : "Selecione a raça"}
              disabled={!especieParaFiltro}
              loading={loading}
              options={racas.filter(
                (r) => r.especie === (especieParaFiltro === 1 ? "Cao" : "Gato")
              )}
              showSearch
              optionFilterProp="label"
            />
          </Form.Item>
          <Form.Item name="tutorIds" label="Tutor(es) Responsável(is)">
            <Select
              mode="multiple"
              placeholder="Busque e selecione os tutores"
              loading={loading}
              options={tutores}
              optionFilterProp="label"
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            name="nascimento"
            label="Data de Nascimento"
            rules={[{ required: true }]}
          >
            <DatePicker
              style={{ width: "100%" }}
              format="DD/MM/YYYY"
              placeholder="Selecione a data"
            />
          </Form.Item>
          <Form.Item name="sexo" label="Sexo" rules={[{ required: true }]}>
            <Select options={opcoesSexo} placeholder="Selecione o sexo" />
          </Form.Item>
          <Form.Item
            name="isCastrado"
            label="Castrado?"
            valuePropName="checked"
          >
            <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
          </Form.Item>
        </Col>
      </Row>
      <div className="flex justify-end gap-2 mt-6">
        <Button onClick={onCancel}>Cancelar</Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Cadastrar
        </Button>
      </div>
    </Form>
  );
};

export default FormEdicaoAnimal;
