import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  Select,
  DatePicker,
  Switch,
  Row,
  Col,
  message,
} from "antd";
import dayjs from "dayjs";
import { tutoresDaClinicaAPI } from "../../../../../api/clinica/tutoresDaClinicaAPI";
import { racasAPI } from "../../../../../api/sistema/racasAPI";

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
  initialValues,
}) => {
  const [form] = Form.useForm();
  const [racas, setRacas] = useState([]);
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(true); // Começa carregando

  const especieSelecionada = Form.useWatch("especie", form);

  // Efeito para buscar raças e tutores
  useEffect(() => {
    const carregarDadosDeSuporte = async () => {
      try {
        const [tutoresData, racasData] = await Promise.all([
          tutoresDaClinicaAPI.listar(),
          racasAPI.listar(),
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
        message.error("Falha ao carregar raças e tutores.");
      } finally {
        setLoading(false); // Finaliza o carregamento após a busca
      }
    };
    carregarDadosDeSuporte();
  }, []);

  // Efeito para preencher o formulário com os valores iniciais
  // Roda quando os valores iniciais mudam OU quando o carregamento termina
  useEffect(() => {
    if (initialValues && !loading && racas.length > 0 && tutores.length > 0) {
      form.setFieldsValue({
        ...initialValues,
        nascimento: initialValues.nascimento
          ? dayjs(initialValues.nascimento)
          : null,
        // O campo tutorIds já vem como uma lista de IDs, então não precisa de mapeamento
        tutorIds: initialValues.tutorIds || [],
      });
    }
  }, [initialValues, loading, form, racas, tutores]);

  // Filtra as opções de raça com base na espécie selecionada no formulário
  console.log("especie: ", especieSelecionada);
  const racasFiltradas = racas.filter(
    (r) => r.especie === (especieSelecionada === 1 ? "Cao" : "Gato")
  );

  console.log("Form Values:", form.getFieldsValue());
  console.log("Racas filtradas:", racasFiltradas);

  return (
    <Form id="form-edicao-animal-clinica" form={form} layout="vertical" onFinish={onFinish}>
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
            <Input />
          </Form.Item>
          <Form.Item
            name="especie"
            label="Espécie"
            rules={[{ required: true }]}
          >
            <Select
              options={opcoesEspecie}
              onChange={() => form.setFieldsValue({ racaId: undefined })}
            />
          </Form.Item>
          <Form.Item name="racaId" label="Raça" rules={[{ required: true }]}>
            <Select
              placeholder={
                !especieSelecionada
                  ? "Primeiro, selecione a espécie"
                  : "Selecione a raça"
              }
              disabled={!especieSelecionada || loading}
              loading={loading}
              options={racasFiltradas} // Usa a lista já filtrada
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
            <DatePicker style={{ width: "100%" }} format="DD/MM/YYYY" />
          </Form.Item>
          <Form.Item name="sexo" label="Sexo" rules={[{ required: true }]}>
            <Select options={opcoesSexo} />
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
    </Form>
  );
};

export default FormEdicaoAnimal;
