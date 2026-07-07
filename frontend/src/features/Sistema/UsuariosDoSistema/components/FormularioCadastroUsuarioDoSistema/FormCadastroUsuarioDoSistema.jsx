import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  Select,
  Checkbox,
  Button,
  message,
  Row,
  Col,
  Divider,
  Switch,
} from "antd";
import { clinicasAPI } from "../../../../../api/sistema/clinicasAPI"; // Ajuste o caminho

const FormCadastroUsuarioDoSistema = ({ onFinish, onCancel, isSubmitting }) => {
  const [form] = Form.useForm();
  const [clinicas, setClinicas] = useState([]);
  const [loadingClinicas, setLoadingClinicas] = useState(false);

  // 1. A FONTE DA VERDADE: Observamos o campo 'perfis' em tempo real.
  const perfisSelecionados = Form.useWatch("perfis", form) || [];

  // 2. ESTADO DERIVADO: Calculamos as flags lógicas a partir dos perfis selecionados.
  const isAdminSelecionado = perfisSelecionados.includes("ADMIN");
  const isOutroPerfilSelecionado = perfisSelecionados.some(
    (p) => p !== "ADMIN"
  );

  // Opções de perfis
  const perfisOptions = [
    { label: "Admin", value: "ADMIN" },
    { label: "Gestor", value: "GESTOR" },
    { label: "Veterinário", value: "VETERINARIO" },
    // { label: "Zootecnista", value: "ZOOTECNISTA" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Técnico", value: "TECNICO" },
  ];

  // 3. EFEITOS COLATERAIS: Reagimos às mudanças no estado derivado.
  useEffect(() => {
    // Se o usuário é admin, limpa a seleção de clínicas.
    if (isAdminSelecionado) {
      form.setFieldsValue({ clinicaIds: [] });
      setClinicas([]);
      return;
    }

    // Se NÃO for admin (ou seja, se outros perfis forem selecionados), busca as clínicas.
    setLoadingClinicas(true);
    clinicasAPI
      .listar()
      .then((data) => {
        const options = data.map((c) => ({
          label: c.nomeClinica,
          value: c.id,
        }));
        setClinicas(options);
      })
      .catch(() => message.error("Falha ao carregar a lista de clínicas."))
      .finally(() => setLoadingClinicas(false));
  }, [isAdminSelecionado, form]);

  // Handler para corrigir a seleção de perfis
  const handlePerfisChange = (selecaoAtual) => {
    let selecaoCorrigida = [...selecaoAtual];

    // Se ADMIN foi selecionado, ele se torna o único perfil.
    if (selecaoAtual.includes("ADMIN")) {
      selecaoCorrigida = ["ADMIN"];
    }
    // Se outro perfil foi selecionado, garante que ADMIN seja removido.
    else if (isOutroPerfilSelecionado) {
      selecaoCorrigida = selecaoAtual.filter((p) => p !== "ADMIN");
    }

    // Atualiza o valor no formulário apenas se a lógica alterou a seleção do usuário.
    if (JSON.stringify(selecaoCorrigida) !== JSON.stringify(selecaoAtual)) {
      form.setFieldsValue({ perfis: selecaoCorrigida });
    }
  };

  // Prepara as opções para o Checkbox.Group, desabilitando itens conforme a regra.
  const opcoesCheckboxDinamicas = perfisOptions.map((opt) => ({
    ...opt,
    disabled:
      // Se ADMIN está selecionado, desabilita todos os outros.
      (isAdminSelecionado && opt.value !== "ADMIN") ||
      // Se outro perfil está selecionado, desabilita o ADMIN.
      (isOutroPerfilSelecionado && opt.value === "ADMIN"),
  }));

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      {/* Campo 'isAdmin' oculto para corresponder ao DTO, seu valor é derivado dos perfis */}
      <Form.Item name="isAdmin" hidden initialValue={isAdminSelecionado}>
        <Input />
      </Form.Item>

      <Row gutter={24}>
        {/* --- Coluna 1: Informações Pessoais --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Dados Pessoais
          </Divider>
          <Form.Item
            name="nome"
            label="Nome Completo"
            rules={[{ required: true }]}
          >
            <Input placeholder="Digite o nome completo" />
          </Form.Item>
          <Form.Item
            name="email"
            label="E-mail"
            rules={[{ required: true, type: "email" }]}
          >
            <Input placeholder="exemplo@email.com" />
          </Form.Item>

          <Divider orientation="left" plain>
            Segurança
          </Divider>
          <Form.Item name="senha" label="Senha" rules={[{ required: true }]}>
            <Input.Password placeholder="Crie uma senha forte" />
          </Form.Item>
          <Form.Item
            name="confirmarSenha"
            label="Confirmar Senha"
            dependencies={["senha"]}
            rules={[
              { required: true },
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
        </Col>

        {/* --- Coluna 2: Acesso e Permissões --- */}
        <Col xs={24} lg={12}>
          <Divider orientation="left" plain>
            Permissões e Acesso
          </Divider>

          <Form.Item name="perfis" label="Perfis" rules={[{ required: true }]}>
            <Checkbox.Group
              options={opcoesCheckboxDinamicas}
              onChange={handlePerfisChange}
              style={{ display: "flex", flexDirection: "column", gap: 8 }}
            />
          </Form.Item>

          <Form.Item
            label="Status"
            name="isAtivo"
            valuePropName="checked"
            initialValue={true}
          >
            <Switch checkedChildren="ATIVO" unCheckedChildren="INATIVO" />
          </Form.Item>

          {/* O campo de Clínicas é renderizado se NÃO for um admin */}
          {!isAdminSelecionado && (
            <Form.Item
              label="Vincular Clínicas"
              name="clinicaIds"
              rules={[
                {
                  required: !isAdminSelecionado,
                  message: "Selecione ao menos uma clínica",
                },
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
        </Col>
      </Row>

      <div style={{ textAlign: "right", marginTop: 24 }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>
          Cancelar
        </Button>
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Cadastrar
        </Button>
      </div>
    </Form>
  );
};

export default FormCadastroUsuarioDoSistema;
