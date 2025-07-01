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
import { SaveOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
import { IMaskInput } from "react-imask";
import dayjs from "dayjs";
import { racasAPI } from "../../api/sistema/racasAPI";

const estadosBrasil = [
  { value: "AC", label: "Acre" },
  { value: "AL", label: "Alagoas" },
  { value: "AP", label: "Amapá" },
  { value: "AM", label: "Amazonas" },
  { value: "BA", label: "Bahia" },
  { value: "CE", label: "Ceará" },
  { value: "DF", label: "Distrito Federal" },
  { value: "ES", label: "Espírito Santo" },
  { value: "GO", label: "Goiás" },
  { value: "MA", label: "Maranhão" },
  { value: "MT", label: "Mato Grosso" },
  { value: "MS", label: "Mato Grosso do Sul" },
  { value: "MG", label: "Minas Gerais" },
  { value: "PA", label: "Pará" },
  { value: "PB", label: "Paraíba" },
  { value: "PR", label: "Paraná" },
  { value: "PE", label: "Pernambuco" },
  { value: "PI", label: "Piauí" },
  { value: "RJ", label: "Rio de Janeiro" },
  { value: "RN", label: "Rio Grande do Norte" },
  { value: "RS", label: "Rio Grande do Sul" },
  { value: "RO", label: "Rondônia" },
  { value: "RR", label: "Roraima" },
  { value: "SC", label: "Santa Catarina" },
  { value: "SP", label: "São Paulo" },
  { value: "SE", label: "Sergipe" },
  { value: "TO", label: "Tocantins" },
];

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

const FormEditarEntidade = ({
  entidade,
  onSubmit,
  onSuccess,
  setModalEdicaoVisivel,
  camposSimplesEditar,
  valoresIniciais,
  tituloModal,
}) => {
  const campos = camposSimplesEditar || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // usuarios e usuariosDaClinica
  const [perfisSelecionados, setPerfisSelecionados] = useState(
    valoresIniciais?.perfis || []
  );

  // Estados para controlar a lógica do formulário de animais
  const [especieSelecionada, setEspecieSelecionada] = useState(
    valoresIniciais?.especie
  );
  const [sexoAnimal, setSexoAnimal] = useState(valoresIniciais?.sexo);
  const [emGestacao, setEmGestacao] = useState(
    valoresIniciais?.gestacao || false
  );
  const [numFilhotes, setNumFilhotes] = useState(
    valoresIniciais?.lactacao || 0
  );

  const isFemea = sexoAnimal === 2; // Supondo que 2 é 'Fêmea'
  const isGestando = emGestacao;
  const isLactando = numFilhotes > 0;

  // clinicas
  const [clinicas, setClinicas] = useState([]);

  const [racasDeCaes, setRacasDeCaes] = useState([]);
  const [racasDeGatos, setRacasDeGatos] = useState([]);

  // useEffect executa sempre que ["changed"] muda, nesse caso, a entidade
  useEffect(() => {
    if (entidade === "usuariosDoSistema") {
      // carrega a lista de clínicas para funcionar a atualização de perfis no backend
      clinicasAPI
        .listar()
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
  }, [entidade]); // dependência: executa novamente se `entidade` mudar como dito anteriormente

  // useEffect executa sempre que ["changed"] muda, nesse caso, os valores iniciais e o form
  // useEffect(() => {
  //   if (valoresIniciais) {
  //     console.log("Valores iniciais recebidos:", valoresIniciais);

  //     // ✅ CONVERSÃO DA DATA ACONTECE AQUI
  //     const valoresFormatados = {
  //       ...valoresIniciais,
  //       // Se 'nascimento' existir, converte a string para um objeto dayjs
  //       nascimento: valoresIniciais.nascimento
  //         ? dayjs(valoresIniciais.nascimento)
  //         : null,
  //       // Faça o mesmo para qualquer outro campo de data que você tenha
  //     };

  //     form.setFieldsValue(valoresFormatados); // Popula o formulário com os valores já corrigidos
  //   }
  // }, [valoresIniciais, form]);

  // Carrega e formata os dados iniciais quando o componente recebe os valores
  useEffect(() => {
    if (valoresIniciais) {
      // Converte a string de data para um objeto dayjs que o DatePicker entende
      const valoresFormatados = {
        ...valoresIniciais,
        nascimento: valoresIniciais.nascimento
          ? dayjs(valoresIniciais.nascimento)
          : null,
      };
      form.setFieldsValue(valoresFormatados);

      // <-- MUDANÇA AQUI: Sincroniza os estados locais com os dados carregados
      if (entidade === "animaisDaClinica") {
        setEspecieSelecionada(valoresIniciais.especie);
        setSexoAnimal(valoresIniciais.sexo);
        setEmGestacao(valoresIniciais.gestacao || false);
        setNumFilhotes(valoresIniciais.lactacao || 0);
      }
    }
  }, [valoresIniciais, form, entidade]);

  const sanitize = (value) => value?.replace(/\D/g, ""); // remove tudo que não for dígito

  // função de submeter o formulário
  const handleFinish = async (values) => {
    try {
      setLoading(true);
      let processedValues = { ...values };
      console.log(valoresIniciais); // ativa o loading (útil para botões/spinners)

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

      console.log("Valores iniciais (antes da edição):", valoresIniciais); // debug
      console.log(
        "Valores submetidos no formulário (antes de ajuste):",
        values
      ); // debug

      const response = await onSubmit(values); // função de envio

      // essa mensagem aparece mesmo com fracasso enquanto a outra que está no componente pai de fato funciona
      // message.success(`${entidade} atualizado com sucesso.`);

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

      // se tiver função de callback após o sucesso, executa
      if (onSuccess) onSuccess(response);
      setModalEdicaoVisivel(false);
      form.resetFields();
    } catch (error) {
      console.error(`Erro ao atualizar ${entidade}:`, error);
      message.error(`Falha ao atualizar ${entidade}.`);
    } finally {
      // com sucesso ou falha desativa o loading
      setLoading(false);
    }
  };

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

  return (
    <div className="p-2">
      <Form
        title={`${tituloModal}`}
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        onChange={handleValuesChange}
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
                {
                  required: true,
                  message: "Pelo menos um perfil deve ser selecionado",
                },
              ]}
            >
              <Checkbox.Group
                options={perfisSistema
                  .filter((perfil) =>
                    valoresIniciais.isAdmin
                      ? ["ADMIN", "INATIVO"].includes(perfil.value)
                      : [
                          "GESTOR",
                          "TECNICO",
                          "SECRETARIO",
                          "VETERINARIO",
                          "INATIVO",
                        ].includes(perfil.value)
                  )
                  .map((perfil) => ({
                    ...perfil,
                    ...(valoresIniciais.isAdmin && perfil.value === "ADMIN"
                      ? { disabled: true }
                      : {}),
                  }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={
                  valoresIniciais.isAdmin
                    ? Array.from(new Set([...perfisSelecionados, "ADMIN"])) // Garante que ADMINISTRADOR está sempre selecionado
                    : perfisSelecionados
                }
                onChange={(values) => {
                  let newValues = valoresIniciais.isAdmin
                    ? Array.from(new Set([...values, "ADMIN"])) // Mantém ADMINISTRADOR selecionado
                    : values;

                  setPerfisSelecionados(newValues);
                  form.setFieldValue("perfis", newValues);
                }}
              />
            </Form.Item>
            <Form.Item
              name="isAdmin"
              label="isAdmin"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.isAdmin} // Valor inicial do DTO
            ></Form.Item>
            <Form.Item
              name="upcId"
              label="upcId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.upcId} // Valor inicial do DTO
            >
              {/* <Input /> */}
            </Form.Item>
            <Form.Item label="Clínica">
              <div
                style={{
                  padding: "4px 11px",
                  backgroundColor: "#f5f5f5",
                  borderRadius: 4,
                }}
              >
                {clinicas.find(
                  (c) => c.value === form.getFieldValue("clinicaId")
                )?.label || "Clínica não encontrada"}
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
            <Form.Item
              name="clinicaId"
              label="clinicaId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.clinicaId} // Valor inicial do DTO
            ></Form.Item>
            <Form.Item
              name="upcId"
              label="upcId"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.upcId} // Valor inicial do DTO
            ></Form.Item>
          </>
        )}

        {entidade === "clinicas" && (
          <>
            {/* Campo CNPJ com máscara */}
            <Form.Item
              label="CNPJ"
              name="cnpj"
              rules={
                [
                  // { required: true, message: "CNPJ é obrigatório" },
                  // {
                  //   validator: (_, value) =>
                  //     value && value.replace(/\D/g, '').length === 14
                  //       ? Promise.resolve()
                  //       : Promise.reject(new Error('CNPJ inválido'))
                  // }
                ]
              }
            >
              <IMaskInput
                mask="00.000.000/0000-00"
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="00.000.000/0000-00"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
                }}
                disabled={form.getFieldValue("IsAtiva") === false}
              />
            </Form.Item>

            {/* Campo Telefone com máscara dinâmica */}
            <Form.Item label="Telefone" name="telefone1">
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
                disabled={form.getFieldValue("isAtiva") === false}
              />
            </Form.Item>

            {/* Campo WhatsApp com máscara */}
            <Form.Item label="WhatsApp" name="whatsapp">
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
                disabled={form.getFieldValue("isAtiva") === false}
              />
            </Form.Item>

            {/* Campo CEP com máscara */}
            <Form.Item
              label="CEP"
              name="cep"
              rules={
                [
                  // { required: true, message: "CEP é obrigatório" },
                  // {
                  //   validator: (_, value) =>
                  //     value && value.replace(/\D/g, '').length === 8
                  //       ? Promise.resolve()
                  //       : Promise.reject(new Error('CEP inválido'))
                  // }
                ]
              }
            >
              <IMaskInput
                mask="00000-000"
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="00000-000"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
                }}
                disabled={form.getFieldValue("isAtiva") === false}
              />
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
                disabled={form.getFieldValue("isAtiva") === false}
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
                    cnpj: checked ? form.getFieldValue("cnpj") : "",
                    telefone1: checked ? form.getFieldValue("telefone1") : "",
                    whatsapp: checked ? form.getFieldValue("whatsapp") : "",
                    cep: checked ? form.getFieldValue("cep") : "",
                  });
                }}
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

        {entidade === "animaisDaClinica" && (
          <>
            <Form.Item
              name="id"
              label="id"
              hidden // Esta propriedade esconde o campo
              initialValue={valoresIniciais.id} // Valor inicial do DTO
            ></Form.Item>
            <Form.Item
              name="castrado"
              label="Castrado?"
              valuePropName="checked" // Para Switch
              initialValue={valoresIniciais.castrado}
            >
              <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
            </Form.Item>
            {isFemea && (
              <>
                <Form.Item
                  name="gestacao"
                  label="Em Gestação?"
                  valuePropName="checked" // Para Switch
                  initialValue={valoresIniciais.gestacao}
                >
                  <Switch />
                </Form.Item>

                <Form.Item
                  name="lactacao" // Este é o campo para "Número de Filhotes"
                  label="Número de Filhotes (Lactação)"
                  initialValue={valoresIniciais.lactacao}
                >
                  <InputNumber min={0} style={{ width: "100%" }} />
                </Form.Item>
              </>
            )}

            {isFemea && isGestando && (
              <Form.Item
                name="semanaGestacao"
                label="Semanas de Gestação"
                initialValue={valoresIniciais.semanaGestacao}
              >
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
                <Form.Item
                  name="semanaLactacao"
                  label="Semanas de Lactação"
                  initialValue={valoresIniciais.semanaLactacao}
                >
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
