import React from "react";
import { Button, Skeleton } from "antd";
import { PlusOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
import ModalPadrao from "../../../components/modal/ModalPadrao";
const ConsultasDaClinica = () => {
  const {
    consultas,
    total,
    page,
    pageSize,
    loading,
    isModalOpen,
    editingConsulta,
    handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleIrParaAcompanhamento,
  } = useConsultasManager();

  if (loading) {
    return <Skeleton active />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Consultas
        </h1>
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
        total={total}
        page={page}
        pageSize={pageSize}
        onTableChange={handleTableChange}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onIrParaAcompanhamento={handleIrParaAcompanhamento}
        loading={loading}
      />

      {/* Modal de Criação / Edição de Consulta */}
      <ModalPadrao
        modo={editingConsulta ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={editingConsulta ? "Editar Consulta" : "Agendar Nova Consulta"}
        icon={editingConsulta ? <EditOutlined /> : <FormOutlined />}
        formId="form-cadastro-edicao-consulta"
        width="min(500px, 95vw)"
      >
        <FormularioConsulta
          initialValues={editingConsulta}
          onFinish={handleSubmit}
        />
      </ModalPadrao>
    </div>
  );
};

export default ConsultasDaClinica;
