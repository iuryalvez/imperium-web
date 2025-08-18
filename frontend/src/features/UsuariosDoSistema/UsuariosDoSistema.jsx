import React, { useState, useEffect, useCallback } from "react";
import { Button, Modal } from "antd";
import { PlusOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import { useUsuariosDoSistemaManager } from "./useUsuariosDoSistemaManager";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import FormCadastroUsuarioDoSistema from "./components/FormularioCadastroUsuarioDoSistema/FormCadastroUsuarioDoSistema.jsx";
import FormEdicaoUsuarioDoSistema from "./components/FormularioEdicaoUsuarioDoSistema/FormEdicaoUsuarioDoSistema.jsx";
import TabelaUsuariosDoSistema from "./components/TabelaUsuariosDoSistema/TabelaUsuariosDoSistema.jsx";

const UsuariosDoSistema = () => {
  // O hook agora gerencia apenas a lógica de dados e modais, não mais as colunas.
  const {
    usuarios,
    loading,
    loadingRowKey,
    isModalVisible,
    isSubmitting,
    editingUser,
    handleAbrirModalCadastro,
    handleModalCancel,
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
          editingUser ? "Editar Usuário" : "Cadastrar Novo Usuário"
        }
        open={isModalVisible}
        onCancel={handleModalCancel}
        footer={null}
        destroyOnClose
        width={700}
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
    </div>
  );
};

export default UsuariosDoSistema;
