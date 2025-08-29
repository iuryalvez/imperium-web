// src/components/formularios/FormEdicaoEntidade.jsx
import React, { useState, useEffect } from "react";
import { Button, Form, Typography, message } from "antd";
import { SaveOutlined } from "@ant-design/icons";

import EdicaoAnimaisDaClinica from "./Edicao/EdicaoAnimaisDaClinica";
import EdicaoTutoresDaClinica from "./Edicao/EdicaoTutoresDaClinica";
import EdicaoClinica from "./Edicao/EdicaoClinica";

const componentesDeEdicao = {
  animaisDaClinica: EdicaoAnimaisDaClinica,
  tutoresDaClinica: EdicaoTutoresDaClinica,
  clinicas: EdicaoClinica,
};

const FormEdicaoEntidade = ({
  entidade,
  valoresIniciais,
  onSubmit,
  onSuccess,
  onCancel,
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  // Efeito para popular o formulário quando os dados de edição são carregados
  useEffect(() => {
    if (valoresIniciais) {
      form.setFieldsValue(valoresIniciais);
    }
  }, [valoresIniciais, form]);

  const handleFinish = async (values) => {
    setLoading(true);
    try {
      // Inclui o ID nos valores a serem submetidos, caso não esteja visível
      const finalValues = { ...valoresIniciais, ...values };
      await onSubmit(finalValues);
      message.success(`Editado com sucesso.`);
      if (onSuccess) onSuccess();
    } catch (error) {
      console.error("Erro ao editar:", error);
      message.error(
        error.response?.data?.message || "Falha ao salvar as alterações."
      );
    } finally {
      setLoading(false);
    }
  };

  const EdicaoEntidade = componentesDeEdicao[entidade];

  return (
    <Form
      form={form}
      layout="vertical" // importante para visualização
      onFinish={handleFinish}
    >
      {EdicaoEntidade ? (
        // Passamos `form` e `valoresIniciais` para o componente filho que vai definir nossos form.items
        <EdicaoEntidade form={form} valoresIniciais={valoresIniciais} />
      ) : (
        <Typography.Text type="danger">
          Formulário de edição para '{entidade}' não foi implementado.
        </Typography.Text>
      )}

      <div className="flex gap-4">
        <Button onClick={onCancel} className="w-1/2">
          Cancelar
        </Button>
        <Button
          type="primary"
          htmlType="submit"
          icon={<SaveOutlined />}
          loading={loading}
          className="w-1/2"
        >
          Salvar Alterações
        </Button>
      </div>
    </Form>
  );
};

export default FormEdicaoEntidade;
