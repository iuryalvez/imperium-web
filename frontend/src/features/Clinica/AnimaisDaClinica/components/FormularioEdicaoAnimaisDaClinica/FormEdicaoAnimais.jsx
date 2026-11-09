import React, { useEffect } from "react";
import {
  Form,
  Input,
  Select,
  DatePicker,
  Switch,
  Row,
  Col,
  Checkbox,
  Alert,
} from "antd";
import dayjs from "dayjs";
import { tutoresDaClinicaAPI } from "../../../../../api/clinica/tutoresDaClinicaAPI";
import { racasDaClinicaAPI } from "../../../../../api/clinica/racasDaClinicaAPI";
import SelectBuscaRemota from "../../../../../components/atoms/SelectBuscaRemota";
import { useAuth } from "../../../../../services/AuthContext";
import { formatarIdade } from "../../../../../utils/formatters";

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

  const especieSelecionada = Form.useWatch("especie", form);
  const sobTutelaClinica = Form.useWatch("sobTutelaClinica", form);
  const nascimento = Form.useWatch("nascimento", form);
  const idade = formatarIdade(nascimento);
  // Só o admin marca ou desmarca a tutela da clínica. Os outros perfis não veem o checkbox,
  // mas um animal que já está sob tutela continua assim (aviso no lugar, tutor não obrigatório).
  const { isAdmin } = useAuth();
  const jaSobTutela = (initialValues?.tutorIds || []).length === 0;

  // Efeito para preencher o formulário com os valores iniciais
  useEffect(() => {
    if (initialValues) {
      const tutorIds = initialValues.tutorIds || [];
      form.setFieldsValue({
        ...initialValues,
        nascimento: initialValues.nascimento
          ? dayjs(initialValues.nascimento)
          : null,
        // O campo tutorIds já vem como uma lista de IDs, então não precisa de mapeamento
        tutorIds: tutorIds,
        sobTutelaClinica: tutorIds.length === 0,
      });
    }
  }, [initialValues, form]);

  // A raça atual sempre tem rótulo no select, mesmo inativa ou oculta
  const racaAtual = initialValues?.racaId
    ? [{ id: initialValues.racaId, nome: initialValues.racaNome || `Raça #${initialValues.racaId}` }]
    : [];

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
            {/* Raças do sistema + da clínica (sem as ocultas) da espécie; inativas aparecem desabilitadas */}
            <SelectBuscaRemota
              placeholder={
                !especieSelecionada
                  ? "Primeiro, selecione a espécie"
                  : "Busque e selecione a raça"
              }
              disabled={!especieSelecionada}
              buscarOpcoes={(busca) => racasDaClinicaAPI.opcoes(busca, especieSelecionada)}
              opcoesAtuais={racaAtual}
              dependencias={[especieSelecionada]}
            />
          </Form.Item>
          {/* O campo fica no form (escondido) para o valor continuar valendo na validação dos tutores */}
          <Form.Item name="sobTutelaClinica" valuePropName="checked" style={{ marginBottom: 8 }} hidden={!isAdmin}>
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
          {!isAdmin && jaSobTutela && (
            <Alert
              className="mb-4"
              type="info"
              showIcon
              message="Animal sob tutela da clínica"
            />
          )}
          <Form.Item 
            name="tutorIds" 
            label="Tutor(es) Responsável(is)"
            rules={[{ required: !sobTutelaClinica, message: 'Selecione um tutor ou a tutela da clínica' }]}
          >
            {/* Tutores da clínica com busca no servidor; inativos aparecem desabilitados */}
            <SelectBuscaRemota
              mode="multiple"
              placeholder={sobTutelaClinica ? "Animal sem tutor físico" : "Busque e selecione os tutores"}
              buscarOpcoes={tutoresDaClinicaAPI.opcoes}
              opcoesAtuais={initialValues?.tutores}
              disabled={sobTutelaClinica}
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            name="nascimento"
            label="Data de Nascimento"
            rules={[{ required: true }]}
            extra={idade && `Idade: ${idade}`}
          >
            <DatePicker
              style={{ width: "100%" }}
              format="DD/MM/YYYY"
              disabledDate={(data) => data.isAfter(dayjs(), "day")}
            />
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
