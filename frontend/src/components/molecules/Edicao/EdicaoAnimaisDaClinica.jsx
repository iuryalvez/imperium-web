import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  Select,
  DatePicker,
  Switch,
  InputNumber,
  Row,
  Col,
  Slider,
  Divider,
} from "antd";
import dayjs from "dayjs"; // Essencial para a conversão de datas
import { racasAPI } from "../../../api/sistema/racasAPI";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI";

// --- Opções para os Selects ---
const opcoesEspecie = [
  { value: 1, label: "Cão" },
  { value: 2, label: "Gato" },
];
const opcoesSexo = [
  { value: 1, label: "Macho" },
  { value: 2, label: "Fêmea" },
];

const EdicaoAnimalDaClinica = ({ form, valoresIniciais }) => {
  // --- State para dados de selects ---
  const [racasDeCaes, setRacasDeCaes] = useState([]);
  const [racasDeGatos, setRacasDeGatos] = useState([]);
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(false);

  // --- Observadores para reatividade do formulário ---
  const especie = Form.useWatch("especie", form);
  const sexo = Form.useWatch("sexo", form);

  // --- Effect para carregar dados de suporte (raças e tutores) ---
  useEffect(() => {
    setLoading(true);
    Promise.all([
      racasAPI.listar(1),
      racasAPI.listar(2),
      tutoresDaClinicaAPI.listar(),
    ])
      .then(([caesData, gatosData, tutoresData]) => {
        setRacasDeCaes(caesData.map((r) => ({ label: r.nome, value: r.id })));
        setRacasDeGatos(gatosData.map((r) => ({ label: r.nome, value: r.id })));
        setTutores(
          tutoresData.map((t) => ({
            label: `${t.nome} (${t.cpf})`,
            value: t.id,
          }))
        );
      })
      .catch((err) => console.error("Erro ao carregar dados:", err))
      .finally(() => setLoading(false));
  }, []);

  // --- Effect para POPULAR O FORMULÁRIO e CORRIGIR A DATA ---
  useEffect(() => {
    if (valoresIniciais) {
      // Extrai os IDs dos tutores para popular o Select múltiplo
      const idsDosTutoresAtuais =
        valoresIniciais.tutores?.map((t) => t.id) || [];

      form.setFieldsValue({
        ...valoresIniciais,
        // AQUI ESTÁ A CORREÇÃO: Converte a string da API em um objeto dayjs
        nascimento: valoresIniciais.nascimento
          ? dayjs(valoresIniciais.nascimento)
          : null,
        tutorIds: idsDosTutoresAtuais,
      });
    }
  }, [valoresIniciais, form]);

  // Efeito para limpar o campo de raça quando a espécie muda
  useEffect(() => {
    // Evita limpar o campo na primeira renderização se já houver um valor inicial
    if (form.isFieldTouched("especie")) {
      form.setFieldsValue({ racaId: undefined });
    }
  }, [especie, form]);

  return (
    <Row gutter={24}>
      {/* Campos ocultos */}
      <Form.Item name="id" hidden>
        <Input />
      </Form.Item>

      <Col span={24}>
        <Divider orientation="left" plain>
          Identificação e Associação
        </Divider>
        <Form.Item
          label="Nome do Animal"
          name="nome"
          rules={[{ required: true }]}
        >
          <Input />
        </Form.Item>
        <Form.Item label="Tutor(es) Responsável(is)" name="tutorIds">
          <Select
            mode="multiple"
            showSearch
            placeholder="Busque e selecione os tutores"
            loading={loading}
            options={tutores}
            optionFilterProp="label"
          />
        </Form.Item>
        {/* <Form.Item
          label="Data de Nascimento"
          name="nascimento"
          rules={[{ required: true }]}
        >
          <DatePicker style={{ width: "100%" }} format="DD/MM/YYYY" />
        </Form.Item> */}

        <Divider orientation="left" plain>
          Características
        </Divider>
        <Form.Item label="Espécie" name="especie" rules={[{ required: true }]}>
          <Select options={opcoesEspecie} />
        </Form.Item>
        <Form.Item label="Raça" name="racaId" rules={[{ required: true }]}>
          <Select
            placeholder="Selecione a raça"
            disabled={!especie}
            loading={loading}
            options={especie === 1 ? racasDeCaes : racasDeGatos}
          />
        </Form.Item>
        <Form.Item label="Sexo" name="sexo" rules={[{ required: true }]}>
          <Select options={opcoesSexo} />
        </Form.Item>
        <Form.Item name="castrado" label="Castrado?" valuePropName="checked">
          <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
        </Form.Item>
      </Col>
    </Row>
  );
};

export default EdicaoAnimalDaClinica;
