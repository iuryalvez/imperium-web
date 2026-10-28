import { useState } from "react";
import { message } from "antd";
import { racasDaClinicaAPI } from "../../../../../api/clinica/racasDaClinicaAPI";
import { usePaginacaoServidor } from "../../../../../utils/usePaginacaoServidor";

/**
 * Raças vistas pela clínica (sistema + próprias), paginadas no servidor.
 * Raça do sistema: editar cria uma cópia da clínica; pode ser ocultada só para a clínica.
 * Raça própria: pode ser editada, ativada/desativada e excluída (se não estiver em uso).
 */
export const useRacasDaClinicaManager = () => {
  const lista = usePaginacaoServidor(racasDaClinicaAPI.listar, { tamanhoPagina: 12 });
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingRaca, setEditingRaca] = useState(null);
  const [loadingItemId, setLoadingItemId] = useState(null);

  const executarAcao = async (raca, acao) => {
    setLoadingItemId(raca.id);
    try {
      const resultado = await acao();
      message.success(resultado?.message || "Operação realizada com sucesso!");
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Erro ao executar a operação.");
    } finally {
      setLoadingItemId(null);
    }
  };

  const handleDelete = (raca) => executarAcao(raca, () => racasDaClinicaAPI.deletar(raca.id));
  const handleToggleStatus = (raca) => executarAcao(raca, () => racasDaClinicaAPI.setStatus(raca.id, !raca.isAtivo));
  const handleToggleOculta = (raca) => executarAcao(raca, () => racasDaClinicaAPI.ocultar(raca.id, !raca.isOculta));

  const handleOpenModal = (raca = null) => {
    setEditingRaca(raca);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingRaca(null);
  };

  // "status" do filtro: ativos, inativos (próprias) ou ocultas (do sistema)
  const handleFiltroStatus = (status) => {
    lista.setFiltros({
      ativo: status === "ativos" ? true : status === "inativos" ? false : undefined,
      oculta: status === "ocultas" ? true : status === "visiveis" ? false : undefined,
    });
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      let resultado;
      if (editingRaca) {
        resultado = await racasDaClinicaAPI.editar({ ...editingRaca, ...values });
      } else {
        resultado = await racasDaClinicaAPI.cadastrar(values);
      }

      message.success(resultado?.message || "Raça salva com sucesso!");
      handleCloseModal();
      await lista.recarregar();
    } catch (error) {
      message.error(error.message || "Falha ao salvar a raça.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    racas: lista.dados,
    loading: lista.loading,
    paginacao: lista.paginacao,
    filtros: lista.filtros,
    loadingItemId,
    isModalOpen,
    isSubmitting,
    editingRaca,
    setBusca: lista.setBusca,
    setFiltros: lista.setFiltros,
    setPagina: (pagina, tamanhoPagina) => lista.handleTableChange({ current: pagina, pageSize: tamanhoPagina }, {}, {}),
    handleFiltroStatus,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
    handleToggleOculta,
  };
};
