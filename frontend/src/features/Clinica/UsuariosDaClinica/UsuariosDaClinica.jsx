import React from "react";
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
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
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
    paginacao,
    setBusca,
    handleTableChange,
  } = useUsuariosDaClinicaManager();


  return (
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Usuários"
        acoes={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
            Cadastrar Usuário
          </Button>
        }
      >
        <TabelaUsuariosDaClinica
          barra={
            // Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela
            <Input.Search
              placeholder="Buscar por nome, e-mail ou CPF..."
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
          loading={loading}
          paginacao={paginacao}
          onChange={handleTableChange}
          usuarios={usuarios}
          onEdit={handleEditarUsuario}
          onToggleStatus={handleToggleStatus}
          onViewDetails={handleOpenModalDetalhes}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

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
    </>
  );
};

export default UsuariosDaClinica;
