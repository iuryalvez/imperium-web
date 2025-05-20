import React, { useState, useEffect } from "react";
import { Input, Select, DatePicker, Checkbox, Button, Form, message, Typography, Switch } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
import { IMaskInput } from 'react-imask';

// const estadosBrasil = [
//   { value: 'AC', label: 'Acre' },
//   { value: 'AL', label: 'Alagoas' },
//   { value: 'AP', label: 'Amapá' },
//   { value: 'AM', label: 'Amazonas' },
//   { value: 'BA', label: 'Bahia' },
//   { value: 'CE', label: 'Ceará' },
//   { value: 'DF', label: 'Distrito Federal' },
//   { value: 'ES', label: 'Espírito Santo' },
//   { value: 'GO', label: 'Goiás' },
//   { value: 'MA', label: 'Maranhão' },
//   { value: 'MT', label: 'Mato Grosso' },
//   { value: 'MS', label: 'Mato Grosso do Sul' },
//   { value: 'MG', label: 'Minas Gerais' },
//   { value: 'PA', label: 'Pará' },
//   { value: 'PB', label: 'Paraíba' },
//   { value: 'PR', label: 'Paraná' },
//   { value: 'PE', label: 'Pernambuco' },
//   { value: 'PI', label: 'Piauí' },
//   { value: 'RJ', label: 'Rio de Janeiro' },
//   { value: 'RN', label: 'Rio Grande do Norte' },
//   { value: 'RS', label: 'Rio Grande do Sul' },
//   { value: 'RO', label: 'Rondônia' },
//   { value: 'RR', label: 'Roraima' },
//   { value: 'SC', label: 'Santa Catarina' },
//   { value: 'SP', label: 'São Paulo' },
//   { value: 'SE', label: 'Sergipe' },
//   { value: 'TO', label: 'Tocantins' }
// ];

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

const FormCadastrarEntidade = ({ entidade, nomeEntidade, onSubmit, onSuccess, setModalVisivel, tituloModal, camposSimplesCadastrar }) => {
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

  const sanitize = (value) => value?.replace(/\D/g, ""); 

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      
      if (entidade === "usuariosDoSistema" || "usuariosDaClinica") {
        // Verifica se o array de perfis contém "INATIVO"
        const isInativo = values.perfis?.includes("INATIVO") || false;
        
        // Atualiza o valor de IsInativo
        values.isAtivo = !isInativo;
      }

      if (values.cpf) values.cpf = sanitize(values.cpf);
      if (values.cep) values.cep = sanitize(values.cep);
      if (values.cnpj) values.cnpj = sanitize(values.cnpj);
      if (values.telefone1) values.telefone1 = sanitize(values.telefone1);
      if (values.telefone2) values.telefone2 = sanitize(values.telefone2);
      if (values.whatsapp) values.whatsapp = sanitize(values.whatsapp);

      const response = await onSubmit(values);
      message.success(`${nomeEntidade} criado com sucesso.`);
      form.resetFields();
      setPerfisSelecionados([]);
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar ${nomeEntidade}:`, error);
      message.error(`Falha ao cadastrar ${nomeEntidade}.`);
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

  const adminSelecionado = perfisSelecionados.includes("ADMIN");
  const outrosSelecionados = perfisSelecionados.some(val => 
    ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val)
  );

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
                    (adminSelecionado && !["ADMIN", "INATIVO"].includes(perfil.value)) ||
                    (outrosSelecionados && perfil.value === "ADMIN")
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;

                  // Se selecionou ADMIN, mantém apenas ADMIN e INATIVO
                  if (values.includes("ADMIN")) {
                    newValues = values.filter(val => val === "ADMIN" || val === "INATIVO");
                  } 
                  // Se selecionou outros perfis, remove ADMIN se estiver presente
                  else if (values.some(val => ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val))) {
                    newValues = values.filter(val => val !== "ADMIN");
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

        {entidade === "tutoresDaClinica" && (
          <>
            {/* Campo CNPJ com máscara */}
            <Form.Item label="CPF" name="cpf">
              <IMaskInput
                mask="000.000.000-00"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="000.000.000-00"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item>

            {/* Campo Telefone com máscara dinâmica */}
            <Form.Item
              label="Telefone/Whatsapp"
              name="telefone1"
            >
              <IMaskInput
                mask="(00) 90000-0000"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="(00) 90000-0000"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item>

            {/* Campo WhatsApp com máscara */}
            {/* <Form.Item
              label="WhatsApp"
              name="whatsapp"
            >
              <IMaskInput
                mask="(00) 90000-0000"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="(00) 90000-0000"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item> */}

            {/* Campo CEP com máscara */}
            {/* <Form.Item
              label="CEP"
              name="cep"
              rules={[
                // { required: true, message: "CEP é obrigatório" },
                // {
                //   validator: (_, value) =>
                //     value && value.replace(/\D/g, '').length === 8
                //       ? Promise.resolve()
                //       : Promise.reject(new Error('CEP inválido'))
                // }
              ]}
            >
              <IMaskInput
                mask="00000-000"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="00000-000"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item> */}

            {/* Campo Estado */}
            {/* <Form.Item
              label="Estado"
              name="endEstado"
              // rules={[{ required: true, message: "Estado é obrigatório" }]}
            >
              <Select
                options={estadosBrasil}
                placeholder="Selecione o estado"
                disabled={form.getFieldValue('isAtiva') === false}
              />
            </Form.Item> */}
          </>
        )}

        {/* <Button onClick={() => {
          console.log(form.getFieldsValue());
        }}>
          Ver campos do formulário
        </Button> */}
        <div className="flex gap-4">
          <Button
            onClick={() => setModalVisivel(false)}
            className="w-1/2 mt-2"
            style={{ backgroundColor: "#d9d9d9", color: "#000" }}
          >
            Cancelar
          </Button>

          <Button
            type="primary"
            icon={<PlusOutlined />}
            loading={loading}
            className="w-1/2 mt-2 bg-green-700 hover:bg-green-800 text-white"
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