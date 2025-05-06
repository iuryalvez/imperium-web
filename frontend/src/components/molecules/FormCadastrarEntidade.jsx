import React, { useState, useEffect } from "react";
import { Input, Select, DatePicker, Checkbox, Button, Form, message, Typography, Switch } from "antd";
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

const FormCadastrarEntidade = ({ entidade, onSubmit, onSuccess, setModalVisivel, tituloModal, camposSimplesCadastrar }) => {
  const campos = camposSimplesCadastrar[entidade] || [];
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
        title={`${tituloModal}`}
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        className="space-y-3"
      >
        <Typography.Title level={4}>
          {tituloModal || "Cadastro de Entidade"}
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
              <>
                <Form.Item
                  label="Clínicas"
                  name="clinicaIds"
                  rules={[
                    { required: true, message: "Selecione pelo menos uma clínica" },
                  ]}
                >
                  <Select
                    mode="multiple"
                    showSearch
                    placeholder="Selecione uma ou mais clínicas"
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

                {/* <Alert
                  message="Atenção"
                  description="Após associar o usuário a uma ou mais clínicas, **não será possível remover essa associação.** Certifique-se antes de continuar."
                  type="warning"
                  showIcon
                  className="mb-4"
                /> */}
              </>
            )}
          </>
        )}

        {entidade === "usuariosDaClinica" && (
          <>
            <Form.Item
              label="Perfis"
              name="perfis"
              rules={[
                { required: true, message: "Pelo menos um perfil deve ser selecionado" },
              ]}
            >
              <Checkbox.Group
                options={perfisClinica.map((perfil) => ({
                  ...perfil,
                  disabled:
                    (inativoSelecionado && perfil.value !== "INATIVO")
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;

                  if (values.includes("INATIVO")) {
                    newValues = ["INATIVO"];
                  } else {
                    newValues = values.filter((val) => val !== "INATIVO");
                  }

                  setPerfisSelecionados(newValues);
                  form.setFieldValue("perfis", newValues);
                }}
              />
            </Form.Item>
          </>
        )}

          {entidade === "clinicas" && (
          <>
            {/* Switch Ativo/Inativo */}
            <Form.Item
              label="Status da Clínica"
              name="isAtiva"
              valuePropName="checked"
            >
              <Switch
                checkedChildren="ATIVA"
                unCheckedChildren="INATIVA"
                defaultUnChecked
              />
            </Form.Item>
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