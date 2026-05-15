import React, { useState, useEffect } from "react";
import { Form, Input, Select, Row, Col, Divider, Button, Spin } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import {
  CepSearchInput,
  CpfMaskInput,
  TelefoneMaskInput,
} from "../../../../utils/masks";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";
import { useCepHandler } from "../../../../utils/useCepHandler";
import { validarCPF } from "../../../../utils/validators";

const FormularioEdicaoTutorDaClinica = ({
  initialValues,
  onFinish,
  isSubmitting,
}) => {
  const [form] = Form.useForm();
  const [animaisDaClinica, setAnimaisDaClinica] = useState([]);
  const [loading, setLoading] = useState(false);

  const { handleCepSearch, loadingCep } = useCepHandler(form);

  useEffect(() => {
    setLoading(true);
    animaisDaClinicaAPI
      .listar()
      .then((data) => {
        setAnimaisDaClinica(
          data.map((a) => ({
            label: `${a.nome} (${a.especie === 1 ? "Cão" : "Gato"})`,
            value: a.id,
          })),
        );
      })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (initialValues) {
      const idsDosAnimaisAtuais = initialValues.animais?.map((a) => a.id) || [];
      form.setFieldsValue({
        ...initialValues,
        animalIds: idsDosAnimaisAtuais,
        // Caso o backend retorne o campo como 'endereco', garantimos que vá para 'logradouro' no form
        logradouro: initialValues.logradouro || initialValues.endereco,
      });
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Dados Pessoais
          </Divider>
          <Form.Item
            label="Nome Completo"
            name="nome"
            rules={[{ required: true }]}
          >
            <Input placeholder="Digite o nome do tutor" />
          </Form.Item>
          <Form.Item
            label="CPF"
            name="cpf"
            rules={[
              { required: true, message: "O CPF é obrigatório" },
              () => ({
                validator(_, value) {
                  if (!value || validarCPF(value)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("CPF inválido"));
                },
              }),
            ]}
          >
            <CpfMaskInput />
          </Form.Item>
          <Form.Item
            label="E-mail"
            name="email"
            rules={[{ required: true }, { type: "email" }]}
          >
            <Input placeholder="exemplo@email.com" />
          </Form.Item>

          <Divider orientation="left" plain>
            Contato
          </Divider>
          <Form.Item label="Telefone Principal" name="telefone">
            <TelefoneMaskInput />
          </Form.Item>
          <Form.Item label="WhatsApp" name="whatsapp">
            <TelefoneMaskInput />
          </Form.Item>
        </Col>

        <Col xs={24} lg={12} className="relative">
          {loadingCep && (
            <div className="absolute inset-0 z-10 flex items-center justify-center bg-white bg-opacity-70 rounded-md">
              <Spin
                indicator={<LoadingOutlined style={{ fontSize: 24 }} spin />}
              />
            </div>
          )}

          <Divider orientation="left" plain>
            Endereço (Opcional)
          </Divider>
          <Form.Item label="CEP" name="cep">
            <CepSearchInput onSearch={handleCepSearch} loading={loadingCep} />
          </Form.Item>

          <Form.Item label="Endereço" name="logradouro">
            <Input placeholder="Rua, Avenida, etc." />
          </Form.Item>

          <Row gutter={12}>
            <Col span={8}>
              <Form.Item label="Número" name="endNumero">
                <Input placeholder="Nº" />
              </Form.Item>
            </Col>
            <Col span={16}>
              <Form.Item label="Bairro" name="bairro">
                <Input placeholder="Bairro" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={12}>
            <Col span={18}>
              <Form.Item label="Cidade" name="cidade">
                <Input placeholder="Cidade" />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item label="UF" name="uf">
                <Input placeholder="UF" maxLength={2} className="uppercase" />
              </Form.Item>
            </Col>
          </Row>

          <Divider orientation="left" plain>
            Animais Associados
          </Divider>
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
