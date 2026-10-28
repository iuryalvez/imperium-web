import { useState } from "react";
import { message } from "antd";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosRacao } from "./components/TabelaRacoesDaClinica/colunas";

/**
 * Rações vistas pela clínica (sistema + próprias), paginadas no servidor.
 * Ração do sistema: editar cria uma cópia da clínica; pode ser ocultada só para a clínica.
 * Ração própria: pode ser editada, ativada/desativada e excluída (se não estiver em uso).
 */
export const useRacoesManager = () => {
  const lista = usePaginacaoServidor(racoesDaClinicaAPI.listar, {}, traduzirFiltrosRacao);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRacao, setEditingRacao] = useState(null);

  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [viewingRacao, setViewingRacao] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [loadingRowKey, setLoadingRowKey] = useState(null);

  const handleOpenModal = (racao = null) => {
    setEditingRacao(racao);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingRacao(null);
  };

  const handleOpenViewModal = (racao) => {
    setViewingRacao(racao);
    setIsViewModalOpen(true);
  };

  const handleCloseViewModal = () => {
    setIsViewModalOpen(false);
    setViewingRacao(null);
  };

  const handleSubmit = async (values) => {
    try {
      setIsSubmitting(true);
      const resultado = editingRacao
        ? await racoesDaClinicaAPI.editar({ ...editingRacao, ...values })
        : await racoesDaClinicaAPI.cadastrar(values);
      message.success(resultado?.message || "Ração salva com sucesso!");
      handleCloseModal();
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Falha na operação.");
    } finally {
      setIsSubmitting(false);
    }
  };

  // Executa uma ação de linha (excluir, status, ocultar) com loading por linha
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

  const handleDelete = (racao) => executarAcao(racao, () => racoesDaClinicaAPI.deletar(racao.id));
  const handleToggleStatus = (racao) => executarAcao(racao, () => racoesDaClinicaAPI.setStatus(racao.id, !racao.isAtivo));
  const handleToggleOculta = (racao) => executarAcao(racao, () => racoesDaClinicaAPI.ocultar(racao.id, !racao.isOculta));

  return {
    racoes: lista.dados,
    loading: lista.loading,
    paginacao: lista.paginacao,
    loadingRowKey,
    isModalOpen,
    editingRacao,
    isViewModalOpen,
    viewingRacao,
    isSubmitting,
    setBusca: lista.setBusca,
    setFiltros: lista.setFiltros,
    handleTableChange: lista.handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleOpenViewModal,
    handleCloseViewModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
    handleToggleOculta,
  };
};
