import React from "react";
import { Button } from "antd";
import {
  PlusOutlined,
  ArrowLeftOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useUsuariosDoSistemaManager } from "./useUsuariosDoSistemaManager.js";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import FormCadastroUsuarioDoSistema from "./components/FormularioCadastroUsuarioDoSistema/FormCadastroUsuarioDoSistema.jsx";
import FormEdicaoUsuarioDoSistema from "./components/FormularioEdicaoUsuarioDoSistema/FormEdicaoUsuarioDoSistema.jsx";
import TabelaUsuariosDoSistema from "./components/TabelaUsuariosDoSistema/TabelaUsuariosDoSistema.jsx";
import FormDetalhesUsuarioDoSistema from "./components/FormularioDetalhesUsuarioDoSistema/FormDetalhesUsuarioDoSistema.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

const UsuariosDoSistema = () => {
  const {
    usuarios,
    total,
    page,
    pageSize,
    loading,
    loadingRowKey,
    isModalVisible,
    isDetailsModalVisible,
    isSubmitting,
    editingUser,
    viewingUser,
    handleTableChange,
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
          Gerenciamento de Usuários do Sistema
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
        total={total}
        page={page}
        pageSize={pageSize}
        onTableChange={handleTableChange}
        onEdit={handleEditarUsuario}
        onToggleStatus={handleToggleStatus}
        onViewDetails={handleVerDetalhes}
        loadingRowKey={loadingRowKey}
      />

      {/* MODAL PADRÃO: Cadastro / Edição */}
      <ModalPadrao
        modo={editingUser ? "edicao" : "criacao"}
        open={isModalVisible}
        onCancel={handleModalCancel}
        title={editingUser ? "Editar Usuário" : "Cadastrar Novo Usuário"}
        icon={editingUser ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingUser
            ? "form-edicao-usuario-sistema"
            : "form-cadastro-usuario-sistema"
        }
        isSubmitting={isSubmitting}
      >
        {editingUser ? (
          <FormEdicaoUsuarioDoSistema
            onFinish={handleFormFinish}
            initialValues={editingUser}
          />
        ) : (
          <FormCadastroUsuarioDoSistema onFinish={handleFormFinish} />
        )}
      </ModalPadrao>

      {/* MODAL PADRÃO: Detalhes */}
      <ModalPadrao
        modo="detalhes"
        open={isDetailsModalVisible}
        onCancel={handleDetailsModalCancel}
        title={`Detalhes de ${viewingUser?.nome || "Usuário"}`}
        icon={<ProfileOutlined />}
      >
        {viewingUser && <FormDetalhesUsuarioDoSistema valores={viewingUser} />}
      </ModalPadrao>
    </div>
  );
};

export default UsuariosDoSistema;
