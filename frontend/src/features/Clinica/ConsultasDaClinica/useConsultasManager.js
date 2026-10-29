import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";

export const useConsultasManager = () => {
  const navigate = useNavigate();
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(consultasDaClinicaAPI.listar, {});
  const consultas = lista.dados;
  const loading = lista.loading;
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingConsulta, setEditingConsulta] = useState(null);

  const fetchConsultas = lista.recarregar;

  const handleOpenModal = (consulta = null) => {
    setEditingConsulta(consulta);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingConsulta(null);
  };

  const handleSubmit = async (formValues) => {
    const payload = { ...formValues };
    if (!payload.isGestante) payload.semanaGestacao = null;
    if (!payload.emLactacao) {
      payload.isLactanteNumeroFilhotes = 0;
      payload.semanaLactacao = null;
    }
    delete payload.emLactacao;

    try {
      if (editingConsulta) {
        await consultasDaClinicaAPI.editar(editingConsulta.id, payload);
        message.success("Consulta atualizada com sucesso!");
      } else {
        await consultasDaClinicaAPI.cadastrar(payload);
        message.success("Consulta agendada com sucesso!");
      }
      handleCloseModal();
      await fetchConsultas();
    } catch (error) {
      message.error(error.message || "Falha ao salvar consulta.");
      throw error;
    }
  };

  const handleDelete = async (consultaId) => {
    try {
      await consultasDaClinicaAPI.deletar(consultaId);
      message.success("Consulta excluída com sucesso!");
      await fetchConsultas();
    } catch (error) {
      message.error(
        error.response?.data?.message || "Erro ao excluir consulta.",
      );
    }
  };

  const handleIrParaAcompanhamento = (animalId) => {
    navigate(`/clinica/animais/acompanhamento/${animalId}`);
  };

  return {
    paginacao: lista.paginacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    consultas,
    loading,
    isModalOpen,
    editingConsulta,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleIrParaAcompanhamento,
  };
};
