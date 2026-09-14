import React, { useState, useEffect } from "react";
import { Form, Input, Select, DatePicker, Switch, Row, Col, message, Checkbox } from "antd";
import { tutoresDaClinicaAPI } from "../../../../../api/clinica/tutoresDaClinicaAPI";
import { racasAPI } from "../../../../../api/sistema/racasAPI";

const opcoesEspecie = [{ value: 1, label: "Cão" }, { value: 2, label: "Gato" }];
const opcoesSexo = [{ value: 1, label: "Macho" }, { value: 2, label: "Fêmea" }];

const FormCadastroAnimal = ({ onFinish }) => {
  const [form] = Form.useForm();
  const [racas, setRacas] = useState([]);
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(false);

  const especieSelecionada = Form.useWatch("especie", form);
  const sobTutelaClinica = Form.useWatch("sobTutelaClinica", form);

  // Efeito para buscar raças e tutores quando o componente monta
  useEffect(() => {
    const carregarDadosDeSuporte = async () => {
      setLoading(true);
      try {
        // Busca tutores e todas as raças em paralelo
        const [tutoresData, racasData] = await Promise.all([
          tutoresDaClinicaAPI.listar(),
          racasAPI.listar(), // Supondo um método que busca todas as raças
        ]);
        
        setTutores(tutoresData.map(t => ({ label: `${t.nome} (${t.cpf || 'N/A'})`, value: t.id })));
        setRacas(racasData.map(r => ({ label: r.nome, value: r.id, especie: r.especie })));

      } catch (err) {
        message.error("Falha ao carregar dados de suporte (raças/tutores).");
      } finally {
        setLoading(false);
      }
    };
    carregarDadosDeSuporte();
  }, []);

  // Limpa o campo 'racaId' sempre que a espécie mudar
  useEffect(() => {
    form.setFieldsValue({ racaId: undefined });
  }, [especieSelecionada, form]);
  
  return (
    <Form id="form-cadastro-animal-clinica" form={form} layout="vertical" onFinish={onFinish} initialValues={{ isCastrado: false, sobTutelaClinica: false }}>
      <Row gutter={24}>
        <Col xs={24} md={12}>
          <Form.Item name="nome" label="Nome do Animal" rules={[{ required: true }]}>
            <Input placeholder="Ex: Bob" />
          </Form.Item>
          <Form.Item name="especie" label="Espécie" rules={[{ required: true }]}>
            <Select options={opcoesEspecie} placeholder="Selecione a espécie" />
          </Form.Item>
          <Form.Item name="racaId" label="Raça" rules={[{ required: true }]}>
            <Select
              placeholder={!especieSelecionada ? "Primeiro, selecione a espécie" : "Selecione a raça"}
              disabled={!especieSelecionada}
              loading={loading}
              options={racas.filter(r => r.especie === (especieSelecionada === 1 ? 'Cao' : 'Gato'))}
              showSearch
              optionFilterProp="label"
            />
          </Form.Item>
          <Form.Item name="sobTutelaClinica" valuePropName="checked" style={{ marginBottom: 8 }}>
            <Checkbox
              onChange={(e) => {
                if (e.target.checked) {
                  form.setFieldsValue({ tutorIds: [] });
                }
              }}
            >
              Animal sob tutela da clínica
            </Checkbox>
          </Form.Item>
          <Form.Item 
            name="tutorIds" 
            label="Tutor(es) Responsável(is)"
            rules={[{ required: !sobTutelaClinica, message: 'Selecione um tutor ou a tutela da clínica' }]}
          >
            <Select
              mode="multiple"
              placeholder={sobTutelaClinica ? "Animal sem tutor físico" : "Busque e selecione os tutores"}
              loading={loading}
              options={tutores}
              optionFilterProp="label"
              disabled={sobTutelaClinica}
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item name="nascimento" label="Data de Nascimento" rules={[{ required: true }]}>
            <DatePicker style={{ width: "100%" }} format="DD/MM/YYYY" placeholder="Selecione a data" />
          </Form.Item>
          <Form.Item name="sexo" label="Sexo" rules={[{ required: true }]}>
            <Select options={opcoesSexo} placeholder="Selecione o sexo" />
          </Form.Item>
          <Form.Item name="isCastrado" label="Castrado?" valuePropName="checked">
            <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
          </Form.Item>
        </Col>
      </Row>
    </Form>
  );
};

export default FormCadastroAnimal;