import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI.js";
import { sanitizeToNumbers, sanitizePayload } from "../../../utils/sanitizers.js";

export const useTutoresDaClinicaManager = () => {
  const [tutores, setTutores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingTutor, setEditingTutor] = useState(null);
  const [isOpenModalDetalhes, setIsOpenModalDetalhes] = useState(false);
  const [viewingTutor, setViewingTutor] = useState(null);

  const fetchTutores = useCallback(async () => {
    setLoading(true);
    try {
      const dados = await tutoresDaClinicaAPI.listar();
      setTutores(dados);
    } catch (error) {
      message.error(error.message || "Erro ao carregar tutores.");
      setTutores([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTutores();
  }, [fetchTutores]);

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

  const handleOpenModalDetalhes = (tutor) => {
    setViewingTutor(tutor);
    setIsOpenModalDetalhes(true);
  };

  const handleCloseModalDetalhes = () => {
    setIsOpenModalDetalhes(false);
    setTimeout(() => setViewingTutor(null), 300);
  };

  return {
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
    handleOpenModalDetalhes,
  };
};