import { useState, useCallback, useEffect } from "react";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../api/clinica/consultasDaClinicaAPI";
import { relatoriosDaClinicaAPI } from "../../api/clinica/relatoriosDaClinicaAPI";

export const useConsultasManager = () => {
  const [consultas, setConsultas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingConsulta, setEditingConsulta] = useState(null);

  const [isRelatorioModalOpen, setIsRelatorioModalOpen] = useState(false);
  const [selectedConsulta, setSelectedConsulta] = useState(null); // Consulta dona do relatório
  const [relatorioData, setRelatorioData] = useState(null); // Dados do relatório a ser editado
  const [loadingRelatorio, setLoadingRelatorio] = useState(false);

  const fetchConsultas = useCallback(async () => {
    setLoading(true);
    try {
      const data = await consultasDaClinicaAPI.listar();
      setConsultas(data);
    } catch (error) {
      message.error("Erro ao carregar consultas.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchConsultas();
  }, [fetchConsultas]);

  const handleOpenModal = (consulta = null) => {
    setEditingConsulta(consulta);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingConsulta(null);
  };

  const handleSubmit = async (values) => {
    try {
      if (editingConsulta) {
        await consultasDaClinicaAPI.editar(editingConsulta.id, values);
        message.success("Consulta atualizada com sucesso!");
      } else {
        await consultasDaClinicaAPI.cadastrar(values);
        message.success("Consulta agendada com sucesso!");
      }
      handleCloseModal();
      fetchConsultas();
    } catch (error) {
      message.error(error.message || "Falha ao salvar consulta.");
      throw error; // Re-lança o erro para o formulário poder manter o modal aberto
    }
  };

  const handleDelete = async (consultaId) => {
    try {
      await consultasDaClinicaAPI.deletar(consultaId);
      message.success("Consulta excluída com sucesso!");
      fetchConsultas();
    } catch (error) {
      message.error(
        error.response?.data?.message || "Erro ao excluir consulta."
      );
    }
  };

  const handleOpenRelatorioModal = useCallback(async (consulta) => {
    setSelectedConsulta(consulta);
    setIsRelatorioModalOpen(true);
    setLoadingRelatorio(true);
    try {
      const data = await relatoriosDaClinicaAPI.getByConsultaId(consulta.id);
      setRelatorioData(data); 
    } catch (error) {
      message.error("Erro ao carregar dados do relatório.");
    } finally {
      setLoadingRelatorio(false);
    }
  }, []);

  const handleCloseRelatorioModal = () => {
    setIsRelatorioModalOpen(false);
    setSelectedConsulta(null);
    setRelatorioData(null);
  };

  const handleSubmitRelatorio = async (values) => {
    if (!selectedConsulta) return;
    try {
      await relatoriosDaClinicaAPI.upsert(selectedConsulta.id, values);
      message.success("Relatório salvo com sucesso!");
      handleCloseRelatorioModal();
      fetchConsultas(); // Re-busca as consultas para atualizar o status do relatório na tabela
    } catch (error) {
      message.error("Falha ao salvar o relatório.");
      throw error;
    }
  };

  return {
    consultas,
    loading,
    isModalOpen,
    editingConsulta,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    isRelatorioModalOpen,
    selectedConsulta,
    relatorioData,
    loadingRelatorio,
    handleOpenRelatorioModal,
    handleCloseRelatorioModal,
    handleSubmitRelatorio,
  };
};
