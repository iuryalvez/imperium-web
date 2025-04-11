import React, { useState, useEffect } from "react";
import { Input, Select, DatePicker, Checkbox, Button, Form, message, Typography } from "antd";
import { SaveOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
  
const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const FormEditarEntidade = ({
  entidade,
  onSubmit,
  onSuccess,
  setModalVisivel,
  camposEditar,
  valoresIniciais,
  tituloModal,
}) => {
  const campos = camposEditar[entidade] || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [perfisSelecionados, setPerfisSelecionados] = useState(valoresIniciais?.perfis || []);
  const [clinicas, setClinicas] = useState([]);

  useEffect(() => {
    if (entidade === "usuariosDoSistema") {
      clinicasAPI.listar()
        .then((data) => {
          const clinicasFormatadas = data.map((clinica) => ({
            label: clinica.nomeClinica,
            value: clinica.id,
          }));
          setClinicas(clinicasFormatadas);
        })
        .catch((err) => console.error("Erro ao carregar clínicas:", err));
    }
  }, [entidade]);

  useEffect(() => {
    if (valoresIniciais) {
      form.setFieldsValue(valoresIniciais);
    }
  }, [valoresIniciais, form]);

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      const response = await onSubmit(values);
      message.success(`${entidade} atualizado com sucesso.`);
      if (onSuccess) onSuccess(response);
      setModalVisivel(false);
    } catch (error) {
      console.error(`Erro ao atualizar ${entidade}:`, error);
      message.error(`Falha ao atualizar ${entidade}.`);
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
          {tituloModal || "Edição de Entidade"}
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
                options={perfisClinica.map((perfil) => ({
                  ...perfil,
                  disabled:
                    (inativoSelecionado && perfil.value !== "INATIVO")
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

            <Form.Item label="Clínica">
              <div style={{ padding: "4px 11px", backgroundColor: "#f5f5f5", borderRadius: 4 }}>
                {clinicas.find(c => c.value === form.getFieldValue("clinicaId"))?.label || "Clínica não encontrada"}
              </div>
            </Form.Item>
          </>
        )}

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
            icon={<SaveOutlined />}
            loading={loading}
            className="w-1/2 bg-blue-700 hover:bg-blue-800 text-white"
            onClick={() => form.submit()}
          >
            Salvar alterações
          </Button>
        </div>
      </Form>
    </div>
  );
};

export default FormEditarEntidade;
