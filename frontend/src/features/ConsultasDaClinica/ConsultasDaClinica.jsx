import React from "react";
import { Modal, Button, Skeleton } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
import FormularioRelatorio from "./components/FormularioRelatorio";

const ConsultasDaClinica = () => {
  const {
    consultas,
    loading,
    isModalOpen,
    editingConsulta,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    isRelatorioModalOpen,
    selectedConsulta,
    relatorioData,
    loadingRelatorio,
    handleOpenRelatorioModal,
    handleCloseRelatorioModal,
    handleSubmitRelatorio,
  } = useConsultasManager();

  if (loading) {
    return <Skeleton active />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Gerenciamento de Consultas</h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
        >
          Agendar Consulta
        </Button>
      </div>

      <TabelaConsultas
        consultas={consultas}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onOpenRelatorio={handleOpenRelatorioModal} 
        loading={loading}
      />

      <Modal
        title={`Relatório da Consulta - ${selectedConsulta?.nomeAnimal || ""}`}
        open={isRelatorioModalOpen}
        onCancel={handleCloseRelatorioModal}
        footer={null}
        destroyOnClose
        width={800} // Um modal mais largo para o relatório
      >
        {loadingRelatorio ? (
          <Skeleton active />
        ) : (
          <FormularioRelatorio
            consulta={selectedConsulta}
            initialValues={relatorioData}
            onFinish={handleSubmitRelatorio}
            onCancel={handleCloseRelatorioModal}
          />
        )}
      </Modal>

      <Modal
        title={editingConsulta ? "Editar Consulta" : "Agendar Nova Consulta"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
      >
        <FormularioConsulta
          initialValues={editingConsulta}
          onFinish={handleSubmit}
          onCancel={handleCloseModal}
        />
      </Modal>
    </div>
  );
};

export default ConsultasDaClinica;
