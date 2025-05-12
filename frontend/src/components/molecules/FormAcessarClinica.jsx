import { Button, Form, Checkbox, message } from "antd";
import { useState } from "react";
import { useAuth } from "../../services/AuthContext";

const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];


const FormAcessarClinica = ({ clinica, onSucesso }) => {
    const { escolherVinculoAdmin } = useAuth();
    const [perfisSelecionados, setPerfisSelecionados] = useState([]);
    const [form] = Form.useForm();
    const [carregando, setCarregando] = useState(false);
    
    const handleSubmit = async ({ perfis }) => {
        setCarregando(true);
        
        try {
            await escolherVinculoAdmin(clinica.id, perfis, clinica.nomeClinica);
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
            name="perfis"
            label="Perfis de clínica para simular"
            rules={[{ required: true, message: "Selecione pelo menos um perfil." }]}
        >
            <Checkbox.Group
                options={perfisClinica.map((perfil) => ({
                    ...perfil
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                    let newValues = values;
                    setPerfisSelecionados(newValues);
                    form.setFieldValue("perfis", newValues);
                }}
            />
        </Form.Item>

        <Form.Item>
            <Button type="primary" htmlType="submit" loading={carregando}>
            Acessar {clinica.nomeClinica}
            </Button>
        </Form.Item>
        </Form>
    );
};

export default FormAcessarClinica;
