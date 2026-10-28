import React from "react";
import { Button, Input, Modal, Space } from "antd";
import {
  PlusOutlined,
  ArrowLeftOutlined,
  ShopOutlined,
  GlobalOutlined,
} from "@ant-design/icons";
import { useRacasDoSistemaManager } from "./useRacasDoSistemaManager";
import TabelaRacasDoSistema from "./components/TabelaRacasDoSistema/TabelaRacasDoSistema";
// Formulários de raça são os mesmos da clínica (gerenciador de raças)
import FormCadastroRaca from "../../Clinica/AnimaisDaClinica/components/GerenciadorDeRacas/FormCadastroRaca";
import FormEdicaoRaca from "../../Clinica/AnimaisDaClinica/components/GerenciadorDeRacas/FormEdicaoRaca";

const RacasDoSistema = () => {
  const {
    visao,
    racas,
    loading,
    paginacao,
    loadingRowKey,
    isModalOpen,
    isSubmitting,
    editingRaca,
    setBusca,
    handleTableChange,
    handleAlternarVisao,
    handleOpenModalCadastro,
    handleEditarRaca,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
  } = useRacasDoSistemaManager();

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
          {isCopias ? "Raças das Clínicas" : "Raças do Sistema"}
        </h1>
        <Space wrap>
          <Button
            icon={isCopias ? <GlobalOutlined /> : <ShopOutlined />}
            onClick={handleAlternarVisao}
          >
            {isCopias ? "Ver raças do sistema" : "Cópias das clínicas"}
          </Button>
          {!isCopias && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
              Cadastrar Raça
            </Button>
          )}
        </Space>
      </div>

      <Input.Search
        placeholder={isCopias ? "Buscar por raça ou clínica..." : "Buscar por nome..."}
        onChange={(e) => setBusca(e.target.value)}
        allowClear
        style={{ maxWidth: 320 }}
        className="mb-4"
      />

      <TabelaRacasDoSistema
        key={visao}
        racas={racas}
        loading={loading}
        paginacao={paginacao}
        onChange={handleTableChange}
        visao={visao}
        onEdit={handleEditarRaca}
        onDelete={handleDelete}
        onToggleStatus={handleToggleStatus}
        loadingRowKey={loadingRowKey}
      />

      {/* Os formulários de raça já trazem os próprios botões, por isso o Modal nativo sem rodapé */}
      <Modal
        title={editingRaca ? "Editar Raça" : "Cadastrar Raça do Sistema"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
        centered
      >
        {editingRaca ? (
          <FormEdicaoRaca
            initialValues={editingRaca}
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        ) : (
          <FormCadastroRaca
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>
    </div>
  );
};

export default RacasDoSistema;
