import { useState } from "react";
import { message } from "antd";
import { racasAPI } from "../../../api/sistema/racasAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosRaca } from "./components/TabelaRacasDoSistema/colunas";

/**
 * Gestão das raças pelo admin.
 * visao "sistema": catálogo compartilhado (mudanças valem para todas as clínicas).
 * visao "copias": raças próprias das clínicas (inclui as cópias feitas a partir do catálogo).
 */
export const useRacasDoSistemaManager = () => {
  const [visao, setVisao] = useState("sistema");
  const lista = usePaginacaoServidor(
    visao === "copias" ? racasAPI.copias : racasAPI.listar,
    {},
    traduzirFiltrosRaca,
  );

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRaca, setEditingRaca] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [loadingRowKey, setLoadingRowKey] = useState(null);

  const handleAlternarVisao = () => {
    setVisao((atual) => (atual === "sistema" ? "copias" : "sistema"));
  };

  const handleOpenModalCadastro = () => {
    setEditingRaca(null);
    setIsModalOpen(true);
  };

  const handleEditarRaca = (raca) => {
    setEditingRaca(raca);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingRaca(null);
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      const resultado = editingRaca
        ? await racasAPI.editar({ ...editingRaca, ...values })
        : await racasAPI.cadastrar(values);
      message.success(resultado?.message || "Raça salva com sucesso!");
      handleCloseModal();
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Erro ao salvar a raça.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const executarAcao = async (raca, acao) => {
    setLoadingRowKey(raca.id);
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

  const handleDelete = (raca) => executarAcao(raca, () => racasAPI.deletar(raca.id));
  const handleToggleStatus = (raca) => executarAcao(raca, () => racasAPI.setStatus(raca.id, !raca.isAtivo));

  return {
    visao,
    racas: lista.dados,
    loading: lista.loading,
    paginacao: lista.paginacao,
    loadingRowKey,
    isModalOpen,
    isSubmitting,
    editingRaca,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    handleAlternarVisao,
    handleOpenModalCadastro,
    handleEditarRaca,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
  };
};
