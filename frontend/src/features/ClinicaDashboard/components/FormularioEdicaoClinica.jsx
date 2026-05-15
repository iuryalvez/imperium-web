import React, { useEffect } from "react";
import { Form, Input, Button, Tabs, Spin } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import {
  CepSearchInput,
  CnpjMaskInput,
  TelefoneMaskInput,
} from "../../../utils/masks";
import { useCepHandler } from "../../../utils/useCepHandler";
import { validarCNPJ } from "../../../utils/validators";

const FormularioEdicaoClinica = ({ initialValues, onFinish, isSubmitting }) => {
  const [form] = Form.useForm();

  // Inicializa o hook passando a referência do formulário
  const { handleCepSearch, loadingCep } = useCepHandler(form);

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
    }
  }, [initialValues, form]);

  const items = [
    {
      key: "1",
      label: "Dados Gerais",
      children: (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
          <Form.Item
            name="nomeClinica"
            label="Razão Social (Nome)"
            rules={[{ required: true, message: "Obrigatório" }]}
          >
            <Input placeholder="Nome da clínica" maxLength={100} />
          </Form.Item>
          <Form.Item name="nomeFantasia" label="Nome Fantasia">
            <Input placeholder="Como a clínica é conhecida" maxLength={100} />
          </Form.Item>
          <Form.Item
            name="cnpj"
            label="CNPJ"
            rules={[
              { required: true, message: "O CNPJ é obrigatório" },
              () => ({
                validator(_, value) {
                  if (!value || validarCNPJ(value)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("CNPJ inválido"));
                },
              }),
            ]}
          >
            <CnpjMaskInput />
          </Form.Item>
          <Form.Item name="logoUrl" label="URL da Logo">
            <Input placeholder="https://link-da-imagem.com/logo.png" />
          </Form.Item>
        </div>
      ),
    },
    {
      key: "2",
      label: "Contato",
      children: (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
          <Form.Item
            name="email"
            label="E-mail Corporativo"
            rules={[{ type: "email", message: "E-mail inválido" }]}
          >
            <Input placeholder="contato@clinica.com" />
          </Form.Item>
          <Form.Item name="telefone" label="Telefone Fixo">
            <TelefoneMaskInput />
          </Form.Item>
          <Form.Item name="whatsapp" label="WhatsApp">
            <TelefoneMaskInput />
          </Form.Item>
          <Form.Item name="instagram" label="Instagram">
            <Input placeholder="@clinicaexemplo" maxLength={30} prefix="@" />
          </Form.Item>
        </div>
      ),
    },
    {
      key: "3",
      label: "Endereço",
      children: (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4 relative">
          {loadingCep && (
            <div className="absolute inset-0 z-10 flex items-center justify-center bg-white bg-opacity-70">
              <Spin
                indicator={<LoadingOutlined style={{ fontSize: 24 }} spin />}
              />
            </div>
          )}

          <Form.Item name="cep" label="CEP">
            <CepSearchInput onSearch={handleCepSearch} loading={loadingCep} />
          </Form.Item>

          <Form.Item
            name="logradouro"
            label="Logradouro"
            className="md:col-span-2"
          >
            <Input placeholder="Rua, Avenida, etc" />
          </Form.Item>
          <Form.Item name="complemento" label="Complemento">
            <Input placeholder="Sala, Lote, Quadra" />
          </Form.Item>
          <Form.Item name="bairro" label="Bairro">
            <Input placeholder="Bairro" />
          </Form.Item>
          <Form.Item name="cidade" label="Cidade">
            <Input placeholder="Cidade" />
          </Form.Item>
          <Form.Item
            name="uf"
            label="UF"
            rules={[{ len: 2, message: "Apenas 2 letras" }]}
          >
            <Input
              placeholder="EX: GO, SP, MG"
              className="uppercase"
              maxLength={2}
            />
          </Form.Item>
        </div>
      ),
    },
  ];

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Tabs defaultActiveKey="1" items={items} />

      <div className="flex justify-end mt-6 pt-4 border-t border-gray-200">
        <Button
          type="primary"
          htmlType="submit"
          loading={isSubmitting}
          size="large"
        >
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormularioEdicaoClinica;
