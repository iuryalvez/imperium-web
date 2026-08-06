import React from "react";
import { Button, Modal, Skeleton, Space } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  CalculatorOutlined,
} from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import ModalDetalhesCalculosRacao from "./components/ModalDetalhesCalculosRacao/ModalDetalhesCalculosRacao";
import ModalPadrao from "../../../components/modal/ModalPadrao";

const RacoesDaClinica = () => {
  const {
    racoes,
    total,
    page,
    pageSize,
    loading,
    isModalOpen,
    editingRacao,
    isViewModalOpen,
    viewingRacao,
    isSubmitting,
    handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleOpenViewModal,
    handleCloseViewModal,
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
        total={total}
        page={page}
        pageSize={pageSize}
        onTableChange={handleTableChange}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onView={handleOpenViewModal}
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

      {/* Modal de Detalhes dos Cálculos */}
      <Modal
        title={
          <Space>
            <CalculatorOutlined></CalculatorOutlined>
            <span>Memória de Cálculo - {viewingRacao?.nome}</span>
          </Space>
        }
        open={isViewModalOpen}
        onCancel={handleCloseViewModal}
        footer={[
          <Button key="close" onClick={handleCloseViewModal}>
            Fechar
          </Button>,
        ]}
        destroyOnClose
        width={700}
        centered
        styles={{
          body: {
            maxHeight: "70vh",
            overflowY: "auto",
            paddingRight: "8px", // Prevents content from touching the scrollbar
          },
        }}
      >
        <ModalDetalhesCalculosRacao racao={viewingRacao} />
      </Modal>
    </div>
  );
};

export default RacoesDaClinica;
