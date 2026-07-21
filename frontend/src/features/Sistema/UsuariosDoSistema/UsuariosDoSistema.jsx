import React from "react";
import { Button, Modal, Typography } from "antd";
import { PlusOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import { useUsuariosDoSistemaManager } from "./useUsuariosDoSistemaManager.js";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import FormCadastroUsuarioDoSistema from "./components/FormularioCadastroUsuarioDoSistema/FormCadastroUsuarioDoSistema.jsx";
import FormEdicaoUsuarioDoSistema from "./components/FormularioEdicaoUsuarioDoSistema/FormEdicaoUsuarioDoSistema.jsx";
import TabelaUsuariosDoSistema from "./components/TabelaUsuariosDoSistema/TabelaUsuariosDoSistema.jsx";
import FormDetalhesUsuarioDoSistema from "./components/FormularioDetalhesUsuarioDoSistema/FormDetalhesUsuarioDoSistema.jsx";

const { Title } = Typography;

const UsuariosDoSistema = () => {
  // O hook agora gerencia apenas a lógica de dados e modais, não mais as colunas.
  const {
    usuarios,
    loading,
    loadingRowKey,
    isModalVisible,
    isDetailsModalVisible,
    isSubmitting,
    editingUser,
    viewingUser,
    handleAbrirModalCadastro,
    handleModalCancel,
    handleDetailsModalCancel,
    handleFormFinish,
    handleEditarUsuario,
    handleToggleStatus,
    handleVerDetalhes,
  } = useUsuariosDoSistemaManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <div>
      <button
        onClick={() => window.history.back()}
        className="flex items-center font-bold text-[var(--verde-medio)] mb-2"
      >
        <ArrowLeftOutlined className="mr-2" /> Voltar
      </button>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Lista de Usuários do Sistema
        </h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={handleAbrirModalCadastro}
        >
          Cadastrar Usuário
        </Button>
      </div>

      <TabelaUsuariosDoSistema
        data={usuarios}
        onEdit={handleEditarUsuario}
        onToggleStatus={handleToggleStatus}
        onViewDetails={handleVerDetalhes}
        loadingRowKey={loadingRowKey}
      />

      <Modal
        title={
          editingUser ? (
            <Title level={3} className="text-verde-escuro m-0">
              {editingUser?.nome}
            </Title>
          ) : (
            <Title level={3} className="text-verde-escuro m-0">
              Cadastrando Novo Usuário
            </Title>
          )
        }
        open={isModalVisible}
        onCancel={handleModalCancel}
        footer={null}
        centered
        destroyOnClose
        width="min(800px, 95vw)"
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto", overflowX: "hidden" },
        }}
      >
        {editingUser ? (
          <FormEdicaoUsuarioDoSistema
            onFinish={handleFormFinish}
            onCancel={handleModalCancel}
            isSubmitting={isSubmitting}
            initialValues={editingUser}
          />
        ) : (
          <FormCadastroUsuarioDoSistema
            onFinish={handleFormFinish}
            onCancel={handleModalCancel}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      <Modal
        title={
          <Title level={3} className="text-verde-escuro m-0">
            Exibindo {viewingUser?.nome}
          </Title>
        }
        open={isDetailsModalVisible}
        onCancel={handleDetailsModalCancel}
        footer={[
          <Button key="close" onClick={handleDetailsModalCancel}>
            Fechar
          </Button>,
        ]}
        centered
        destroyOnClose
        width="min(800px, 95vw)"
      >
        {/* Renderiza o componente de detalhes apenas se houver um usuário selecionado */}
        {viewingUser && (
          <div
            style={{
              maxHeight: "70vh",
              overflowY: "auto",
              overflowX: "hidden",
              paddingRight: "12px",
            }}
          >
            <FormDetalhesUsuarioDoSistema valores={viewingUser} />
          </div>
        )}
      </Modal>
    </div>
  );
};

export default UsuariosDoSistema;
