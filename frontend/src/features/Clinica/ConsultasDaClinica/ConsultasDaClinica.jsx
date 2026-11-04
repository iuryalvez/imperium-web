import React from "react";
import { Button, Input } from "antd";
import { PlusOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
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
    handleIrParaAcompanhamento,
    handleCancelar,
    handleReabrir,
    paginacao,
    setBusca,
    handleTableChange,
  } = useConsultasManager();


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

      {/* Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela */}
      <Input.Search
        placeholder="Buscar por animal ou tutor..."
        onChange={(e) => setBusca(e.target.value)}
        allowClear
        style={{ maxWidth: 320 }}
        className="mb-4"
      />

      <TabelaConsultas
        paginacao={paginacao}
        onChange={handleTableChange}
        consultas={consultas}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onIrParaAcompanhamento={handleIrParaAcompanhamento}
        onCancelar={handleCancelar}
        onReabrir={handleReabrir}
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
