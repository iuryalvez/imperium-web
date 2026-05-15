import React from "react";
import { Modal, Button, Skeleton } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";

const RacoesDaClinica = () => {
  const {
    racoes,
    loading,
    isModalOpen,
    editingRacao,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
  } = useRacoesManager();

  if (loading) {
    return <Skeleton active />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Gerenciamento de Rações</h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
        >
          Cadastrar Ração
        </Button>
      </div>

      <TabelaRacoesDaClinica
        racoes={racoes}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        loading={loading}
      />

      <Modal
        title={editingRacao ? "Editar Ração" : "Cadastrar Nova Ração"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
        width="min(800px, 95vw)"
      >
        <FormCadastroEdicaoRacaoClinica
          initialValues={editingRacao}
          onFinish={handleSubmit}
          onCancel={handleCloseModal}
        />
      </Modal>
    </div>
  );
};

export default RacoesDaClinica;
