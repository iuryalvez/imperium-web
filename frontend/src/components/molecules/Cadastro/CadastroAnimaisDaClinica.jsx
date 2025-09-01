import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  Select,
  DatePicker,
  Switch,
  Row,
  Col
} from "antd";
import { racasAPI } from "../../../api/sistema/racasAPI"; // Ajuste o caminho
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI"; // Ajuste o caminho

// --- Opções para os Selects ---
const opcoesEspecie = [
  { value: 1, label: "Cão" },
  { value: 2, label: "Gato" },
];
const opcoesSexo = [
  { value: 1, label: "Macho" },
  { value: 2, label: "Fêmea" },
];

const FormularioCadastroAnimaisDaClinica = ({ form }) => {
  // --- State para dados assíncronos ---
  const [racasDeCaes, setRacasDeCaes] = useState([]);
  const [racasDeGatos, setRacasDeGatos] = useState([]);
  const [loadingRacas, setLoadingRacas] = useState(false);
  const [tutores, setTutores] = useState([]);
  const [loadingTutores, setLoadingTutores] = useState(false);

  // --- Observadores para reatividade do formulário ---
  const especie = Form.useWatch("especie", form);

  // --- Effects para buscar dados ---
  useEffect(() => {
    setLoadingRacas(true);
    setLoadingTutores(true);
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
            label: `${t.nome} (${t.cpf || "CPF não informado"})`,
            value: t.id,
          }))
        );
      })
      .catch((err) => console.error("Erro ao carregar dados:", err))
      .finally(() => {
        setLoadingRacas(false);
        setLoadingTutores(false);
      });
  }, []);

  // Limpa o campo de raça quando a espécie muda
  useEffect(() => {
    form.setFieldsValue({ racaId: undefined });
  }, [especie, form]);

  return (
    <Row gutter={24}>
      <Col span={24}>
        <Form.Item
          label="Nome do Animal"
          name="nome"
          rules={[{ required: true, message: "O nome é obrigatório" }]}
        >
          <Input placeholder="Ex: Bob" />
        </Form.Item>
        <Form.Item
          label="Espécie"
          name="especie"
          rules={[{ required: true, message: "A espécie é obrigatória" }]}
        >
          <Select options={opcoesEspecie} placeholder="Selecione a espécie" />
        </Form.Item>
        <Form.Item
          label="Raça"
          name="racaId"
          rules={[{ required: true, message: "A raça é obrigatória" }]}
        >
          <Select
            placeholder={
              !especie ? "Primeiro, selecione a espécie" : "Selecione a raça"
            }
            disabled={!especie}
            loading={loadingRacas}
            options={especie === 1 ? racasDeCaes : racasDeGatos}
          />
        </Form.Item>
        <Form.Item
          label="Data de Nascimento"
          name="nascimento"
          rules={[{ required: true, message: "A data é obrigatória" }]}
        >
          <DatePicker
            style={{ width: "100%" }}
            format="DD/MM/YYYY"
            placeholder="Selecione a data"
          />
        </Form.Item>
        <Form.Item
          label="Sexo"
          name="sexo"
          rules={[{ required: true, message: "O sexo é obrigatório" }]}
        >
          <Select options={opcoesSexo} placeholder="Selecione o sexo" />
        </Form.Item>
        <Form.Item
          name="isCastrado"
          label="Castrado?"
          valuePropName="checked"
          initialValue={false}
        >
          <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
        </Form.Item>
        <Form.Item label="Tutor(es) Responsável(is)" name="tutorIds">
          <Select
            mode="multiple"
            showSearch
            placeholder="Busque por nome ou CPF (opcional)"
            loading={loadingTutores}
            options={tutores}
            optionFilterProp="label"
          />
        </Form.Item>
      </Col>
    </Row>
  );
};

export default FormularioCadastroAnimaisDaClinica;
