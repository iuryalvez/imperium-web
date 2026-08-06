import React from "react";
import { Button } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { useTutoresDaClinicaManager } from "./useTutoresDaClinicaManager.js";
import TabelaTutoresDaClinica from "./components/TabelaTutoresDaClinica/TabelaTutoresDaClinica.jsx";
import FormularioEdicaoTutorDaClinica from "./components/FormularioEdicaoTutorDaClinica/FormEdicaoTutorDaClinica.jsx";
import FormularioCadastroTutorDaClinica from "./components/FormularioCadastroTutorDaClinica/FormCadastroTutorDaClinica.jsx";
import FormularioDetalhesTutorDaClinica from "./components/FormularioDetalhesTutorDaClinica/FormDetalhesTutorDaClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

const TutoresDaClinica = () => {
  const {
    tutores,
    total,
    page,
    pageSize,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingTutor,
    viewingTutor,
    handleTableChange,
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
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Tutores
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
        total={total}
        page={page}
        pageSize={pageSize}
        onTableChange={handleTableChange}
        onEdit={handleEditarTutor}
        onDelete={handleDeletarTutor}
        onViewDetails={handleOpenModalDetalhes}
        loadingRowKey={loadingRowKey}
      />

      <ModalPadrao
        modo={editingTutor ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={
          editingTutor ? `Editar ${editingTutor.nome}` : "Cadastrar novo tutor"
        }
        icon={editingTutor ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingTutor
            ? "form-edicao-tutor-clinica"
            : "form-cadastro-tutor-clinica"
        }
        isSubmitting={isSubmitting}
      >
        {editingTutor ? (
          <FormularioEdicaoTutorDaClinica
            onFinish={handleSubmit}
            initialValues={editingTutor}
          />
        ) : (
          <FormularioCadastroTutorDaClinica onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      <ModalPadrao
        modo="detalhes"
        open={isOpenModalDetalhes}
        onCancel={handleCloseModalDetalhes}
        title={`Informações de ${viewingTutor?.nome}`}
        icon={<ProfileOutlined />}
      >
        {viewingTutor && (
          <FormularioDetalhesTutorDaClinica valores={viewingTutor} />
        )}
      </ModalPadrao>
    </div>
  );
};

export default TutoresDaClinica;
