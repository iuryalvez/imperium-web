import React from "react";
import { Form, Input, Switch, Typography } from "antd";

const FormularioCadastroClinica = () => {
  return (
    <Form layout="vertical">
      <Typography.Title level={4}>{`Cadastro de Clínica`}</Typography.Title>
      <Form.Item
        label="Nome da Clínica"
        name="nomeClinica"
        rules={[{ required: true, message: "O nome da clínica é obrigatório" }]}
      >
        <Input placeholder="Digite o nome da nova clínica" />
      </Form.Item>

      <Form.Item
        label="Status Inicial da Clínica"
        name="isAtiva"
        // 'valuePropName="checked"' é essencial para componentes como Switch e Checkbox
        valuePropName="checked"
        // É uma boa prática definir um valor inicial para o status
        initialValue={true}
      >
        <Switch checkedChildren="ATIVA" unCheckedChildren="INATIVA" />
      </Form.Item>
    </Form>
  );
};

export default FormularioCadastroClinica;
