import { useState, useCallback, useEffect } from "react";
import { message } from "antd";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";

export const useRacoesManager = () => {
  const [racoes, setRacoes] = useState([]);
  const [loading, setLoading] = useState(true);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRacao, setEditingRacao] = useState(null);

  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [viewingRacao, setViewingRacao] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchRacoes = useCallback(async () => {
    setLoading(true);
    try {
      const dadosRacoes = await racoesDaClinicaAPI.listar();
      setRacoes(dadosRacoes);
    } catch (error) {
      message.error("Erro ao carregar rações.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchRacoes();
  }, [fetchRacoes]);

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
      if (editingRacao) {
        await racoesDaClinicaAPI.editar({ ...editingRacao, ...values });
        message.success("Ração atualizada com sucesso!");
      } else {
        await racoesDaClinicaAPI.cadastrar(values);
        message.success("Ração cadastrada com sucesso!");
      }
      handleCloseModal();
      fetchRacoes();
    } catch (error) {
      message.error(error.message || "Falha na operação.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (racaoId) => {
    try {
      await racoesDaClinicaAPI.deletar(racaoId);
      message.success("Ração excluída com sucesso!");
      fetchRacoes();
    } catch (error) {
      message.error("Erro ao excluir ração.");
    }
  };

  return {
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
  };
};
