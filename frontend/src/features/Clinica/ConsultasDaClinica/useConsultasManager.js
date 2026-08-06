import { useState, useCallback, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";

export const useConsultasManager = () => {
  const navigate = useNavigate();
  const [consultas, setConsultas] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  // Filtros removidos a pedido do usuário
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingConsulta, setEditingConsulta] = useState(null);

  const fetchConsultas = useCallback(async () => {
    setLoading(true);
    try {
      const params = { page, pageSize };
      const response = await consultasDaClinicaAPI.listar(params);
      setConsultas(response.data || []);
      setTotal(response.totalCount || 0);
    } catch (error) {
      message.error("Erro ao carregar consultas.");
      setConsultas([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchConsultas();
  }, [fetchConsultas]);

  const handleTableChange = (pagination) => {
    setPage(pagination.current);
    setPageSize(pagination.pageSize);
  };



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
    consultas,
    total,
    page,
    pageSize,
    loading,
    isModalOpen,
    editingConsulta,
    handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleIrParaAcompanhamento,
  };
};
