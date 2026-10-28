import React from "react";
import { Button, Input, Modal, Space } from "antd";
import {
  PlusOutlined,
  ArrowLeftOutlined,
  EditOutlined,
  FormOutlined,
  CalculatorOutlined,
  ShopOutlined,
  GlobalOutlined,
} from "@ant-design/icons";
import { useRacoesDoSistemaManager } from "./useRacoesDoSistemaManager";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";
// Tabela, formulário e memória de cálculo são os mesmos da clínica (a regra nutricional é única)
import TabelaRacoesDaClinica from "../../Clinica/RacoesDaClinica/components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import FormCadastroEdicaoRacaoClinica from "../../Clinica/RacoesDaClinica/components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import ModalDetalhesCalculoRacao from "../../Clinica/RacoesDaClinica/components/ModalDetalhesCalculosRacao/ModalDetalhesCalculosRacao";

const RacoesDoSistema = () => {
  const {
    visao,
    racoes,
    loading,
    paginacao,
    loadingRowKey,
    isModalOpen,
    isModalDetalhesOpen,
    isSubmitting,
    editingRacao,
    viewingRacao,
    setBusca,
    handleTableChange,
    handleAlternarVisao,
    handleOpenModalCadastro,
    handleEditarRacao,
    handleOpenModalDetalhes,
    handleCloseModals,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
  } = useRacoesDoSistemaManager();

  const isCopias = visao === "copias";

  return (
    <div>
      <button
        onClick={() => window.history.back()}
        className="flex items-center font-bold text-[var(--verde-medio)] mb-2"
      >
        <ArrowLeftOutlined className="mr-2" /> Voltar
      </button>

      <div className="flex justify-between items-center mb-4 flex-wrap gap-2">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          {isCopias ? "Rações das Clínicas" : "Rações do Sistema"}
        </h1>
        <Space wrap>
          <Button
            icon={isCopias ? <GlobalOutlined /> : <ShopOutlined />}
            onClick={handleAlternarVisao}
          >
            {isCopias ? "Ver rações do sistema" : "Cópias das clínicas"}
          </Button>
          {!isCopias && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
              Cadastrar Ração
            </Button>
          )}
        </Space>
      </div>

      <Input.Search
        placeholder={isCopias ? "Buscar por ração ou clínica..." : "Buscar por nome..."}
        onChange={(e) => setBusca(e.target.value)}
        allowClear
        style={{ maxWidth: 320 }}
        className="mb-4"
      />

      <TabelaRacoesDaClinica
        key={visao}
        racoes={racoes}
        loading={loading}
        paginacao={paginacao}
        onChange={handleTableChange}
        modo={visao}
        onEdit={handleEditarRacao}
        onDelete={handleDelete}
        onView={handleOpenModalDetalhes}
        onToggleStatus={handleToggleStatus}
        loadingRowKey={loadingRowKey}
      />

      <ModalPadrao
        modo={editingRacao ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModals}
        title={editingRacao ? "Editar Ração" : "Cadastrar Ração do Sistema"}
        icon={editingRacao ? <EditOutlined /> : <FormOutlined />}
        formId="form-cadastro-edicao-racao-clinica"
        isSubmitting={isSubmitting}
      >
        <FormCadastroEdicaoRacaoClinica initialValues={editingRacao} onFinish={handleSubmit} />
      </ModalPadrao>

      <Modal
        title={
          <Space>
            <CalculatorOutlined />
            <span>Memória de Cálculo - {viewingRacao?.nome}</span>
          </Space>
        }
        open={isModalDetalhesOpen}
        onCancel={handleCloseModals}
        footer={[
          <Button key="close" onClick={handleCloseModals}>
            Fechar
          </Button>,
        ]}
        destroyOnClose
        width={700}
        centered
        styles={{ body: { maxHeight: "70vh", overflowY: "auto", paddingRight: "8px" } }}
      >
        <ModalDetalhesCalculoRacao racao={viewingRacao} />
      </Modal>
    </div>
  );
};

export default RacoesDoSistema;
