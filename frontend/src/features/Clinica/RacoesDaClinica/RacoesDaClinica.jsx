import React from "react";
import { Button, Input, Modal, Select, Space } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  CalculatorOutlined,
} from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import ModalPadrao from "../../../components/modal/ModalPadrao";
import ModalDetalhesCalculoRacao from "./components/ModalDetalhesCalculosRacao/ModalDetalhesCalculosRacao";

const RacoesDaClinica = () => {
  const {
    racoes,
    loading,
    isModalOpen,
    editingRacao,
    isViewModalOpen,
    viewingRacao,
    isSubmitting,
    handleOpenModal,
    handleCloseModal,
    handleOpenViewModal,
    handleCloseViewModal,
    handleSubmit,
    handleDelete,
    paginacao,
    loadingRowKey,
    setBusca,
    setFiltros,
    handleTableChange,
    handleToggleStatus,
    handleToggleOculta,
  } = useRacoesManager();

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

      {/* Busca e origem filtram no servidor; espécie e status ficam nos filtros das colunas */}
      <Space wrap className="mb-4">
        <Input.Search
          placeholder="Buscar por nome..."
          onChange={(e) => setBusca(e.target.value)}
          allowClear
          style={{ width: 260 }}
        />
        <Select
          defaultValue={null}
          style={{ width: 190 }}
          onChange={(value) =>
            setFiltros({
              origem: value === "ocultas" ? undefined : value,
              oculta: value === "ocultas" ? true : undefined,
            })
          }
          options={[
            { value: null, label: "Todas as Origens" },
            { value: "sistema", label: "Apenas do Sistema" },
            { value: "clinica", label: "Apenas da Clínica" },
            { value: "ocultas", label: "Ocultas pela Clínica" },
          ]}
        />
      </Space>

      <TabelaRacoesDaClinica
        racoes={racoes}
        loading={loading}
        paginacao={paginacao}
        onChange={handleTableChange}
        modo="clinica"
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onView={handleOpenViewModal}
        onToggleStatus={handleToggleStatus}
        onToggleOculta={handleToggleOculta}
        loadingRowKey={loadingRowKey}
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
        <ModalDetalhesCalculoRacao racao={viewingRacao} />
      </Modal>
    </div>
  );
};

export default RacoesDaClinica;
