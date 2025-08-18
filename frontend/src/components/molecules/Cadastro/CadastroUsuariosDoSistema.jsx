import React from "react";
import { Form, Input, Select, Checkbox } from "antd";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI"; // Ajuste o caminho

const perfisSistema = [
  { label: "Administrador", value: "ADMIN" },
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const FormularioCadastroUsuarioDoSistema = ({ form }) => {
  const [clinicas, setClinicas] = React.useState([]);
  const [loadingClinicas, setLoadingClinicas] = React.useState(false);

  // 1. Observa o campo 'perfis' para reagir a mudanças
  const perfisSelecionados = Form.useWatch("perfis", form) || [];

  // 2. Deriva os estados da seleção atual para usar na lógica
  const adminSelecionado = perfisSelecionados.includes("ADMIN");
  const outrosPerfisSelecionados = perfisSelecionados.some((val) =>
    ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val)
  );

  React.useEffect(() => {
    if (!adminSelecionado) {
      setLoadingClinicas(true);
      clinicasAPI
        .listar()
        .then((data) =>
          setClinicas(data.map((c) => ({ label: c.nomeClinica, value: c.id })))
        )
        .catch((err) => console.error("Erro ao carregar clínicas:", err))
        .finally(() => setLoadingClinicas(false));
    }
  }, [adminSelecionado]);

  React.useEffect(() => {
    if (adminSelecionado && form.getFieldValue("clinicaIds")?.length) {
      form.setFieldsValue({ clinicaIds: [] });
    }
  }, [adminSelecionado, form]);

  // 3. Lógica para corrigir a seleção de perfis automaticamente
  const handlePerfisChange = (checkedValues) => {
    let newValues = [...checkedValues];

    // Se ADMIN foi selecionado, desmarca todos os outros perfis, exceto INATIVO
    if (checkedValues.includes("ADMIN")) {
      newValues = checkedValues.filter((v) => v === "ADMIN" || v === "INATIVO");
    }
    // Se outro perfil foi selecionado, garante que ADMIN seja desmarcado
    else if (outrosPerfisSelecionados) {
      newValues = checkedValues.filter((v) => v !== "ADMIN");
    }

    // Atualiza o formulário somente se os valores foram de fato alterados pela lógica
    if (JSON.stringify(newValues) !== JSON.stringify(checkedValues)) {
      form.setFieldsValue({ perfis: newValues });
    }
  };

  return (
    <div>
      <Form.Item
        label="Nome Completo"
        name="nome"
        rules={[{ required: true, message: "O nome é obrigatório" }]}
      >
        <Input placeholder="Digite o nome completo" />
      </Form.Item>
      <Form.Item
        label="E-mail"
        name="email"
        rules={[
          { required: true, message: "O e-mail é obrigatório" },
          { type: "email", message: "Formato de e-mail inválido" },
        ]}
      >
        <Input placeholder="exemplo@email.com" />
      </Form.Item>
      <Form.Item
        label="Senha"
        name="senha"
        rules={[{ required: true, message: "A senha é obrigatória" }]}
      >
        <Input.Password placeholder="Crie uma senha forte" />
      </Form.Item>
      <Form.Item
        label="Confirmar Senha"
        name="confirmarSenha"
        dependencies={["senha"]}
        rules={[
          { required: true, message: "Confirme sua senha" },
          ({ getFieldValue }) => ({
            validator(_, value) {
              if (!value || getFieldValue("senha") === value)
                return Promise.resolve();
              return Promise.reject(new Error("As senhas não coincidem!"));
            },
          }),
        ]}
      >
        <Input.Password placeholder="Digite a senha novamente" />
      </Form.Item>
      <Form.Item
        label="Perfis"
        name="perfis"
        rules={[{ required: true, message: "Selecione ao menos um perfil" }]}
      >
        <Checkbox.Group
          style={{ display: "flex", flexDirection: "column", gap: 8 }}
          // 4. Mapeia as opções para desabilitar checkboxes dinamicamente
          options={perfisSistema.map((perfil) => ({
            ...perfil,
            disabled:
              (adminSelecionado &&
                !["ADMIN", "INATIVO"].includes(perfil.value)) ||
              (outrosPerfisSelecionados && perfil.value === "ADMIN"),
          }))}
          // 5. Usa o handler para corrigir a seleção
          onChange={handlePerfisChange}
        />
      </Form.Item>

      {/* O campo de Clínicas continua sendo renderizado condicionalmente */}
      {!adminSelecionado && (
        <Form.Item
          label="Clínicas"
          name="clinicaIds"
          rules={[
            { required: true, message: "Selecione ao menos uma clínica" },
          ]}
        >
          <Select
            mode="multiple"
            showSearch
            placeholder="Selecione as clínicas para vincular"
            optionFilterProp="label"
            loading={loadingClinicas}
            options={clinicas}
          />
        </Form.Item>
      )}
    </div>
  );
};

export default FormularioCadastroUsuarioDoSistema;
