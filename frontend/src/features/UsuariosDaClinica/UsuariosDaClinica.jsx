import React from "react";
import { Button, Modal } from "antd";
import { PlusOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import { useUsuariosDaClinicaManager } from "./useUsuariosDaClinica.js";
import TabelaUsuariosDaClinica from "./components/TabelaUsuariosDaClinica/TabelaUsuariosDaClinica.jsx";
import FormEdicaoUsuarioDaClinica from "./components/FormularioEdicaoUsuarioDaClinica/FormEdicaoUsuarioDaClinica.jsx";
import FormCadastroUsuarioDaClinica from "./components/FormularioCadastroUsuarioDaClinica/FormCadastroUsuarioDaClinica.jsx";
import FormDetalhesUsuarioDaClinica from "./components/FormularioDetalhesUsuarioDaClinica/FormularioDetalhesUsuarioDaClinica.jsx";


const UsuariosDaClinica = () => {
  // O hook agora gerencia apenas a lógica de dados e modais, não mais as colunas.
  const {
    usuarios,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingUser,
    viewingUser,
    handleOpenModalCadastro,
    handleCloseModal,
    handleCloseModalDetalhes,
    handleSubmit,
    handleEditarUsuario,
    handleToggleStatus,
    handleOpenModalDetalhes,
  } = useUsuariosDaClinicaManager();

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
          onClick={handleOpenModalCadastro}
        >
          Cadastrar Usuário
        </Button>
      </div>

      <TabelaUsuariosDaClinica
        usuarios={usuarios}
        onEdit={handleEditarUsuario}
        onToggleStatus={handleToggleStatus}
        onViewDetails={handleOpenModalDetalhes}
        loadingRowKey={loadingRowKey}
      />

      <Modal
        title={editingUser ? "Editar Usuário" : "Cadastrar Novo Usuário"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        centered
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }}
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto", overflowX: "hidden" },
        }}
      >
        {editingUser ? (
          <FormEdicaoUsuarioDaClinica
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
            initialValues={editingUser}
          />
        ) : (
          <FormCadastroUsuarioDaClinica
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      <Modal
        title="Detalhes do Usuário"
        open={isOpenModalDetalhes}
        onCancel={handleCloseModalDetalhes}
        footer={[
          <Button key="close" onClick={handleCloseModalDetalhes}>
            Fechar
          </Button>,
        ]}
        centered
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }}
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto" },
        }}
      >
        {/* Renderiza o componente de detalhes apenas se houver um usuário selecionado */}
        {viewingUser && <FormDetalhesUsuarioDaClinica valores={viewingUser} />}
      </Modal>
    </div>
  );
};

export default UsuariosDaClinica;
