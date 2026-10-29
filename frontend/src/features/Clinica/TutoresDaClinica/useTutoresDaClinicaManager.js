import { useState } from "react";
import { message } from "antd";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI.js";
import { sanitizeToNumbers, sanitizePayload } from "../../../utils/sanitizers.js";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosTutor } from "./components/TabelaTutoresDaClinica/colunas";

export const useTutoresDaClinicaManager = () => {
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(tutoresDaClinicaAPI.listar, {}, traduzirFiltrosTutor);
  const tutores = lista.dados;
  const loading = lista.loading;
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingTutor, setEditingTutor] = useState(null);
  const [isOpenModalDetalhes, setIsOpenModalDetalhes] = useState(false);
  const [viewingTutor, setViewingTutor] = useState(null);

  const fetchTutores = lista.recarregar;

  const handleOpenModalCadastro = () => {
    setEditingTutor(null);
    setIsModalOpen(true);
  };

  const handleEditarTutor = (tutor) => {
    setEditingTutor(tutor);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingTutor(null);
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      // Clean mask formatting before sending to API
      const payload = sanitizePayload({
        ...values,
        cpf: sanitizeToNumbers(values.cpf),
        telefone: sanitizeToNumbers(values.telefone),
        whatsapp: sanitizeToNumbers(values.whatsapp),
        cep: sanitizeToNumbers(values.cep),
      });

      if (editingTutor) {
        await tutoresDaClinicaAPI.editar({ ...editingTutor, ...payload });
        message.success("Tutor atualizado com sucesso!");
      } else {
        await tutoresDaClinicaAPI.cadastrar(payload);
        message.success("Tutor cadastrado com sucesso!");
      }
      
      handleCloseModal();
      await fetchTutores();
    } catch (error) {
      message.error(error.message || "Erro ao salvar tutor.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeletarTutor = async (tutor) => {
    setLoadingRowKey(tutor.id);
    try {
      await tutoresDaClinicaAPI.deletar(tutor.id);
      message.success("Tutor removido com sucesso!");
      await fetchTutores();
    } catch (error) {
      message.error(error.message || "Erro ao remover tutor.");
    } finally {
      setLoadingRowKey(null);
    }
  };

  // Ativa/desativa o tutor. Inativo continua no histórico, mas não entra em vínculos nem consultas novas.
  const handleToggleStatus = async (tutor) => {
    setLoadingRowKey(tutor.id);
    try {
      const novoStatus = !tutor.isAtivo;
      await tutoresDaClinicaAPI.setStatus(tutor.id, novoStatus);
      message.success(`Tutor ${novoStatus ? "ativado" : "desativado"} com sucesso!`);
      await fetchTutores();
    } catch (error) {
      message.error(error.message || "Erro ao alterar o status do tutor.");
    } finally {
      setLoadingRowKey(null);
    }
  };

  const handleOpenModalDetalhes = (tutor) => {
    setViewingTutor(tutor);
    setIsOpenModalDetalhes(true);
  };

  const handleCloseModalDetalhes = () => {
    setIsOpenModalDetalhes(false);
    setTimeout(() => setViewingTutor(null), 300);
  };

  return {
    paginacao: lista.paginacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    tutores,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingTutor,
    viewingTutor,
    handleOpenModalCadastro,
    handleCloseModal,
    handleCloseModalDetalhes,
    handleSubmit,
    handleEditarTutor,
    handleDeletarTutor,
    handleToggleStatus,
    handleOpenModalDetalhes,
  };
};