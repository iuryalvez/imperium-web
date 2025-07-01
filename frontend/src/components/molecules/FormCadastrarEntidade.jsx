import React, { useState, useEffect } from "react";
import {
  Input,
  Select,
  DatePicker,
  Checkbox,
  Button,
  Form,
  message,
  Typography,
  Switch,
  InputNumber,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
import { IMaskInput } from "react-imask";
import { racasAPI } from "../../api/sistema/racasAPI";

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

const FormCadastrarEntidade = ({
  entidade,
  nomeEntidade,
  onSubmit,
  onSuccess,
  setModalVisivel,
  tituloModal,
  camposSimplesCadastrar,
}) => {
  const campos = camposSimplesCadastrar || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // usuarios e usuariosDaClinica
  const [perfisSelecionados, setPerfisSelecionados] = useState([]);

  // clinicas
  const [clinicas, setClinicas] = useState([]);

  // animaisDaClinica
  const [especieSelecionada, setEspecieSelecionada] = useState(
    form.getFieldValue("especie")
  );
  const [sexoAnimal, setSexoAnimal] = useState(form.getFieldValue("sexo"));
  const [emGestacao, setEmGestacao] = useState(
    form.getFieldValue("gestacao") || false
  );
  const [numFilhotes, setNumFilhotes] = useState(
    form.getFieldValue("lactacao") || 0
  ); // 'lactacao' é o campo para número de filhotes

  const isFemea = sexoAnimal === 2;
  const isGestando = emGestacao;
  const isLactando = numFilhotes > 0;

  const [racasDeCaes, setRacasDeCaes] = useState([]);
  const [racasDeGatos, setRacasDeGatos] = useState([]);

  useEffect(() => {
    if (entidade === "usuariosDoSistema") {
      clinicasAPI
        .listar()
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
    if (entidade === "animaisDaClinica") {
      const carregarRacas = async () => {
        try {
          setLoading(true);

          const [caesData, gatosData] = await Promise.all([
            racasAPI.listar(1), // 1 = EspecieAnimal.Cao
            racasAPI.listar(2), // 2 = EspecieAnimal.Gato
          ]);

          const caesFormatados = caesData.map((raca) => ({
            label: raca.nome,
            value: raca.id,
          }));
          setRacasDeCaes(caesFormatados);

          const gatosFormatados = gatosData.map((raca) => ({
            label: raca.nome,
            value: raca.id,
          }));
          setRacasDeGatos(gatosFormatados);
        } catch (err) {
          setError("Não foi possível carregar as listas de raças.");
          console.error("Erro ao carregar raças:", err);
        } finally {
          setLoading(false);
        }
      };

      carregarRacas();
    }
  }, [entidade]);

  const sanitize = (value) => value?.replace(/\D/g, "");

  const handleValuesChange = (changedValues, allValues) => {
    if (entidade === "animaisDaClinica") {
      let formUpdates = {};
      let stateNeedsUpdate = false;

      if ("sexo" in changedValues) {
        setSexoAnimal(changedValues.sexo);
        stateNeedsUpdate = true;
        if (changedValues.sexo !== 2) {
          // Não é Fêmea (Macho ou indefinido)
          setEmGestacao(false);
          setNumFilhotes(0);
          formUpdates = {
            ...formUpdates,
            gestacao: false,
            semanaGestacao: undefined,
            lactacao: 0,
            semanaLactacao: undefined,
          };
        }
      }
      if ("gestacao" in changedValues) {
        setEmGestacao(changedValues.gestacao);
        stateNeedsUpdate = true;
        if (!changedValues.gestacao) {
          formUpdates.semanaGestacao = undefined;
        }
      }
      if ("lactacao" in changedValues) {
        // 'lactacao' é o campo para número de filhotes
        setNumFilhotes(changedValues.lactacao || 0);
        stateNeedsUpdate = true;
        if (!changedValues.lactacao || changedValues.lactacao === 0) {
          formUpdates.semanaLactacao = undefined;
        }
      }
      if ("especie" in changedValues) {
        setEspecieSelecionada(changedValues.especie);
        stateNeedsUpdate = true;
        formUpdates.raca = undefined;
      }

      if (Object.keys(formUpdates).length > 0) {
        form.setFieldsValue(formUpdates);
      }
    }

    if (entidade === "usuariosDoSistema" && "perfis" in changedValues) {
      const currentPerfis = changedValues.perfis || [];
      let newValues = [...currentPerfis];

      if (currentPerfis.includes("ADMIN")) {
        newValues = currentPerfis.filter(
          (val) => val === "ADMIN" || val === "INATIVO"
        );
      } else if (
        currentPerfis.some((val) =>
          ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val)
        )
      ) {
        newValues = currentPerfis.filter((val) => val !== "ADMIN");
      }
      setPerfisSelecionados(newValues);
      // Se os valores foram alterados pela lógica acima, atualize o formulário
      if (
        newValues.length !== currentPerfis.length ||
        !newValues.every((val, index) => val === currentPerfis[index])
      ) {
        form.setFieldsValue({ perfis: newValues });
      }
    }
  };

  const handleFinish = async (values) => {
    try {
      setLoading(true);
      let processedValues = { ...values };

      if (
        entidade === "usuariosDoSistema" ||
        entidade === "usuariosDaClinica"
      ) {
        const isInativo = processedValues.perfis?.includes("INATIVO") || false;
        processedValues.isAtivo = !isInativo;
      }

      if (entidade === "clinicas") {
        if (processedValues.cpf)
          processedValues.cpf = sanitize(processedValues.cpf);
        if (processedValues.cep)
          processedValues.cep = sanitize(processedValues.cep);
        if (processedValues.cnpj)
          processedValues.cnpj = sanitize(processedValues.cnpj);
        if (processedValues.telefone1)
          processedValues.telefone1 = sanitize(processedValues.telefone1);
        if (processedValues.telefone2)
          processedValues.telefone2 = sanitize(processedValues.telefone2);
        if (processedValues.whatsapp)
          processedValues.whatsapp = sanitize(processedValues.whatsapp);
      }

      // Para animais, garantir que valores condicionais sejam enviados corretamente
      if (entidade === "animaisDaClinica") {
        if (processedValues.sexo === 1) {
          // Macho
          processedValues.gestacao = false;
          processedValues.semanaGestacao = 0;
          processedValues.lactacao = 0;
          processedValues.semanaLactacao = 0;
        } else {
          // Fêmea
          if (!processedValues.gestacao) {
            processedValues.semanaGestacao = 0;
          }
          if (!processedValues.lactacao || processedValues.lactacao === 0) {
            processedValues.lactacao = 0; // Garante 0 se for null/undefined
            processedValues.semanaLactacao = 0;
          }
        }
      }

      const response = await onSubmit(processedValues);
      message.success(`${nomeEntidade} criado com sucesso.`);
      form.resetFields();
      // Resetar estados de controle após o sucesso
      if (entidade === "animaisDaClinica") {
        setSexoAnimal(form.getFieldValue("sexo")); // Ou valor padrão
        setEmGestacao(false);
        setNumFilhotes(0);
      }
      if (
        entidade === "usuariosDoSistema" ||
        entidade === "usuariosDaClinica"
      ) {
        setPerfisSelecionados([]);
      }
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar ${nomeEntidade}:`, error);
      const errorMessage =
        error.response?.data?.message || `Falha ao cadastrar ${nomeEntidade}.`;
      message.error(errorMessage);
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
        if (entidade === "animaisDaClinica" && campo.name === "raca") {
          return (
            <Select
              placeholder={campo.placeholder || "Selecione a raça"}
              allowClear
              // This logic correctly disables the dropdown if no species is selected.
              // Comentário: Esta lógica desabilita corretamente o dropdown se nenhuma espécie for selecionada.
              disabled={!especieSelecionada}
            >
              {/* Renders dog breeds if 'Cão' (value 1) is selected */}
              {/* Comentário: Renderiza raças de cães se 'Cão' (valor 1) for selecionado */}
              {especieSelecionada === 1 &&
                racasDeCaes.map((raca) => (
                  // The key and value should be the breed's ID
                  // Comentário: A chave e o valor devem ser o ID da raça
                  <Select.Option key={raca.value} value={raca.value}>
                    {/* The text shown to the user is the breed's name */}
                    {/* Comentário: O texto mostrado para o usuário é o nome da raça */}
                    {raca.label}
                  </Select.Option>
                ))}

              {/* Renders cat breeds if 'Gato' (value 2) is selected */}
              {/* Comentário: Renderiza raças de gatos se 'Gato' (valor 2) for selecionado */}
              {especieSelecionada === 2 &&
                racasDeGatos.map((raca) => (
                  <Select.Option key={raca.value} value={raca.value}>
                    {raca.label}
                  </Select.Option>
                ))}
            </Select>
          );
        } else {
          return (
            <Select placeholder={campo.placeholder}>
              {campo.options?.map((opt) => (
                <Select.Option key={opt.value} value={opt.value}>
                  {opt.label}
                </Select.Option>
              ))}
            </Select>
          );
        }
      case "date":
        return (
          <DatePicker
            style={{ width: "100%" }}
            placeholder="Selecione a data"
            format="DD/MM/YYYY"
          />
        );
      case "boolean":
        return <Checkbox>{campo.placeholder}</Checkbox>;
      case "number":
        return (
          <InputNumber
            prefix=""
            style={{ width: "100%" }}
            precision={2} // Duas casas decimais
            step={0.01}
            decimalSeparator="," // Mais simples para este caso
            parser={(value) => value.replace(",", ".")}
            min={0}
            max={1000}
          />
        );
      default:
        return null;
    }
  };

  const adminSelecionado = perfisSelecionados.includes("ADMIN");
  const outrosSelecionados = perfisSelecionados.some((val) =>
    ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val)
  );

  // Inicializa os estados com os valores do formulário, se houver, após a primeira renderização
  useEffect(() => {
    if (entidade === "animaisDaClinica") {
      setEspecieSelecionada(form.getFieldValue("especie"));
      setSexoAnimal(form.getFieldValue("sexo"));
      setEmGestacao(form.getFieldValue("gestacao") || false);
      setNumFilhotes(form.getFieldValue("lactacao") || 0);
    }
    if (entidade === "usuariosDoSistema") {
      setPerfisSelecionados(form.getFieldValue("perfis") || []);
    }
  }, [form, entidade]);

  return (
    <div className="p-2">
      <Form
        title={`${tituloModal}`}
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        className="space-y-3"
        onValuesChange={handleValuesChange}
        initialValues={
          entidade === "animaisDaClinica"
            ? {
                gestacao: false,
                lactacao: 0, // Número de filhotes
                // Defina um valor inicial para sexo se desejar, ex: '1' para Macho
                // sexo: '1',
              }
            : {}
        }
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
                {
                  required: true,
                  message: "Pelo menos um perfil deve ser selecionado",
                },
              ]}
            >
              <Checkbox.Group
                options={perfisSistema.map((perfil) => ({
                  ...perfil,
                  disabled:
                    (adminSelecionado &&
                      !["ADMIN", "INATIVO"].includes(perfil.value)) ||
                    (outrosSelecionados && perfil.value === "ADMIN"),
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;

                  // Se selecionou ADMIN, mantém apenas ADMIN e INATIVO
                  if (values.includes("ADMIN")) {
                    newValues = values.filter(
                      (val) => val === "ADMIN" || val === "INATIVO"
                    );
                  }
                  // Se selecionou outros perfis, remove ADMIN se estiver presente
                  else if (
                    values.some((val) =>
                      [
                        "GESTOR",
                        "TECNICO",
                        "SECRETARIO",
                        "VETERINARIO",
                      ].includes(val)
                    )
                  ) {
                    newValues = values.filter((val) => val !== "ADMIN");
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
                    {
                      required: true,
                      message: "Selecione pelo menos uma clínica",
                    },
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
                {
                  required: true,
                  message: "Pelo menos um perfil deve ser selecionado",
                },
              ]}
            >
              <Checkbox.Group
                options={perfisClinica.map((perfil) => ({
                  ...perfil,
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
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="000.000.000-00"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
                }}
              />
            </Form.Item>

            {/* Campo Telefone com máscara dinâmica */}
            <Form.Item label="Telefone/Whatsapp" name="telefone1">
              <IMaskInput
                mask="(00) 90000-0000"
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="(00) 90000-0000"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
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

        {/* Campos condicionais específicos para animaisDaClinica */}
        {entidade === "animaisDaClinica" && (
          <>
            <Form.Item
              name="castrado"
              label="Castrado?"
              valuePropName="checked" // Para Switch
            >
              <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
            </Form.Item>
            {isFemea && (
              <>
                <Form.Item
                  name="gestacao"
                  label="Em Gestação?"
                  valuePropName="checked" // Para Switch
                >
                  <Switch />
                </Form.Item>

                <Form.Item
                  name="lactacao" // Este é o campo para "Número de Filhotes"
                  label="Número de Filhotes (Lactação)"
                >
                  <InputNumber min={0} style={{ width: "100%" }} />
                </Form.Item>
              </>
            )}

            {isFemea && isGestando && (
              <Form.Item name="semanaGestacao" label="Semanas de Gestação">
                <InputNumber
                  min={1}
                  max={45}
                  style={{ width: "100%" }}
                  placeholder="Nº de semanas"
                />
              </Form.Item>
            )}

            {isFemea &&
              isLactando && ( // isLactando é true se numFilhotes > 0
                <Form.Item name="semanaLactacao" label="Semanas de Lactação">
                  <InputNumber
                    min={1}
                    max={20}
                    style={{ width: "100%" }}
                    placeholder="Nº de semanas"
                  />
                </Form.Item>
              )}
          </>
        )}

        {/* <Button onClick={() => {
          console.log(form.getFieldsValue());
        }}>
          Ver campos do formulário
        </Button> */}
        <div className="flex gap-4 mt-6">
          <Button
            onClick={() => {
              form.resetFields();
              if (entidade === "animaisDaClinica") {
                setSexoAnimal(form.getFieldValue("sexo"));
                setEmGestacao(false);
                setNumFilhotes(0);
                setEspecieSelecionada(form.getFieldValue("especie"));
              }
              if (entidade === "usuariosDoSistema") {
                setPerfisSelecionados([]);
              }
              setModalVisivel(false);
            }}
            className="w-1/2"
          >
            Cancelar
          </Button>
          <Button
            type="primary"
            htmlType="submit" // Mudado para htmlType="submit" para acionar onFinish do Form
            icon={<PlusOutlined />}
            loading={loading}
            className="w-1/2 bg-[var(--verde-medio)] hover:bg-[var(--verde-relce)] text-white"
            // Removido onClick={() => form.submit()} pois htmlType="submit" já faz isso
          >
            Cadastrar
          </Button>
        </div>
      </Form>
    </div>
  );
};

export default FormCadastrarEntidade;
