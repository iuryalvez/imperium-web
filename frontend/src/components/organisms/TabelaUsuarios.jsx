import { useState, useEffect } from "react";
import { Table, Button, Space, Checkbox } from "antd";
import { usuario as usuarioAPI } from "../../api/usuariosAPI";
import { PlusOutlined, EditOutlined } from "@ant-design/icons";
import "../styles/TabelaUsuarios.css";

const TabelaUsuarios = ({ usuarios: propUsuarios, onEditarUsuario, onAdicionarUsuario }) => {
  const [usuarios, setUsuarios] = useState(propUsuarios);
  const [loadingId, setLoadingId] = useState(null);

  useEffect(() => {
    setUsuarios(propUsuarios); // Atualiza o estado quando a prop `usuarios` mudar
  }, [propUsuarios]);

  const handleToggleHabilitado = async (upcId, checked) => {
    try {
      setLoadingId(upcId);
      if (checked) {
        await usuarioAPI.habilitar(upcId);
      } else {
        await usuarioAPI.desabilitar(upcId);
      }

      // Atualiza imediatamente o estado local para refletir a mudança
      setUsuarios((prevUsuarios) =>
        prevUsuarios.map((usuario) =>
          usuario.upcId === upcId ? { ...usuario, habilitado: checked } : usuario
        )
      );
    } catch (error) {
      console.error("Erro ao atualizar usuário:", error);
    } finally {
      setLoadingId(null);
    }
  };

  const columns = [
    {
      title: "Nome",
      dataIndex: "nome",
      key: "nome",
    },
    {
      title: "Perfil",
      dataIndex: "perfil",
      key: "perfil",
      sorter: (a, b) => a.perfil.localeCompare(b.perfil),
    },
    {
      title: "Data de Cadastro",
      dataIndex: "dataCadastro",
      key: "dataCadastro",
      sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
      render: (data) => new Date(data).toLocaleDateString("pt-BR"),
    },
    {
      title: "Último Acesso",
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
    {
      title: "Ações",
      key: "acoes",
      render: (_, record) => (
        <Space>
          <Button
            icon={<EditOutlined />}
            onClick={() => onEditarUsuario(record)}
            style={{
              backgroundColor: "var(--verde-medio)",
              borderColor: "var(--verde-medio)",
              color: "white"
            }}
            hover={{ backgroundColor: "var(--verde-realce)", borderColor: "var(--verde-realce)" }}
          >
            Editar
          </Button>
        </Space>
      )
    }
  ];

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 16 }}>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={onAdicionarUsuario}
          style={{
            backgroundColor: "var(--verde-medio)",
            borderColor: "var(--verde-medio)",
          }}
          hover={{ backgroundColor: "var(--verde-realce)", borderColor: "var(--verde-realce)" }}
        >
          Adicionar Usuário
        </Button>
      </div>
      <Table
        className="custom-table"
        dataSource={usuarios}
        columns={columns}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />
    </div>
  );
};

export default TabelaUsuarios;
