import React, { useState } from "react";
import { Form, Button, message, Typography } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import FormularioCadastroClinica from "./Cadastro/CadastroClinicas";

const componentesDeCadastro = {
  clinicas: FormularioCadastroClinica
};

const FormCadastroEntidade = ({
  entidade,
  onSubmit,
  onSuccess,
  onCancel
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleFinish = async (values) => {
    setLoading(true);
    try {
      // A lógica de processamento específica da entidade agora pode viver no `onSubmit` do componente pai,
      // ou ser retornada pelo formulário filho. Manter aqui é mais simples por enquanto.
      const response = await onSubmit(values);
      message.success(
        "Cadastrado(a) com sucesso."
      );
      form.resetFields();
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar:`, error);
      message.error(
        error.response?.data?.message || `Falha ao realizar cadastro.`
      );
    } finally {
      setLoading(false);
    }
  };

  const CadastroEntidade = componentesDeCadastro[entidade];

  return (
    <div>
        <Form
        form={form}
        layout="vertical" // importante
        onFinish={handleFinish}
        autoComplete="off"
        >

        {CadastroEntidade ? (
            <CadastroEntidade form={form} /> // passamos o form para nosso componente filho que vai definir os form.items
        ) : (
            <Typography.Text type="danger">
            Formulário para a entidade '{entidade}' não foi implementado.
            </Typography.Text>
        )}

        <div className="flex gap-4">
            <Button onClick={onCancel} className="w-1/2">
            Cancelar
            </Button>
            <Button
            type="primary"
            htmlType="submit"
            icon={<PlusOutlined />}
            loading={loading}
            className="w-1/2"
            >
            Cadastrar
            </Button>
        </div>
        </Form>
    </div>
  );
};

export default FormCadastroEntidade;
