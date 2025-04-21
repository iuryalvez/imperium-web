import { Button, Form, Checkbox, message } from "antd";
import { useState } from "react";
import { autenticacaoAPI } from "../../api/autenticacaoAPI";

const perfisClinica = [
    { label: "Gestor", value: "GESTOR" },
    { label: "Técnico", value: "TECNICO" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Veterinário", value: "VETERINARIO" },
    { label: "Inativo", value: "INATIVO" },
  ];  

const FormAcessarClinica = ({ clinica, onSucesso }) => {
  const [form] = Form.useForm();
  const [carregando, setCarregando] = useState(false);

  const handleSubmit = async ({ perfisClinica }) => {
    setCarregando(true);
    try {
      await autenticacaoAPI.escolherVinculoAdmin({
        clinicaId: clinica.id,
        perfisClinica,
      });
      message.success("Vínculo criado com sucesso!");
      onSucesso(); // Fecha modal ou atualiza interface
    } catch (err) {
      message.error("Erro ao acessar a clínica.");
    } finally {
      setCarregando(false);
    }
  };

  return (
    <Form form={form} onFinish={handleSubmit} layout="vertical">
      <Form.Item
        name="perfisClinica"
        label="Perfis de clínica para simular"
        rules={[{ required: true, message: "Selecione pelo menos um perfil." }]}
      >
        <Checkbox.Group options={perfisClinica} />
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" loading={carregando}>
          Acessar Clínica
        </Button>
      </Form.Item>
    </Form>
  );
};

export default FormAcessarClinica;
