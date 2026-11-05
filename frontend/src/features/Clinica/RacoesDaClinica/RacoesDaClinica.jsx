import React from "react";
import { Button, Input, Select } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  CalculatorOutlined,
} from "@ant-design/icons";
import { useRacoesManager } from "./useRacoesManager";
import { usePermissao } from "../../../utils/usePermissao";
import FormCadastroEdicaoRacaoClinica from "./components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import TabelaRacoesDaClinica from "./components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import ModalPadrao from "../../../components/modal/ModalPadrao";
import PaginaPadrao from "../../../components/layout/PaginaPadrao";
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

  // Cadastrar/editar/ocultar rações: GESTOR, VETERINARIO e TECNICO (o SECRETARIO só visualiza)
  const pode = usePermissao();
  const podeEditar = pode("gerenciaRacoes");

  return (
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Rações"
        acoes={
          podeEditar && (
            <Button type="primary" icon={<PlusOutlined />} onClick={() => handleOpenModal()}>
              Cadastrar Ração
            </Button>
          )
        }
      >
        <TabelaRacoesDaClinica
          barra={
            // Busca e origem filtram no servidor; espécie e status ficam nos filtros das colunas
            <>
              <Input.Search
                placeholder="Buscar por nome..."
                onChange={(e) => setBusca(e.target.value)}
                allowClear
                className="w-full sm:w-64"
              />
              <Select
                defaultValue={null}
                className="w-full sm:w-48"
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
            </>
          }
          racoes={racoes}
          loading={loading}
          paginacao={paginacao}
          onChange={handleTableChange}
          modo="clinica"
          podeEditar={podeEditar}
          onEdit={handleOpenModal}
          onDelete={handleDelete}
          onView={handleOpenViewModal}
          onToggleStatus={handleToggleStatus}
          onToggleOculta={handleToggleOculta}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

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
      <ModalPadrao
        modo="detalhes"
        open={isViewModalOpen}
        onCancel={handleCloseViewModal}
        title={`Memória de Cálculo - ${viewingRacao?.nome ?? ""}`}
        icon={<CalculatorOutlined />}
      >
        <ModalDetalhesCalculoRacao racao={viewingRacao} />
      </ModalPadrao>
    </>
  );
};

export default RacoesDaClinica;
