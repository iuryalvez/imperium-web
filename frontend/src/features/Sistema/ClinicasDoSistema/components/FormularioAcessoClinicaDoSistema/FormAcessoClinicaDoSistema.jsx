import React from "react";
import { Button, Form, Checkbox } from "antd";

// Perfis que o admin pode simular. INATIVO não entra: um vínculo inativo não acessa nada da clínica
// (o backend também recusa INATIVO na simulação).
const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
];

const FormAcessarClinica = ({ clinica, onFinish, onCancel }) => {
  const [form] = Form.useForm();

  // Interceptamos o submit do Ant Design para extrair apenas o array
  const handleSubmit = (values) => {
    if (onFinish) {
      onFinish(values.perfis);
    }
  };

  return (
    <Form id="form-acesso-clinica" form={form} onFinish={handleSubmit} layout="vertical">
      <Form.Item
        name="perfis"
        label="Perfis de clínica para simular"
        rules={[{ required: true, message: "Selecione pelo menos um perfil." }]}
      >
        <Checkbox.Group
          options={perfisClinica}
          style={{ display: "flex", flexDirection: "column", gap: 8 }}
        />
      </Form.Item>
    </Form>
  );
};

export default FormAcessarClinica;
