import React from "react";
import { Modal, Button, Skeleton, Typography } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import ModalDetalhesCalculoRacao from "./components/ModalDetalhesCalculosRacao/ModalDetalhesCalculosRacao";

const { Title } = Typography;

const RacoesDaClinica = () => {
  const {
    racoes,
    loading,
    isModalOpen,
    editingRacao,
    isViewModalOpen,
    viewingRacao,
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
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onView={handleOpenViewModal}
        loading={loading}
      />

      {/* Modal de Cadastro/Edição */}
      <Modal
        title={
          editingRacao ? (
            <Title level={3} className="text-verde-escuro m-0">
              {editingRacao?.nome}
            </Title>
          ) : (
            <Title level={3} className="text-verde-escuro m-0">
              Cadastrando Nova Ração
            </Title>
          )
        }
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

      {/* Modal de Detalhes dos Cálculos */}
      <Modal
        title={`Memória de Cálculo - ${viewingRacao?.nome}`}
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
        <ModalDetalhesCalculoRacao racao={viewingRacao} />
      </Modal>
    </div>
  );
};

export default RacoesDaClinica;
