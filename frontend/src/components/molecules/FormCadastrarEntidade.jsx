import React, { useState, useEffect } from "react";
import { Input, Select, DatePicker, Checkbox, Button, Form, message, Typography } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";

const perfisSistema = [
  { label: "Administrador", value: "ADMIN" },
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const camposCadastro = {
  usuariosDoSistema: [
    {
      name: "nome",
      label: "Nome",
      type: "text",
      placeholder: "Digite seu nome",
      rules: [{ type: "required", message: "Nome obrigatório" }],
    },
    {
      name: "email",
      label: "E-mail",
      type: "email",
      placeholder: "Digite seu e-mail",
      rules: [
        { type: "required", message: "E-mail obrigatório" },
        { type: "email", message: "Formato inválido" },
      ],
    },
    {
      name: "senha",
      label: "Senha",
      type: "password",
      placeholder: "Digite sua senha",
      rules: [
        { type: "required", message: "Senha obrigatória" },
        { type: "minLength", message: "Mínimo 6 caracteres" },
      ],
    },
  ],
  usuarios: [
    {
      name: "nome",
      label: "Nome",
      type: "text",
      placeholder: "Digite seu nome",
      rules: [{ type: "required", message: "Nome obrigatório" }],
    },
    {
      name: "email",
      label: "E-mail",
      type: "email",
      placeholder: "Digite seu e-mail",
      rules: [
        { type: "required", message: "E-mail obrigatório" },
        { type: "email", message: "Formato inválido" },
      ],
    },
    {
      name: "senha",
      label: "Senha",
      type: "password",
      placeholder: "Digite sua senha",
      rules: [
        { type: "required", message: "Senha obrigatória" },
        { type: "minLength", message: "Mínimo 6 caracteres" },
      ],
    },
    {
      name: "perfilId",
      label: "Perfil",
      type: "select",
      placeholder: "Selecione um perfil",
      options: [
        { label: "Gestor", value: 2 },
        { label: "Técnico", value: 5 },
        { label: "Secretário", value: 4 },
        { label: "Veterinário", value: 3 },
      ],
      rules: [{ type: "required", message: "Perfil obrigatório" }],
    },
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
  clinicas: [
      { name: "nomeClinica", label: "Nome da Clínica", type: "text", placeholder: "Digite o nome da clínica", rules: [{ type: "required", message: "O nome da clínica é obrigatório" }, { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres" }] }
  ]
};

const FormCadastrarEntidade = ({ entidade, onSubmit, onSuccess, setModalVisivel, tituloModal }) => {
  const campos = camposCadastro[entidade] || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [perfisSelecionados, setPerfisSelecionados] = useState([]);
  const [clinicas, setClinicas] = useState([]);

  useEffect(() => {
    if (entidade === "usuariosDoSistema") {
      clinicasAPI.listar()
      .then((data) => {
        // Aqui preparamos os dados no formato que o Select espera
        const clinicasFormatadas = data.map((clinica) => ({
          label: clinica.nomeClinica,
          value: clinica.id,
        }));
        setClinicas(clinicasFormatadas);
      })  
      .catch((err) => console.error("Erro ao carregar clínicas:", err));
    }
  }, [entidade]);

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      const response = await onSubmit(values);
      message.success(`${entidade} criado com sucesso.`);
      form.resetFields();
      setPerfisSelecionados([]);
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar ${entidade}:`, error);
      message.error(`Falha ao cadastrar ${entidade}.`);
    } finally {
      setLoading(false);
    }
  };

  const renderCampo = (campo) => {
    switch (campo.type) {
      case "text":
      case "email":
        return <Input type={campo.type} placeholder={campo.placeholder} />;
      case "password":
        return <Input.Password placeholder={campo.placeholder} />;
      case "select":
        return (
          <Select placeholder={campo.placeholder}>
            {campo.options?.map((opt) => (
              <Select.Option key={opt.value} value={opt.value}>
                {opt.label}
              </Select.Option>
            ))}
          </Select>
        );
      case "date":
        return <DatePicker style={{ width: "100%" }} />;
      case "boolean":
        return <Checkbox>{campo.placeholder}</Checkbox>;
      default:
        return null;
    }
  };

  const inativoSelecionado = perfisSelecionados.includes("INATIVO");
  const adminSelecionado = perfisSelecionados.includes("ADMIN");

  return (
    <div className="p-2">
      <Form
        title={`Cadastro de ${tituloModal}`}
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        className="space-y-3"
      >
        <Typography.Title level={4}>
          Cadastro de {tituloModal || "Entidade"}
        </Typography.Title>
        {campos.map((campo) => (
          <Form.Item
            key={campo.name}
            name={campo.name}
            label={campo.label}
            rules={campo.rules || []}
          >
            {renderCampo(campo)}
          </Form.Item>
        ))}

        {entidade === "usuariosDoSistema" && (
          <>
            <Form.Item
              label="Perfis"
              name="perfis"
              rules={[
                { required: true, message: "Pelo menos um perfil deve ser selecionado" },
              ]}
            >
              <Checkbox.Group
                options={perfisSistema.map((perfil) => ({
                  ...perfil,
                  disabled:
                    (inativoSelecionado && perfil.value !== "INATIVO") ||
                    (adminSelecionado && perfil.value !== "ADMIN"),
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;

                  if (values.includes("ADMIN")) {
                    newValues = ["ADMIN"];
                  } else if (values.includes("INATIVO")) {
                    newValues = ["INATIVO"];
                  } else {
                    newValues = values.filter((val) => val !== "ADMIN" && val !== "INATIVO");
                  }

                  setPerfisSelecionados(newValues);
                  form.setFieldValue("perfis", newValues);
                }}
              />
            </Form.Item>

            {!adminSelecionado && (
              <Form.Item
              label="Clínica"
              name="clinicaId"
              rules={[{ required: true, message: "Selecione a clínica" }]}
              >
                <Select
                showSearch
                placeholder="Selecione a clínica"
                optionFilterProp="label"
                filterOption={(input, option) =>
                  option.label.toLowerCase().includes(input.toLowerCase())
                }
                options={clinicas}
                >
                  {clinicas.map((clinica) => (
                    <Select.Option key={clinica.value} value={clinica.value}>
                      {clinica.label}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            )}
          </>
        )}
        {/* <Button onClick={() => {
          console.log(form.getFieldsValue());
        }}>
          Ver campos do formulário
        </Button> */}
        <div className="flex gap-4 justify-between">
          <Button
            onClick={() => setModalVisivel(false)}
            className="w-1/2"
            style={{ backgroundColor: "#d9d9d9", color: "#000" }}
          >
            Cancelar
          </Button>

          <Button
            type="primary"
            icon={<PlusOutlined />}
            loading={loading}
            className="w-1/2 bg-green-700 hover:bg-green-800 text-white"
            onClick={() => form.submit()}
          >
            Cadastrar
          </Button>
        </div>
      </Form>
    </div>
  );
};

export default FormCadastrarEntidade;