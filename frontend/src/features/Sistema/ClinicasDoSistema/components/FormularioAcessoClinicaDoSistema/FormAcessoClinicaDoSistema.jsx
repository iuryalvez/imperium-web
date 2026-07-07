import React from "react";
import { Button, Form, Checkbox } from "antd";

const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const FormAcessarClinica = ({ clinica, onFinish }) => {
  const [form] = Form.useForm();

  // Interceptamos o submit do Ant Design para extrair apenas o array
  const handleSubmit = (values) => {
    // Em vez de enviar o objeto inteiro { perfis: [...] }, enviamos só o array
    if (onFinish) {
      onFinish(values.perfis); 
    }
  };

  return (
    <Form form={form} onFinish={handleSubmit} layout="vertical">
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

      <div className="flex justify-end mt-4 pt-4 border-t border-gray-200">
        <Button type="primary" htmlType="submit">
          Acessar {clinica?.nomeClinica}
        </Button>
      </div>
    </Form>
  );
};

export default FormAcessarClinica;