import React from "react";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  CalculatorOutlined,
  ShopOutlined,
  GlobalOutlined,
} from "@ant-design/icons";
import { useRacoesDoSistemaManager } from "./useRacoesDoSistemaManager";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
// Tabela, formulário e memória de cálculo são os mesmos da clínica (a regra nutricional é única)
import TabelaRacoesDaClinica from "../../Clinica/RacoesDaClinica/components/TabelaRacoesDaClinica/TabelaRacoesDaClinica";
import FormCadastroEdicaoRacaoClinica from "../../Clinica/RacoesDaClinica/components/FormularioCadastroEdicaoRacao/FormCadastroEdicaoRacaoClinica";
import ModalDetalhesCalculoRacao from "../../Clinica/RacoesDaClinica/components/ModalDetalhesCalculosRacao/ModalDetalhesCalculosRacao";

// Breadcrumb: Início do admin > esta tela
const TRILHA = [{ titulo: "Início", para: "/sistema" }];

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
    <>
      <PaginaPadrao
        titulo={isCopias ? "Rações das Clínicas" : "Rações do Sistema"}
        trilha={TRILHA}
        acoes={
          <>
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
          </>
        }
      >
        <TabelaRacoesDaClinica
          key={visao}
          barra={
            <Input.Search
              placeholder={isCopias ? "Buscar por ração ou clínica..." : "Buscar por nome..."}
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
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
      </PaginaPadrao>

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

      <ModalPadrao
        modo="detalhes"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModals}
        title={`Memória de Cálculo - ${viewingRacao?.nome ?? ""}`}
        icon={<CalculatorOutlined />}
      >
        <ModalDetalhesCalculoRacao racao={viewingRacao} />
      </ModalPadrao>
    </>
  );
};

export default RacoesDoSistema;
