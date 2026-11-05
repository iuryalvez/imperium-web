import React from "react";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useTutoresDaClinicaManager } from "./useTutoresDaClinicaManager.js";
import TabelaTutoresDaClinica from "./components/TabelaTutoresDaClinica/TabelaTutoresDaClinica.jsx";
import FormularioEdicaoTutorDaClinica from "./components/FormularioEdicaoTutorDaClinica/FormEdicaoTutorDaClinica.jsx";
import FormularioCadastroTutorDaClinica from "./components/FormularioCadastroTutorDaClinica/FormCadastroTutorDaClinica.jsx";
import FormularioDetalhesTutorDaClinica from "./components/FormularioDetalhesTutorDaClinica/FormDetalhesTutorDaClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";

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
    handleToggleStatus,
    handleOpenModalDetalhes,
    paginacao,
    setBusca,
    handleTableChange,
  } = useTutoresDaClinicaManager();


  return (
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Tutores"
        acoes={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
            Cadastrar Tutor
          </Button>
        }
      >
        <TabelaTutoresDaClinica
          barra={
            // Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela
            <Input.Search
              placeholder="Buscar por nome, CPF ou e-mail..."
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
          loading={loading}
          paginacao={paginacao}
          onChange={handleTableChange}
          tutores={tutores}
          onEdit={handleEditarTutor}
          onDelete={handleDeletarTutor}
          onToggleStatus={handleToggleStatus}
          onViewDetails={handleOpenModalDetalhes}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

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
    </>
  );
};

export default TutoresDaClinica;
