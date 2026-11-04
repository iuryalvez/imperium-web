import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosConsulta } from "./components/colunas";

export const useConsultasManager = () => {
  const navigate = useNavigate();
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(consultasDaClinicaAPI.listar, {}, traduzirFiltrosConsulta);
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

  // A rota do acompanhamento usa o id do ACOMPANHAMENTO (não do animal)
  const handleIrParaAcompanhamento = (acompanhamentoId) => {
    if (acompanhamentoId) navigate(`/clinica/animais/acompanhamento/${acompanhamentoId}`);
  };

  // Cancelar (só pendente, motivo opcional) e reabrir: GESTOR, VETERINARIO e SECRETARIO
  const handleCancelar = async (consulta, motivo) => {
    try {
      const resultado = await consultasDaClinicaAPI.cancelar(consulta.id, motivo || null);
      message.success(resultado?.message || "Consulta cancelada com sucesso!");
      await fetchConsultas();
    } catch (error) {
      message.error(error.message || "Erro ao cancelar a consulta.");
    }
  };

  const handleReabrir = async (consulta) => {
    try {
      const resultado = await consultasDaClinicaAPI.reabrir(consulta.id);
      message.success(resultado?.message || "Consulta reaberta com sucesso!");
      await fetchConsultas();
    } catch (error) {
      message.error(error.message || "Erro ao reabrir a consulta.");
    }
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
    handleCancelar,
    handleReabrir,
  };
};
