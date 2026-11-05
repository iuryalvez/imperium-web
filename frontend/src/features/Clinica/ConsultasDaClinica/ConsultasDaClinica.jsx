import React from "react";
import { Button, Input } from "antd";
import { PlusOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
import PaginaPadrao from "../../../components/layout/PaginaPadrao";
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
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Consultas"
        acoes={
          <Button type="primary" icon={<PlusOutlined />} onClick={() => handleOpenModal()}>
            Agendar Consulta
          </Button>
        }
      >
        <TabelaConsultas
          barra={
            // Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela
            <Input.Search
              placeholder="Buscar por animal ou tutor..."
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
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
      </PaginaPadrao>

      {/* Modal de Criação / Edição de Consulta */}
      <ModalPadrao
        modo={editingConsulta ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={editingConsulta ? "Editar Consulta" : "Agendar Nova Consulta"}
        icon={editingConsulta ? <EditOutlined /> : <FormOutlined />}
        formId="form-cadastro-edicao-consulta"
        tamanho="sm"
      >
        <FormularioConsulta
          initialValues={editingConsulta}
          onFinish={handleSubmit}
        />
      </ModalPadrao>
    </>
  );
};

export default ConsultasDaClinica;
