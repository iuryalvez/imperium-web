import { useState, useEffect } from "react";
import { Table, Button, Space, Checkbox, Select, message } from "antd";
import { PlusOutlined, EditOutlined } from "@ant-design/icons";

import { usuario } from "../../api/clinica/usuariosAPI";

import "../styles/TabelaEntidade.css";

const TabelaEntidade = ({ entidade, dados, onEditar, onAdicionar}) => {
  const [registros, setRegistros] = useState(dados);
  const [loadingId, setLoadingId] = useState(null);
  const [loadingPerfilId, setLoadingPerfilId] = useState(null);

  useEffect(() => {
    setRegistros(dados);
  }, [dados]);

  const handleToggleHabilitado = async (upcId, checked) => {
    try {
      setLoadingId(upcId);
      if (checked) {
        await usuario.habilitar(upcId);
        message.success("Usuário habilitado com sucesso!");
      } else {
        await usuario.desabilitar(upcId);
        message.success("Usuário desabilitado com sucesso!");
      }
      // Atualiza imediatamente o estado local
      setRegistros((prevUsuarios) =>
        prevUsuarios.map((usuario) =>
          usuario.upcId === upcId ? { ...usuario, habilitado: checked } : usuario
        )
      );
    } catch (error) {
      console.error("Erro ao atualizar status:", error);
    } finally {
      setLoadingId(null);
    }
  };

  const handlePerfilChange = async (upcId, novoPerfil) => {
    try {
      setLoadingPerfilId(upcId);
      console.log("Enviando novo perfil:", novoPerfil); // Debug: veja o que está sendo enviado
      await usuario.atualizarPerfil(upcId, novoPerfil);
      message.success("Perfil atualizado com sucesso!");
      setRegistros((prevUsuarios) =>
        prevUsuarios.map((usuario) =>
          usuario.upcId === upcId ? { ...usuario, perfil: novoPerfil } : usuario
        )
      );
    } catch (error) {
      console.error("Erro ao atualizar perfil:", error);
      message.error("Falha ao atualizar perfil.");
    } finally {
      setLoadingPerfilId(null);
    }
  };

  const opcoesPerfil = ["GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO"];

  const colunasBase = {
    usuarios: [
      { title: "Nome", dataIndex: "nome", key: "nome" },
      {
        title: "Perfil",
        dataIndex: "perfil",
        key: "perfil",
        render: (perfil, record) => (
          <Select
            value={perfil}
            onChange={(value) => handlePerfilChange(record.upcId, value)}
            loading={loadingPerfilId === record.upcId}
            style={{ width: 150 }}
          >
            {opcoesPerfil.map((opcao) => (
              <Select.Option key={opcao} value={opcao}>
                {opcao}
              </Select.Option>
            ))}
          </Select>
        ),
      },
      { 
        title: "Data de Cadastro", 
        dataIndex: "dataCadastro", 
        key: "dataCadastro", 
        sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
        render: (data) => new Date(data).toLocaleDateString("pt-BR"),
      },
      { 
        title: "Data de Último Acesso", 
        dataIndex: "dataUltimoAcesso", 
        key: "dataUltimoAcesso", 
        sorter: (a, b) => new Date(a.dataUltimoAcesso) - new Date(b.dataUltimoAcesso),
        render: (data) => new Date(data).toLocaleDateString("pt-BR"),
      },
      { 
        title: "Habilitado", 
        dataIndex: "habilitado", 
        key: "habilitado", 
        render: (habilitado, record) => (
          <Checkbox
            checked={habilitado}
            onChange={(e) => handleToggleHabilitado(record.upcId, e.target.checked)}
            loading={loadingId === record.upcId}
          />
        ),
      },
    ],
    animais: [
      { title: "Nome", dataIndex: "nome", key: "nome" },
      { title: "Espécie", dataIndex: "especie", key: "especie" },
      { title: "Idade", dataIndex: "idade", key: "idade" },
    ],
    tutores: [
      { title: "Nome", dataIndex: "nome", key: "nome" },
      { title: "E-mail", dataIndex: "email", key: "email" },
    ],
  };

  const colunasComAcoes = [
    ...(colunasBase[entidade] || []),
    {
      title: "Ações",
      key: "acoes",
      render: (_, record) => (
        <Space>
          <Button
            icon={<EditOutlined />}
            onClick={() => onEditar(record)}
            style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
          >
            Editar
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 16 }}>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={onAdicionar}
          style={{ backgroundColor: "var(--verde-medio)" }}
        >
          Adicionar {entidade.charAt(0).toUpperCase() + entidade.slice(1)}
        </Button>
      </div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={(record) => record.upcId || record.id}
        pagination={{ pageSize: 10 }}
      />
    </div>
  );
};

export default TabelaEntidade;
