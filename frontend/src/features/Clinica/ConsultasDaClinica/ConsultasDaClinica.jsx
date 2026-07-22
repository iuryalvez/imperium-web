import React from "react";
import { Button, Modal, Popconfirm, Skeleton, Space } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  FileTextOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
import FormularioRelatorio from "./components/FormularioRelatorio";
import ModalPadrao from "../../../components/modal/ModalPadrao";

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
    handleDeleteRelatorio,
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
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onOpenRelatorio={handleOpenRelatorioModal}
        loading={loading}
      />

      {/* Modal de Relatório da Consulta */}
      <Modal
        title={
          <Space>
            <FileTextOutlined className="modal-title-icon" />
            <span className="text-verde-escuro m-0 text-lg">
              Relatório da Consulta - {selectedConsulta?.nomeAnimal || ""}
            </span>
          </Space>
        }
        open={isRelatorioModalOpen}
        onCancel={handleCloseRelatorioModal}
        style={{ top: 40 }} // Mantém o modal travado no topo, igual ao ModalPadrao
        footer={
          <div className="modal-footer-actions">
            <Button onClick={handleCloseRelatorioModal}>Cancelar</Button>

            {relatorioData && (
              <Popconfirm
                title="Excluir relatório"
                description="Tem certeza que deseja excluir este relatório? Esta ação não pode ser desfeita."
                onConfirm={() => handleDeleteRelatorio(relatorioData.id)}
                okText="Sim, Excluir"
                cancelText="Cancelar"
              >
                <Button icon={<DeleteOutlined />} danger>
                  Excluir
                </Button>
              </Popconfirm>
            )}

            <Button
              form="form-relatorio-consulta-clinica"
              type="primary"
              htmlType="submit"
            >
              Salvar Relatório
            </Button>
          </div>
        }
        destroyOnClose
        width="min(800px, 95vw)"
      >
        <div className="modal-scroll-container">
          {loadingRelatorio ? (
            <Skeleton active />
          ) : (
            <FormularioRelatorio
              consulta={selectedConsulta}
              initialValues={relatorioData}
              onFinish={handleSubmitRelatorio}
            />
          )}
        </div>
      </Modal>

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
