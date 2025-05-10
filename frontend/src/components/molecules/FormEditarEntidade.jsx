import React, { useState, useEffect } from "react";
import { Input, Select, DatePicker, Checkbox, Button, Form, message, Typography, Switch } from "antd";
import { SaveOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
import InputMask from 'react-input-mask';

const estadosBrasil = [
  { value: 'AC', label: 'Acre' },
  { value: 'AL', label: 'Alagoas' },
  { value: 'AP', label: 'Amapá' },
  { value: 'AM', label: 'Amazonas' },
  { value: 'BA', label: 'Bahia' },
  { value: 'CE', label: 'Ceará' },
  { value: 'DF', label: 'Distrito Federal' },
  { value: 'ES', label: 'Espírito Santo' },
  { value: 'GO', label: 'Goiás' },
  { value: 'MA', label: 'Maranhão' },
  { value: 'MT', label: 'Mato Grosso' },
  { value: 'MS', label: 'Mato Grosso do Sul' },
  { value: 'MG', label: 'Minas Gerais' },
  { value: 'PA', label: 'Pará' },
  { value: 'PB', label: 'Paraíba' },
  { value: 'PR', label: 'Paraná' },
  { value: 'PE', label: 'Pernambuco' },
  { value: 'PI', label: 'Piauí' },
  { value: 'RJ', label: 'Rio de Janeiro' },
  { value: 'RN', label: 'Rio Grande do Norte' },
  { value: 'RS', label: 'Rio Grande do Sul' },
  { value: 'RO', label: 'Rondônia' },
  { value: 'RR', label: 'Roraima' },
  { value: 'SC', label: 'Santa Catarina' },
  { value: 'SP', label: 'São Paulo' },
  { value: 'SE', label: 'Sergipe' },
  { value: 'TO', label: 'Tocantins' }
];

const perfisSistema = [
  { label: "Administrador", value: "ADMINISTRADOR" },
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

const FormEditarEntidade = ({
  entidade,
  onSubmit,
  onSuccess,
  setModalEdicaoVisivel,
  camposSimplesEditar,
  valoresIniciais,
  tituloModal,
}) => {
  const campos = camposSimplesEditar[entidade] || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [perfisSelecionados, setPerfisSelecionados] = useState(valoresIniciais?.perfis || []);
  const [clinicas, setClinicas] = useState([]);

  // useEffect executa sempre que ["changed"] muda, nesse caso, a entidade
  useEffect(() => {
    if (entidade === "usuariosDoSistema") { 
      // carrega a lista de clínicas para funcionar a atualização de perfis no backend
      clinicasAPI.listar()
        .then((data) => {
          console.log("Clínicas carregadas da api:", data); // debug
          // formata os dados da clínica no formato esperado pelo Select (label e value) e conseguir captar no backend
          const clinicasFormatadas = data.map((clinica) => ({
            label: clinica.nomeClinica,
            value: clinica.id,
          }));
          setClinicas(clinicasFormatadas);
          console.log("Clínicas formatadas para o Select:", clinicasFormatadas); // debug
        })
        .catch((err) => console.error("Erro ao carregar clínicas:", err));
    } 
  }, [entidade]); // dependência: executa novamente se `entidade` mudar como dito anteriormente

  // useEffect executa sempre que ["changed"] muda, nesse caso, os valores iniciais e o form
  useEffect(() => {
    if (valoresIniciais) {
      console.log("Valores iniciais recebidos:", valoresIniciais);
      form.setFieldsValue(valoresIniciais); // popula o formulário com dados já existentes
    }
  }, [valoresIniciais, form]); // dependências: executa novamente como dito anteriormente

  const sanitize = (value) => value?.replace(/\D/g, ""); // remove tudo que não for dígito

  // função de submeter o formulário
  const handleFinish = async (values) => {
    try {
      setLoading(true);
      console.log(valoresIniciais); // ativa o loading (útil para botões/spinners)
      
      // TODO: ajustar para corrigir quando usuário é administrador e quando não é, aqui está só para os comuns
      // e pode ser feito a mesma coisa no cadastro / visualização ! 
      // if (entidade === "usuariosDoSistema") values.id = valoresIniciais.upcId; 
      if (entidade === "clinicas") {
        values.cep = sanitize(values.cep);
        values.cnpj = sanitize(values.cnpj);
        values.telefone1 = sanitize(values.telefone1);
        values.telefone2 = sanitize(values.telefone2);
        values.whatsapp = sanitize(values.whatsapp);
      }

      console.log("Valores iniciais (antes da edição):", valoresIniciais); // debug
      console.log("Valores submetidos no formulário (antes de ajuste):", values); // debug
      
      const response = await onSubmit(values); // função de envio
      
      // essa mensagem aparece mesmo com fracasso enquanto a outra que está no componente pai de fato funciona
      // message.success(`${entidade} atualizado com sucesso.`); 
      
      // se tiver função de callback após o sucesso, executa
      if (onSuccess) onSuccess(response);
      setModalEdicaoVisivel(false);
    } catch (error) {
      console.error(`Erro ao atualizar ${entidade}:`, error);
      message.error(`Falha ao atualizar ${entidade}.`);
    } finally {
      // com sucesso ou falha desativa o loading
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

        <Form.Item
          name="id"
          label="id"
          hidden // esconde o campo
          initialValue={valoresIniciais.id} // valor do Id do DTO
        >
        {/* <Input /> */}
        </Form.Item>

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
                options={
                  perfisSistema
                    .filter((perfil) =>
                      entidade?.isAdmin
                        ? ["ADMIN", "INATIVO"].includes(perfil.value)
                        : ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO", "INATIVO"].includes(perfil.value)
                    )
                    .map((perfil) => ({
                      ...perfil,
                      disabled: inativoSelecionado && perfil.value !== "INATIVO",
                    }))
                }
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
            <Form.Item
              name="upcId"
              label="upcId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.upcId} // Valor inicial do DTO
            >
              {/* <Input /> */}
            </Form.Item>
            <Form.Item label="Clínica">
              <div style={{ padding: "4px 11px", backgroundColor: "#f5f5f5", borderRadius: 4 }}>
                {clinicas.find(c => c.value === form.getFieldValue("clinicaId"))?.label || "Clínica não encontrada"}
              </div>
            </Form.Item>
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
            <Form.Item
              name="clinicaId"
              label="clinicaId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.clinicaId} // Valor inicial do DTO
            >
              <Input />
            </Form.Item>
            <Form.Item
              name="upcId"
              label="upcId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.upcId} // Valor inicial do DTO
            >
              <Input />
            </Form.Item>
          </>
        )}

        {entidade === "clinicas" && (
          <>
            {/* Campo CNPJ com máscara */}
            <Form.Item
              label="CNPJ"
              name="cnpj"
              rules={[
                // { required: true, message: "CNPJ é obrigatório" },
                // {
                //   validator: (_, value) =>
                //     value && value.replace(/\D/g, '').length === 14
                //       ? Promise.resolve()
                //       : Promise.reject(new Error('CNPJ inválido'))
                // }
              ]}
            >
              <InputMask
                mask="99.999.999/9999-99"
                maskChar={null}
                disabled={form.getFieldValue('IsAtiva') === false}
              >
                {(inputProps) => (
                  <Input 
                    {...inputProps} 
                    placeholder="00.000.000/0000-00" 
                  />
                )}
              </InputMask>
            </Form.Item>

            {/* Campo Telefone com máscara dinâmica */}
            <Form.Item
              label="Telefone"
              name="telefone1"
            >
              <InputMask
                mask={form.getFieldValue('telefone1')?.replace(/\D/g, '').length > 10 
                  ? "(99) 99999-9999" 
                  : "(99) 9999-9999"}
                maskChar={null}
                disabled={form.getFieldValue('isAtiva') === false}
              >
                {(inputProps) => (
                  <Input 
                    {...inputProps} 
                    placeholder="(00) 0000-0000" 
                  />
                )}
              </InputMask>
            </Form.Item>

            {/* Campo WhatsApp com máscara */}
            <Form.Item
              label="WhatsApp"
              name="whatsapp"
            >
              <InputMask
                mask="(99) 99999-9999"
                maskChar={null}
                disabled={form.getFieldValue('isAtiva') === false}
              >
                {(inputProps) => (
                  <Input 
                    {...inputProps} 
                    placeholder="(00) 00000-0000" 
                  />
                )}
              </InputMask>
            </Form.Item>

            {/* Campo CEP com máscara */}
            <Form.Item
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
              <InputMask
                mask="99999-999"
                maskChar={null}
                disabled={form.getFieldValue('isAtiva') === false}
              >
                {(inputProps) => (
                  <Input 
                    {...inputProps} 
                    placeholder="00000-000" 
                  />
                )}
              </InputMask>
            </Form.Item>

            {/* Campo Estado */}
            <Form.Item
              label="Estado"
              name="endEstado"
              // rules={[{ required: true, message: "Estado é obrigatório" }]}
            >
              <Select
                options={estadosBrasil}
                placeholder="Selecione o estado"
                disabled={form.getFieldValue('isAtiva') === false}
              />
            </Form.Item>

            {/* Switch Ativo/Inativo */}
            <Form.Item
              label="Status da Clínica"
              name="isAtiva"
              valuePropName="checked"
            >
              <Switch
                checkedChildren="ATIVA"
                unCheckedChildren="INATIVA"
                defaultChecked
                onChange={(checked) => {
                  // Desabilita campos quando clínica está inativa
                  form.setFieldsValue({
                    cnpj: checked ? form.getFieldValue('cnpj') : '',
                    telefone1: checked ? form.getFieldValue('telefone1') : '',
                    whatsapp: checked ? form.getFieldValue('whatsapp') : '',
                    cep: checked ? form.getFieldValue('cep') : ''
                  });
                }}
              />
            </Form.Item>
          </>
        )}

        <div className="flex gap-4 justify-between">
          <Button
            onClick={() => setModalEdicaoVisivel(false)}
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
