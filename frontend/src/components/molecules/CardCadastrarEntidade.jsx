import React, { useState } from "react";
import { Card, Form, Input, Button, Select, message } from "antd";
import { PlusOutlined } from "@ant-design/icons";

const { Option } = Select;

const CardCadastrarEntidade = ({ entidade, campos, onSubmit, onSuccess }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      // Chama a função de submissão com os valores do formulário
      const response = await onSubmit(values);
      message.success(`${entidade} criado com sucesso.`);
      form.resetFields();
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar ${entidade}:`, error);
      message.error(`Falha ao cadastrar ${entidade}.`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card
      title={`Cadastrar ${entidade}`}
      extra={
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => form.submit()}
          loading={loading}
          style={{ backgroundColor: "var(--verde-medio)", borderColor: "var(--verde-medio)" }}
        >
          Cadastrar
        </Button>
      }
    >
      <Form form={form} layout="vertical" onFinish={handleFinish}>
        {campos.map((campo) => {
          switch (campo.type) {
            case "text":
            case "email":
            case "password":
              return (
                <Form.Item
                  key={campo.name}
                  name={campo.name}
                  label={campo.label}
                  rules={campo.rules || []}
                >
                  {campo.type === "password" ? (
                    <Input.Password placeholder={campo.placeholder} />
                  ) : (
                    <Input type={campo.type} placeholder={campo.placeholder} />
                  )}
                </Form.Item>
              );
            case "select":
              return (
                <Form.Item
                  key={campo.name}
                  name={campo.name}
                  label={campo.label}
                  rules={campo.rules || []}
                >
                  <Select placeholder={campo.placeholder}>
                    {campo.options.map((opcao) => (
                      <Option key={opcao.value} value={opcao.value}>
                        {opcao.label}
                      </Option>
                    ))}
                  </Select>
                </Form.Item>
              );
            default:
              return null;
          }
        })}
      </Form>
    </Card>
  );
};

export default CardCadastrarEntidade;
