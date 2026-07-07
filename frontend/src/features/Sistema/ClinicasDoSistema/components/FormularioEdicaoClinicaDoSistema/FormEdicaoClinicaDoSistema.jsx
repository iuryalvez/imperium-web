import React, { useEffect } from "react";
import { Form, Input, Row, Col, Divider, Button, Switch, Spin } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import { useCepHandler } from "../../../../../utils/useCepHandler";
import { validarCNPJ } from "../../../../../utils/validators";
import { CepSearchInput, CnpjMaskInput, TelefoneMaskInput } from "../../../../../utils/masks";

const FormularioEdicaoClinicaDoSistema = ({ initialValues, onFinish, isSubmitting }) => {
  const [form] = Form.useForm();
  const { handleCepSearch, loadingCep } = useCepHandler(form);

  useEffect(() => {
    if (initialValues) form.setFieldsValue(initialValues);
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24} className="relative">
        {loadingCep && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white bg-opacity-60 rounded-md">
            <Spin indicator={<LoadingOutlined style={{ fontSize: 24 }} spin />} />
          </div>
        )}
        
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>Identificação</Divider>
          <Form.Item label="Razão Social" name="nomeClinica" rules={[{ required: true }]}>
            <Input placeholder="Nome oficial" maxLength={100} />
          </Form.Item>
          <Form.Item label="Nome Fantasia" name="nomeFantasia">
            <Input placeholder="Nome popular" maxLength={100} />
          </Form.Item>
          <Form.Item label="CNPJ" name="cnpj" rules={[
            { validator: (_, v) => !v || validarCNPJ(v) ? Promise.resolve() : Promise.reject("CNPJ Inválido") }
          ]}>
            <CnpjMaskInput />
          </Form.Item>

          <Divider orientation="left" plain>Contato</Divider>
          <Form.Item label="E-mail" name="email" rules={[{ type: 'email', message: "E-mail inválido" }]}>
            <Input placeholder="contato@clinica.com" />
          </Form.Item>
          <Form.Item label="Telefone" name="telefone"><TelefoneMaskInput /></Form.Item>
          <Form.Item label="WhatsApp" name="whatsapp"><TelefoneMaskInput /></Form.Item>
          <Form.Item label="Instagram" name="instagram"><Input placeholder="clinica" prefix="@" maxLength={30} /></Form.Item>
        </Col>

        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>Endereço</Divider>
          <Form.Item label="CEP" name="cep">
            <CepSearchInput onSearch={handleCepSearch} loading={loadingCep} />
          </Form.Item>
          <Form.Item label="Logradouro (Rua/Av)" name="logradouro"><Input placeholder="Ex: Avenida Paulista" /></Form.Item>
          <Form.Item label="Complemento/Número" name="complemento"><Input placeholder="Ex: Nº 100, Sala 2" /></Form.Item>
          <Form.Item label="Bairro" name="bairro"><Input /></Form.Item>
          <Row gutter={8}>
            <Col span={16}><Form.Item label="Cidade" name="cidade"><Input /></Form.Item></Col>
            <Col span={8}><Form.Item label="UF" name="uf"><Input maxLength={2} className="uppercase" /></Form.Item></Col>
          </Row>
          <Form.Item label="Estado (Nome Completo)" name="estado"><Input placeholder="Ex: São Paulo" /></Form.Item>

          <Divider orientation="left" plain>Status</Divider>
          <Form.Item label="Status da Clínica" name="isAtiva" valuePropName="checked">
            <Switch checkedChildren="ATIVA" unCheckedChildren="INATIVA" />
          </Form.Item>
        </Col>
      </Row>
      <div className="flex justify-end mt-4 pt-4 border-t border-gray-200">
        <Button type="primary" htmlType="submit" loading={isSubmitting}>Salvar Alterações</Button>
      </div>
    </Form>
  );
};

export default FormularioEdicaoClinicaDoSistema;