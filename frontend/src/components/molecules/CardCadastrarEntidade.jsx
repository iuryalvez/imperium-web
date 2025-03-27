import React, { useState } from "react";
import { Input, Select, DatePicker, Checkbox, Card, Button, Form, message } from "antd";
import { PlusOutlined } from "@ant-design/icons";

const camposCadastro = {
  usuariosDoSistema: [
      { name: "nome", label: "Nome", type: "text", placeholder: "Digite seu nome", rules: [{ type: "required", message: "Nome obrigatório" }] },
      { name: "email", label: "E-mail", type: "email", placeholder: "Digite seu e-mail", rules: [{ type: "required", message: "E-mail obrigatório" }, { type: "email", message: "Formato inválido" }] },
      { name: "senha", label: "Senha", type: "password", placeholder: "Digite sua senha", rules: [{ type: "required", message: "Senha obrigatória" }, { type: "minLength", message: "Mínimo 6 caracteres" }] },
      { name: "perfil", label: "Perfil", type: "select", placeholder: "Selecione um perfil", options: [
          { label: "Administrador", value: "ADMIN" },
          { label: "Gestor", value: "GESTOR" },
          { label: "Técnico", value: "TECNICO" },
          { label: "Secretário", value: "SECRETARIO" },
          { label: "Veterinário", value: "VETERINARIO" }
      ], rules: [{ type: "required", message: "Perfil obrigatório" }] }
  ],
  usuarios: [
      { name: "nome", label: "Nome", type: "text", placeholder: "Digite seu nome", rules: [{ type: "required", message: "Nome obrigatório" }] },
      { name: "email", label: "E-mail", type: "email", placeholder: "Digite seu e-mail", rules: [{ type: "required", message: "E-mail obrigatório" }, { type: "email", message: "Formato inválido" }] },
      { name: "senha", label: "Senha", type: "password", placeholder: "Digite sua senha", rules: [{ type: "required", message: "Senha obrigatória" }, { type: "minLength", message: "Mínimo 6 caracteres" }] },
      { name: "perfilId", label: "Perfil", type: "select", placeholder: "Selecione um perfil", options: [
          { label: "Gestor", value: 2 },
          { label: "Técnico", value: 5 },
          { label: "Secretário", value: 4 },
          { label: "Veterinário", value: 3 }
      ], rules: [{ type: "required", message: "Perfil obrigatório" }] }
  ],
  animais: [
      { name: "nome", label: "Nome do Animal", type: "text", placeholder: "Digite o nome", rules: [{ type: "required", message: "Nome obrigatório" }] },
      { name: "especie", label: "Espécie", type: "select", placeholder: "Selecione a espécie", options: [
          { label: "Cão", value: "1" },
          { label: "Gato", value: "2" }
      ], rules: [{ type: "required", message: "Espécie obrigatória" }] },
      { name: "raca", label: "Raça", type: "select", placeholder: "Selecione a raça", options: [] },
      { name: "nascimento", label: "Data de Nascimento", type: "date", placeholder: "Selecione a data de nascimento" },
      { name: "sexo", label: "Sexo", type: "select", placeholder: "Selecione o sexo", options: [
          { label: "Macho", value: "1" },
          { label: "Fêmea", value: "2" }
      ] },
      { name: "castrado", label: "Castrado", type: "boolean", placeholder: "O animal é castrado?" },
      { name: "peso", label: "Peso (kg)", type: "number", placeholder: "Digite o peso do animal" },
      { name: "condicaoCorporal", label: "Condição Corporal", type: "select", placeholder: "Selecione a condição corporal", options: Array.from({ length: 10 }, (_, i) => ({ label: (i + 1).toString(), value: (i + 1).toString() })) },
      { name: "grauAtividade", label: "Grau de Atividade", type: "select", placeholder: "Selecione o nível de atividade", options: [
          { label: "Inativo", value: "1" },
          { label: "Moderado", value: "2" },
          { label: "Ativo", value: "3" }
      ] },
      { name: "gestacao", label: "Está em gestação?", type: "boolean", placeholder: "O animal está gestante?" },
      { name: "lactacao", label: "Número de Filhotes", type: "number", placeholder: "Digite o número de filhotes (se aplicável)" },
      { name: "semanaLactacao", label: "Semana de Lactação", type: "number", placeholder: "Digite a semana de lactação (se aplicável)" },
      { name: "semanaGestacao", label: "Semana de Gestação", type: "number", placeholder: "Digite a semana de gestação (se aplicável)" }
  ],
  tutores: [
      { name: "nome", label: "Nome Completo", type: "text", placeholder: "Digite o nome completo", rules: [{ type: "required", message: "O nome é obrigatório." }, { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres." }] },
      { name: "cpf", label: "CPF", type: "text", placeholder: "Digite o CPF (somente números)", rules: [{ type: "required", message: "O CPF é obrigatório." }, { type: "exactLength", value: 11, message: "O CPF deve ter 11 caracteres." }] },
      { name: "email", label: "E-mail", type: "email", placeholder: "Digite o e-mail", rules: [{ type: "required", message: "O e-mail é obrigatório." }, { type: "email", message: "O e-mail não é válido." }] }
  ],
  clinicasDoSistema: [
      { name: "nomeClinica", label: "Nome da Clínica", type: "text", placeholder: "Digite o nome da clínica", rules: [{ type: "required", message: "O nome da clínica é obrigatório" }, { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres" }] }
  ]
};

const CardCadastrarEntidade = ({ entidade, onSubmit, onSuccess, setModalVisivel }) => {
  const campos = camposCadastro[entidade] || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      const response = await onSubmit(values); // chamando a função onSubmit definida na TabelaEntidade
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
    <Card>
      <Form form={form} layout="vertical" onFinish={handleFinish}>
        {campos.map((campo) => (
          <Form.Item
            key={campo.name}
            name={campo.name}
            label={campo.label}
            rules={campo.rules || []}
          >
            {campo.type === "text" || campo.type === "email" || campo.type === "password" ? (
              campo.type === "password" ? (
                <Input.Password placeholder={campo.placeholder} />
              ) : (
                <Input type={campo.type} placeholder={campo.placeholder} />
              )
            ) : campo.type === "select" ? (
              <Select placeholder={campo.placeholder}>
                {campo.options.map((option) => (
                  <Select.Option key={option.value} value={option.value}>
                    {option.label}
                  </Select.Option>
                ))}
              </Select>
            ) : campo.type === "date" ? (
              <DatePicker style={{ width: "100%" }} />
            ) : campo.type === "boolean" ? (
              <Checkbox>{campo.placeholder}</Checkbox>
            ) : null}
          </Form.Item>
        ))}
        <Form.Item style={{ display: "flex", justifyContent: "space-between" }}>
          <Button
            type="primary"
            onClick={() => {
              form.resetFields(); // Limpa o formulário
              setModalVisivel(false);
            }}
            style={{ flex: 1, backgroundColor: "gray", marginLeft: "8px", marginRight: "8px" }}
          >
            Cancelar
          </Button>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => form.submit()}
            loading={loading}
            style={{ flex: 1, backgroundColor: "var(--verde-medio)" }}
          >
            Cadastrar
          </Button>
      </Form.Item>
      </Form>
    </Card>
  );
};

export default CardCadastrarEntidade;
