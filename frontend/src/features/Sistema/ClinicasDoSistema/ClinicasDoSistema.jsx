import React from "react";
import { Button, Modal } from "antd";
import { PlusOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import { useClinicasDoSistemaManager } from "./useClinicasDoSistemaManager";
import TabelaClinicasDoSistema from "./components/TabelaClinicasDoSistema/TabelaClinicasDoSistema.jsx";
import FormularioEdicaoClinicaDoSistema from "./components/FormularioEdicaoClinicaDoSistema/FormEdicaoClinicaDoSistema.jsx";
import FormularioCadastroClinicaDoSistema from "./components/FormularioCadastroClinicaDoSistema/FormCadastroClinicaDoSistema.jsx";
import FormularioDetalhesClinicaDoSistema from "./components/FormularioDetalhesClinicaDoSistema/FormDetalhesClinicaDoSistema.jsx";
import FormularioAcessoClinicaDoSistema from "./components/FormularioAcessoClinicaDoSistema/FormAcessoClinicaDoSistema.jsx";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";

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
          Clínicas do Sistema
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

      {/* Modal Cadastro/Edição */}
      <Modal
        title={editingClinica ? "Editar Clínica" : "Cadastrar Nova Clínica"}
        open={isModalOpen}
        onCancel={handleCloseModals}
        footer={null}
        destroyOnClose
        width="min(850px, 95vw)"
        centered
      >
        {editingClinica ? (
          <FormularioEdicaoClinicaDoSistema
            initialValues={editingClinica}
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
          />
        ) : (
          <FormularioCadastroClinicaDoSistema
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      {/* Modal Detalhes */}
      <Modal
        title="Detalhes da Clínica"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModals}
        footer={[
          <Button key="close" onClick={handleCloseModals}>
            Fechar
          </Button>,
        ]}
        width="min(800px, 95vw)"
        destroyOnClose
        centered
      >
        {viewingClinica && (
          <FormularioDetalhesClinicaDoSistema valores={viewingClinica} />
        )}
      </Modal>

      {/* Modal Acesso (Simulação) */}
      <Modal
        title={`Simular Acesso: ${accessingClinica?.nomeClinica}`}
        open={isModalAcessoOpen}
        onCancel={handleCloseModals}
        footer={null}
        destroyOnClose
        centered
      >
        {accessingClinica && (
          <FormularioAcessoClinicaDoSistema
            clinica={accessingClinica}
            onFinish={handleSimularAcesso}
          />
        )}
      </Modal>
    </div>
  );
};

export default ClinicasDoSistema;
