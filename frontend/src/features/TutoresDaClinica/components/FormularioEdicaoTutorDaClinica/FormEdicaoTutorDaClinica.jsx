import React, { useState, useEffect } from "react";
import { Form, Input, Select, Row, Col, Divider, Button } from "antd";
import { CepMaskInput, CpfMaskInput, TelefoneMaskInput } from "../../../../utils/masks";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";

const FormularioEdicaoTutorDaClinica = ({ initialValues, onFinish, isSubmitting }) => {
  const [form] = Form.useForm();
  const [animaisDaClinica, setAnimaisDaClinica] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    animaisDaClinicaAPI.listar()
      .then((data) => {
        setAnimaisDaClinica(data.map((a) => ({
          label: `${a.nome} (${a.especie === 1 ? "Cão" : "Gato"})`,
          value: a.id,
        })));
      })
      .finally(() => setLoading(false));
  }, []);

  // Sets initial form state matching DTO parameters
  useEffect(() => {
    if (initialValues) {
      const idsDosAnimaisAtuais = initialValues.animais?.map((a) => a.id) || [];
      form.setFieldsValue({
        ...initialValues,
        animalIds: idsDosAnimaisAtuais,
      });
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>Dados Pessoais</Divider>
          <Form.Item label="Nome Completo" name="nome" rules={[{ required: true }]}>
            <Input placeholder="Digite o nome do tutor" />
          </Form.Item>
          <Form.Item label="CPF" name="cpf" rules={[{ required: true }]}>
            <CpfMaskInput />
          </Form.Item>
          <Form.Item label="E-mail" name="email" rules={[{ required: true }, { type: "email" }]}>
            <Input placeholder="exemplo@email.com" />
          </Form.Item>

          <Divider orientation="left" plain>Contato</Divider>
          <Form.Item label="Telefone Principal" name="telefone">
             <TelefoneMaskInput />
          </Form.Item>
          <Form.Item label="WhatsApp" name="whatsapp">
             <TelefoneMaskInput />
          </Form.Item>
        </Col>

        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>Endereço (Opcional)</Divider>
          <Form.Item label="CEP" name="cep">
            <CepMaskInput />
          </Form.Item>
          <Form.Item label="Endereço" name="endereco">
            <Input placeholder="Rua, Avenida, etc." />
          </Form.Item>
          <Form.Item label="Número" name="endNumero">
            <Input placeholder="Nº" />
          </Form.Item>

          <Divider orientation="left" plain>Animais Associados</Divider>
          <Form.Item label="Animais" name="animalIds">
            <Select
              mode="multiple"
              showSearch
              placeholder="Busque e selecione os animais"
              loading={loading}
              options={animaisDaClinica}
              optionFilterProp="label"
            />
          </Form.Item>
        </Col>
      </Row>
      <div className="flex justify-end mt-4">
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormularioEdicaoTutorDaClinica;