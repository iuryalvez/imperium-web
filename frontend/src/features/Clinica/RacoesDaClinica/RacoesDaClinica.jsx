import React from "react";
import { Button, Skeleton } from "antd";
import { PlusOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import ModalPadrao from "../../../components/modal/ModalPadrao";

const RacoesDaClinica = () => {
  const {
    racoes,
    loading,
    isModalOpen,
    editingRacao,
    isSubmitting,
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

      <ModalPadrao
        modo={editingRacao ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={editingRacao ? "Editar Ração" : "Cadastrar Nova Ração"}
        icon={editingRacao ? <EditOutlined /> : <FormOutlined />}
        formId="form-cadastro-edicao-racao-clinica"
        isSubmitting={isSubmitting}
      >
        <FormCadastroEdicaoRacaoClinica
          initialValues={editingRacao}
          onFinish={handleSubmit}
        />
      </ModalPadrao>
    </div>
  );
};

export default RacoesDaClinica;
