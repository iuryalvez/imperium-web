import React from "react";
import { Button } from "antd";
import {
  PlusOutlined,
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
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

// Breadcrumb: Início do admin > esta tela
const TRILHA = [{ titulo: "Início", para: "/sistema" }];

const UsuariosDoSistema = () => {
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
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Usuários do Sistema"
        trilha={TRILHA}
        acoes={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAbrirModalCadastro}>
            Cadastrar Usuário
          </Button>
        }
      >
        <TabelaUsuariosDoSistema
          data={usuarios}
          onEdit={handleEditarUsuario}
          onToggleStatus={handleToggleStatus}
          onViewDetails={handleVerDetalhes}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

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
    </>
  );
};

export default UsuariosDoSistema;
