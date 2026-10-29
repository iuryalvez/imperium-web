import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../../../services/AuthContext";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useUsuariosDaClinicaManager } from "./useUsuariosDaClinicaManager.js";
import TabelaUsuariosDaClinica from "./components/TabelaUsuariosDaClinica/TabelaUsuariosDaClinica.jsx";
import FormEdicaoUsuarioDaClinica from "./components/FormularioEdicaoUsuarioDaClinica/FormEdicaoUsuarioDaClinica.jsx";
import FormCadastroUsuarioDaClinica from "./components/FormularioCadastroUsuarioDaClinica/FormCadastroUsuarioDaClinica.jsx";
import FormDetalhesUsuarioDaClinica from "./components/FormularioDetalhesUsuarioDaClinica/FormularioDetalhesUsuarioDaClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

const UsuariosDaClinicaConteudo = () => {
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
    paginacao,
    setBusca,
    handleTableChange,
  } = useUsuariosDaClinicaManager();


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

      {/* Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela */}
      <Input.Search
        placeholder="Buscar por nome, e-mail ou CPF..."
        onChange={(e) => setBusca(e.target.value)}
        allowClear
        style={{ maxWidth: 320 }}
        className="mb-4"
      />

      <TabelaUsuariosDaClinica
        loading={loading}
        paginacao={paginacao}
        onChange={handleTableChange}
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

/**
 * Usuários da clínica: só para GESTOR (ou admin simulando acesso).
 * O backend também restringe (policy GerenciaClinica); aqui só evitamos mostrar a tela.
 */
const UsuariosDaClinica = () => {
  const { perfis, isAdmin } = useAuth();
  const podeGerirClinica = isAdmin || (perfis || []).includes("GESTOR");

  if (!podeGerirClinica) return <Navigate to="/clinica/inicio" replace />;
  return <UsuariosDaClinicaConteudo />;
};

export default UsuariosDaClinica;
