import React from "react";
import { Button } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { useUsuariosDaClinicaManager } from "./useUsuariosDaClinicaManager.js";
import TabelaUsuariosDaClinica from "./components/TabelaUsuariosDaClinica/TabelaUsuariosDaClinica.jsx";
import FormEdicaoUsuarioDaClinica from "./components/FormularioEdicaoUsuarioDaClinica/FormEdicaoUsuarioDaClinica.jsx";
import FormCadastroUsuarioDaClinica from "./components/FormularioCadastroUsuarioDaClinica/FormCadastroUsuarioDaClinica.jsx";
import FormDetalhesUsuarioDaClinica from "./components/FormularioDetalhesUsuarioDaClinica/FormularioDetalhesUsuarioDaClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

const UsuariosDaClinica = () => {
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
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Usuários
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

      {/* Modal de Criação / Edição */}
      <ModalPadrao
        modo={editingUser ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={editingUser ? "Editar Usuário" : "Cadastrar Novo Usuário"}
        icon={editingUser ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingUser
            ? "form-edicao-usuario-clinica"
            : "form-cadastro-usuario-clinica"
        }
        isSubmitting={isSubmitting}
      >
        {editingUser ? (
          <FormEdicaoUsuarioDaClinica
            onFinish={handleSubmit}
            initialValues={editingUser}
          />
        ) : (
          <FormCadastroUsuarioDaClinica onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      {/* Modal de Detalhes */}
      <ModalPadrao
        modo="detalhes"
        open={isOpenModalDetalhes}
        onCancel={handleCloseModalDetalhes}
        title={`Detalhes de ${viewingUser?.nome || "Usuário"}`}
        icon={<ProfileOutlined />}
      >
        {viewingUser && <FormDetalhesUsuarioDaClinica valores={viewingUser} />}
      </ModalPadrao>
    </div>
  );
};

export default UsuariosDaClinica;
