import React from "react";
import { Button, Modal, Space } from "antd";
import {
  PlusOutlined,
  ArrowLeftOutlined,
  DesktopOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useClinicasDoSistemaManager } from "./useClinicasDoSistemaManager";
import TabelaClinicasDoSistema from "./components/TabelaClinicasDoSistema/TabelaClinicasDoSistema.jsx";
import FormularioEdicaoClinicaDoSistema from "./components/FormularioEdicaoClinicaDoSistema/FormEdicaoClinicaDoSistema.jsx";
import FormularioCadastroClinicaDoSistema from "./components/FormularioCadastroClinicaDoSistema/FormCadastroClinicaDoSistema.jsx";
import FormularioDetalhesClinicaDoSistema from "./components/FormularioDetalhesClinicaDoSistema/FormDetalhesClinicaDoSistema.jsx";
import FormularioAcessoClinicaDoSistema from "./components/FormularioAcessoClinicaDoSistema/FormAcessoClinicaDoSistema.jsx";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

const ClinicasDoSistema = () => {
  const {
    clinicas,
    loading,
    isSubmitting,
    isModalOpen,
    isModalDetalhesOpen,
    isModalAcessoOpen,
    editingClinica,
    viewingClinica,
    accessingClinica,
    handleOpenModalCadastro,
    handleEditarClinica,
    handleOpenModalDetalhes,
    handleOpenModalAcesso,
    handleCloseModals,
    handleSubmit,
    handleSimularAcesso,
  } = useClinicasDoSistemaManager();

  if (loading) return <LoadingSpinner />;

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
          Gerenciamento de Clínicas do Sistema
        </h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={handleOpenModalCadastro}
        >
          Cadastrar Clínica
        </Button>
      </div>

      <TabelaClinicasDoSistema
        clinicas={clinicas}
        onEdit={handleEditarClinica}
        onViewDetails={handleOpenModalDetalhes}
        onAccess={handleOpenModalAcesso}
      />

      {/* MODAL PADRÃO: Cadastro / Edição */}
      <ModalPadrao
        modo={editingClinica ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModals}
        title={editingClinica ? "Editar Clínica" : "Cadastrar Nova Clínica"}
        icon={editingClinica ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingClinica
            ? "form-edicao-clinica-sistema"
            : "form-cadastro-clinica-sistema"
        }
        isSubmitting={isSubmitting}
        width="min(850px, 95vw)"
      >
        {editingClinica ? (
          <FormularioEdicaoClinicaDoSistema
            initialValues={editingClinica}
            onFinish={handleSubmit}
          />
        ) : (
          <FormularioCadastroClinicaDoSistema onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      {/* MODAL PADRÃO: Detalhes */}
      <ModalPadrao
        modo="detalhes"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModals}
        title={`Detalhes de ${viewingClinica?.nomeClinica || "Clínica"}`}
        icon={<ProfileOutlined />}
      >
        {viewingClinica && (
          <FormularioDetalhesClinicaDoSistema valores={viewingClinica} />
        )}
      </ModalPadrao>

      {/* MODAL NATIVO (Customizado): Acesso (Simulação) */}
      <Modal
        title={
          <Space>
            <DesktopOutlined className="modal-title-icon" />
            <span className="text-verde-escuro m-0 text-lg">
              Simular Acesso na clínica {accessingClinica?.nomeClinica}
            </span>
          </Space>
        }
        open={isModalAcessoOpen}
        onCancel={handleCloseModals}
        style={{ top: 40 }}
        footer={
          <div className="modal-footer-actions">
            <Button onClick={handleCloseModals}>Cancelar</Button>
            <Button form="form-acesso-clinica" type="primary" htmlType="submit">
              Acessar {accessingClinica?.nomeClinica}
            </Button>
          </div>
        }
        destroyOnClose
        width="min(600px, 95vw)"
      >
        <div className="modal-scroll-container">
          {accessingClinica && (
            <FormularioAcessoClinicaDoSistema
              clinica={accessingClinica}
              onFinish={handleSimularAcesso}
            />
          )}
        </div>
      </Modal>
    </div>
  );
};

export default ClinicasDoSistema;
