import { useState, useEffect } from "react";
import { Table, Button, Space, Checkbox } from "antd";
import { PlusOutlined, EditOutlined } from "@ant-design/icons";

import { usuario } from "../../api/clinica/usuariosAPI";

import "../styles/TabelaEntidade.css";

const TabelaEntidade = ({ entidade, dados, onEditar, onAdicionar}) => {
  const [registros, setRegistros] = useState(dados);
  const [loadingId, setLoadingId] = useState(null);

  useEffect(() => {
    setRegistros(dados);
  }, [dados]);

  const handleToggleHabilitado = async (id, checked) => {
    try {
      setLoadingId(id);
      if (checked) {
        await usuario.habilitar(id);
      } else {
        await usuario.desabilitar(id);
      }
      setRegistros((prev) =>
        prev.map((item) => (item.id === id ? { ...item, habilitado: checked } : item))
      );
    } catch (error) {
      console.error("Erro ao atualizar status:", error);
    } finally {
      setLoadingId(null);
    }
  };

  const colunasBase = {
    usuarios: [
      { title: "Nome", dataIndex: "nome", key: "nome" },
      { title: "Perfil", dataIndex: "perfil", key: "perfil" },
      { 
        title: "Data de Cadastro", 
        dataIndex: "dataCadastro", 
        key: "dataCadastro", 
        sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
        render: (data) => new Date(data).toLocaleDateString("pt-BR"),
      },
      { 
        title: "Habilitado", 
        dataIndex: "habilitado", 
        key: "habilitado", 
        render: (habilitado, record) => (
          <Checkbox
            checked={habilitado}
            onChange={(e) => handleToggleHabilitado(record.id, e.target.checked)}
            loading={loadingId === record.id}
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
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />
    </div>
  );
};

export default TabelaEntidade;
