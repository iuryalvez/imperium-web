import React, { useEffect } from "react";
import { Form, Input, Button, Tabs } from "antd";

const FormularioEdicaoPerfilPessoal = ({ initialValues, onFinish, isSubmitting }) => {
  const [form] = Form.useForm();

  // Reset required to update the form when fetch completes or re-fetches
  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue({
        ...initialValues,
        senhaAtual: undefined,
        novaSenha: undefined,
        confirmarSenha: undefined,
      });
    }
  }, [initialValues, form]);

  const items = [
    {
      key: "1",
      label: "Dados Pessoais",
      children: (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-2">
          <Form.Item name="nome" label="Nome" rules={[{ required: true }]}>
            <Input placeholder="Seu nome completo" />
          </Form.Item>
          <Form.Item name="email" label="E-mail" rules={[{ type: 'email' }]}>
            <Input placeholder="seuemail@exemplo.com" />
          </Form.Item>
          <Form.Item name="formacao" label="Formação">
            <Input placeholder="Ex: Médico Veterinário" />
          </Form.Item>
          <Form.Item name="cpf" label="CPF">
            <Input placeholder="000.000.000-00" />
          </Form.Item>
          <Form.Item name="telefone" label="Telefone">
            <Input placeholder="(00) 0000-0000" />
          </Form.Item>
          <Form.Item name="whatsapp" label="WhatsApp">
            <Input placeholder="(00) 00000-0000" />
          </Form.Item>
        </div>
      ),
    },
    {
      key: "2",
      label: "Segurança",
      children: (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-2">
          <Form.Item name="senhaAtual" label="Senha Atual">
            <Input.Password placeholder="Digite sua senha atual" />
          </Form.Item>
          <Form.Item name="novaSenha" label="Nova Senha">
            <Input.Password placeholder="Digite a nova senha" />
          </Form.Item>
          <Form.Item
            name="confirmarSenha"
            label="Confirmar Nova Senha"
            dependencies={['novaSenha']}
            rules={[
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('novaSenha') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('As senhas não coincidem.'));
                },
              }),
            ]}
          >
            <Input.Password placeholder="Confirme a nova senha" />
          </Form.Item>
        </div>
      ),
    },
  ];

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={onFinish}
    >
      <Tabs defaultActiveKey="1" items={items} />
      
      <div className="flex justify-end mt-6">
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormularioEdicaoPerfilPessoal;