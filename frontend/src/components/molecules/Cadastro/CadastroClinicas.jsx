import React from "react";
import { Form, Input, Switch } from "antd";

const FormularioCadastroClinica = () => {
  return (
    <div>
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
    </div>
  );
};

export default FormularioCadastroClinica;
