import { useState } from "react";
import { message } from "antd";
import { racoesAPI } from "../../../api/sistema/racoesAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosRacao } from "../../Clinica/RacoesDaClinica/components/TabelaRacoesDaClinica/colunas";

/**
 * Gestão das rações pelo admin.
 * visao "sistema": catálogo compartilhado (mudanças valem para todas as clínicas).
 * visao "copias": rações próprias das clínicas (inclui as cópias feitas a partir do catálogo).
 */
export const useRacoesDoSistemaManager = () => {
  const [visao, setVisao] = useState("sistema");
  const lista = usePaginacaoServidor(
    visao === "copias" ? racoesAPI.copias : racoesAPI.listar,
    {},
    traduzirFiltrosRacao,
  );

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRacao, setEditingRacao] = useState(null);
  const [isModalDetalhesOpen, setIsModalDetalhesOpen] = useState(false);
  const [viewingRacao, setViewingRacao] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [loadingRowKey, setLoadingRowKey] = useState(null);

  const handleAlternarVisao = () => {
    setVisao((atual) => (atual === "sistema" ? "copias" : "sistema"));
    lista.setFiltros({});
  };

  const handleOpenModalCadastro = () => {
    setEditingRacao(null);
    setIsModalOpen(true);
  };

  const handleEditarRacao = (racao) => {
    setEditingRacao(racao);
    setIsModalOpen(true);
  };

  const handleOpenModalDetalhes = (racao) => {
    setViewingRacao(racao);
    setIsModalDetalhesOpen(true);
  };

  const handleCloseModals = () => {
    setIsModalOpen(false);
    setIsModalDetalhesOpen(false);
    setEditingRacao(null);
    setViewingRacao(null);
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      const resultado = editingRacao
        ? await racoesAPI.editar({ ...editingRacao, ...values })
        : await racoesAPI.cadastrar(values);
      message.success(resultado?.message || "Ração salva com sucesso!");
      handleCloseModals();
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Erro ao salvar a ração.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const executarAcao = async (racao, acao) => {
    setLoadingRowKey(racao.id);
    try {
      const resultado = await acao();
      message.success(resultado?.message || "Operação realizada com sucesso!");
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Erro ao executar a operação.");
    } finally {
      setLoadingRowKey(null);
    }
  };

  const handleDelete = (racao) => executarAcao(racao, () => racoesAPI.deletar(racao.id));
  const handleToggleStatus = (racao) => executarAcao(racao, () => racoesAPI.setStatus(racao.id, !racao.isAtivo));

  return {
    visao,
    racoes: lista.dados,
    loading: lista.loading,
    paginacao: lista.paginacao,
    loadingRowKey,
    isModalOpen,
    isModalDetalhesOpen,
    isSubmitting,
    editingRacao,
    viewingRacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    handleAlternarVisao,
    handleOpenModalCadastro,
    handleEditarRacao,
    handleOpenModalDetalhes,
    handleCloseModals,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
  };
};
