import { useState, useCallback, useEffect } from "react";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { relatoriosDaClinicaAPI } from "../../../api/clinica/relatoriosDaClinicaAPI";

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

  const handleSubmit = async (formValues) => {
    // 1. Create a mutable copy of the form formValues to prepare for submission.
    //    Criamos uma cópia dos valores para poder modificá-los sem afetar o estado original.
    const payload = { ...formValues };

    // 2. Clean up data based on conditional logic.
    //    Executamos a lógica de limpeza para garantir a consistência dos dados.
    if (!payload.isGestante) {
      payload.semanaGestacao = null; // ou undefined, dependendo do que sua API espera
    }

    // Assumindo que o switch de lactação se chama 'emLactacao' no formulário
    if (!payload.emLactacao) {
      payload.isLactanteNumeroFilhotes = 0;
      payload.semanaLactacao = null; // ou undefined
    }

    // O campo 'emLactacao' é apenas para controle de UI, podemos removê-lo do payload final.
    delete payload.emLactacao;

    // 3. Perform the upsert (update or insert) operation.
    //    Realizamos a operação de cadastro ou edição.
    try {
      if (editingConsulta) {
        await consultasDaClinicaAPI.editar(editingConsulta.id, formValues);
        message.success("Consulta atualizada com sucesso!");
      } else {
        await consultasDaClinicaAPI.cadastrar(formValues);
        message.success("Consulta agendada com sucesso!");
      }
      handleCloseModal();
      await fetchConsultas();
    } catch (error) {
      message.error(error.message || "Falha ao salvar consulta.");
      throw error; // Re-lança o erro para o formulário poder manter o modal aberto
    }
  };

  const handleDelete = async (consultaId) => {
    try {
      await consultasDaClinicaAPI.deletar(consultaId);
      message.success("Consulta excluída com sucesso!");
      await fetchConsultas();
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
    if (consulta.relatorioPreenchido) {
      try {
        const response = await relatoriosDaClinicaAPI.getByConsultaId(
          consulta.id
        );
        setRelatorioData(response.data);
      } catch (error) {
        // message.error("Erro ao carregar dados do relatório.");
        setRelatorioData(null); // Se não houver relatório, iniciamos com null para um novo
      } finally {
        setLoadingRelatorio(false);
      }
    } else {
      setRelatorioData(null); // Novo relatório
      setLoadingRelatorio(false);
    }
  }, []);

  const handleCloseRelatorioModal = () => {
    setIsRelatorioModalOpen(false);
    setSelectedConsulta(null);
    setRelatorioData(null);
  };

  const handleSubmitRelatorio = async (formValues) => {
    if (!selectedConsulta) return;
    try {
      await relatoriosDaClinicaAPI.upsert(selectedConsulta.id, formValues);
      message.success("Relatório salvo com sucesso!");
      handleCloseRelatorioModal();
      await fetchConsultas(); // Re-busca as consultas para atualizar o status do relatório na tabela
    } catch (error) {
      message.error("Falha ao salvar o relatório.");
      throw error;
    }
  };

  const handleDeleteRelatorio = async (relatorioId) => {
    if (!selectedConsulta) return;
    try {
      await relatoriosDaClinicaAPI.deletar(relatorioId);
      message.success("Relatório excluído com sucesso!");
      handleCloseRelatorioModal();
      await fetchConsultas(); // Re-busca as consultas para atualizar o status do relatório na tabela
    } catch (error) {
      message.error("Falha ao excluir o relatório.");
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
    handleDeleteRelatorio
  };
};
