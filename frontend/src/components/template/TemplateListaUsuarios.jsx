import { useState } from "react";
import { Table, Button, Tag, Space } from "antd";
import { usuario as usuarioAPI } from "../../api/usuariosAPI";

const TemplateListaUsuarios = ({ usuarios, onEditarUsuario, onAdicionarUsuario }) => {
  const [loading, setLoading] = useState(false);

  const handleDesabilitar = async (upcId) => {
    try {
      setLoading(true);
      await usuarioAPI.desabilitar(upcId);
      console.log(`Vínculo de id ${upcId} foi desabilitado.`);
    } catch (error) {
      console.error("Erro ao desabilitar usuário:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleHabilitar = async (upcId) => {
    try {
      setLoading(true);
      await usuarioAPI.habilitar(upcId);
      console.log(`Vínculo de id ${upcId} foi habilitado.`);
    } catch (error) {
      console.error("Erro ao habilitar usuário:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleEditar = (usuario) => {
    onEditarUsuario(usuario);
  };

  const columns = [
    {
      title: "Nome",
      dataIndex: "nome",
      key: "nome",
    },
    {
      title: "Habilitado",
      dataIndex: "habilitado",
      key: "habilitado",
      render: (habilitado) => (
        <Tag color={habilitado ? "green" : "red"}>
          {habilitado ? "Sim" : "Não"}
        </Tag>
      ),
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
      title: "Ações",
      key: "acoes",
      render: (_, record) => (
        <Space>
          <Button type="default" onClick={() => handleEditar(record)}>
            Editar
          </Button>
          {record.habilitado ? (
            <Button type="primary" danger loading={loading} onClick={() => handleDesabilitar(record.upcId)}>
              Desabilitar
            </Button>
          ) : (
            <Button type="primary" loading={loading} onClick={() => handleHabilitar(record.upcId)}>
              Habilitar
            </Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <Button type="primary" onClick={onAdicionarUsuario} style={{ marginBottom: 16 }}>
        Adicionar Usuário
      </Button>
      <Table
        dataSource={usuarios}
        columns={columns}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />
    </div>
  );
};

export default TemplateListaUsuarios;
