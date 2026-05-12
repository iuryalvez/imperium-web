import React from "react";
import { Button, Modal } from "antd";
import { PlusOutlined, ArrowLeftOutlined } from "@ant-design/icons";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import { useTutoresDaClinicaManager } from "./useTutoresDaClinicaManager.js";
import TabelaTutoresDaClinica from "./components/TabelaTutoresDaClinica/TabelaTutoresDaClinica.jsx";
import FormularioEdicaoTutorDaClinica from "./components/FormularioEdicaoTutorDaClinica/FormEdicaoTutorDaClinica.jsx";
import FormularioCadastroTutorDaClinica from "./components/FormularioCadastroTutorDaClinica/FormCadastroTutorDaClinica.jsx";
import FormularioDetalhesTutorDaClinica from "./components/FormularioDetalhesTutorDaClinica/FormDetalhesTutorDaClinica.jsx";

const TutoresDaClinica = () => {
  const {
    tutores,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingTutor,
    viewingTutor,
    handleOpenModalCadastro,
    handleCloseModal,
    handleCloseModalDetalhes,
    handleSubmit,
    handleEditarTutor,
    handleDeletarTutor,
    handleOpenModalDetalhes,
  } = useTutoresDaClinicaManager();

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
          Tutores da Clínica
        </h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={handleOpenModalCadastro}
        >
          Cadastrar Tutor
        </Button>
      </div>

      <TabelaTutoresDaClinica
        tutores={tutores}
        onEdit={handleEditarTutor}
        onDelete={handleDeletarTutor}
        onViewDetails={handleOpenModalDetalhes}
        loadingRowKey={loadingRowKey}
      />

      <Modal
        title={editingTutor ? "Editar Tutor" : "Cadastrar Novo Tutor"}
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
        {editingTutor ? (
          <FormularioEdicaoTutorDaClinica
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
            initialValues={editingTutor}
          />
        ) : (
          <FormularioCadastroTutorDaClinica
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      <Modal
        title="Detalhes do Tutor"
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
        styles={{ body: { maxHeight: "80vh", overflowY: "auto" } }}
      >
        {viewingTutor && <FormularioDetalhesTutorDaClinica valores={viewingTutor} />}
      </Modal>
    </div>
  );
};

export default TutoresDaClinica;