import React, { useState, useEffect } from "react";
import { Form, Input, Select, DatePicker, Switch, Row, Col, message, Checkbox } from "antd";
import { tutoresDaClinicaAPI } from "../../../../../api/clinica/tutoresDaClinicaAPI";
import { racasDaClinicaAPI } from "../../../../../api/clinica/racasDaClinicaAPI";
import SelectBuscaRemota from "../../../../../components/atoms/SelectBuscaRemota";

const opcoesEspecie = [{ value: 1, label: "Cão" }, { value: 2, label: "Gato" }];
const opcoesSexo = [{ value: 1, label: "Macho" }, { value: 2, label: "Fêmea" }];

const FormCadastroAnimal = ({ onFinish }) => {
  const [form] = Form.useForm();
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(false);

  const especieSelecionada = Form.useWatch("especie", form);
  const sobTutelaClinica = Form.useWatch("sobTutelaClinica", form);

  // Efeito para buscar raças e tutores quando o componente monta
  useEffect(() => {
    const carregarDadosDeSuporte = async () => {
      setLoading(true);
      try {
        // Tutores inativos aparecem desabilitados (não podem ganhar vínculo novo)
        const tutoresData = await tutoresDaClinicaAPI.listar();
        setTutores(tutoresData.map(t => ({
          label: `${t.nome} (${t.cpf || 'N/A'})${t.isAtivo === false ? ' (inativo)' : ''}`,
          value: t.id,
          disabled: t.isAtivo === false,
        })));

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
            {/* Raças do sistema + da clínica (sem as ocultas) da espécie escolhida; inativas aparecem desabilitadas */}
            <SelectBuscaRemota
              placeholder={!especieSelecionada ? "Primeiro, selecione a espécie" : "Busque e selecione a raça"}
              disabled={!especieSelecionada}
              buscarOpcoes={(busca) => racasDaClinicaAPI.opcoes(busca, especieSelecionada)}
              dependencias={[especieSelecionada]}
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