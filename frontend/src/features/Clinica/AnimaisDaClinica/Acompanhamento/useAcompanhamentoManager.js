import { useState, useEffect, useCallback } from "react";
import { useParams } from "react-router-dom";
import { acompanhamentoAPI } from "../../../../api/clinica/acompanhamentoAPI";
import { racoesDaClinicaAPI } from "../../../../api/clinica/racoesDaClinicaAPI";
import { avaliacoesDaClinicaAPI } from "../../../../api/clinica/avaliacoesDaClinicaAPI";
import { consultasDaClinicaAPI } from "../../../../api/clinica/consultasDaClinicaAPI";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";
import { message } from "antd";

const formatError = (error, defaultMessage) => {
  if (error?.errors?.length) return `${error.message}: ${error.errors.join(", ")}`;
  return error?.message || defaultMessage;
};

export const useAcompanhamentoManager = () => {
  const { acompanhamentoId } = useParams();
  const [historico, setHistorico] = useState(null);
  const [loading, setLoading] = useState(true);
  const [racoes, setRacoes] = useState([]);

  // Estado para Avaliações
  const [isAvaliacaoModalOpen, setIsAvaliacaoModalOpen] = useState(false);
  const [selectedConsulta, setSelectedConsulta] = useState(null);
  const [avaliacaoData, setAvaliacaoData] = useState(null);
  const [loadingAvaliacao, setLoadingAvaliacao] = useState(false);

  // Estado para o modal de CRIAÇÃO de dieta
  const [isModalCriarDietaOpen, setIsModalCriarDietaOpen] = useState(false);
  const [avaliacaoSelecionado, setAvaliacaoSelecionado] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Estado para o modal de EDIÇÃO de dieta
  const [isModalEditarDietaOpen, setIsModalEditarDietaOpen] = useState(false);
  const [dietaParaEditar, setDietaParaEditar] = useState(null);

  // Estado para o modal de VISUALIZAÇÃO de dieta
  const [isModalVisualizarDietaOpen, setIsModalVisualizarDietaOpen] =
    useState(false);
  const [dietaParaVisualizar, setDietaParaVisualizar] = useState(null);

  // Estado para o modal de EDIÇÃO de consulta
  const [isConsultaModalOpen, setIsConsultaModalOpen] = useState(false);
  const [consultaToEdit, setConsultaToEdit] = useState(null);

  // Estado para o modal de EDIÇÃO do animal
  const [isEditAnimalModalOpen, setIsEditAnimalModalOpen] = useState(false);

  const handleOpenEditAnimalModal = () => setIsEditAnimalModalOpen(true);
  const handleCloseEditAnimalModal = () => setIsEditAnimalModalOpen(false);

  const handleSubmitEditAnimal = async (values) => {
    try {
      await animaisDaClinicaAPI.editar({ ...values, id: historico.id });
      message.success("Animal atualizado com sucesso!");
      handleCloseEditAnimalModal();
      await fetchHistorico();
    } catch (error) {
      message.error(formatError(error, "Erro ao atualizar o animal."));
    }
  };

  // Usamos useCallback para evitar recriações desnecessárias da função
  const fetchHistorico = useCallback(async ({ silent = false } = {}) => {
    try {
      if (!silent) setLoading(true);
      const result =
        await acompanhamentoAPI.getHistoricoCompleto(acompanhamentoId);
      if (result.succeeded) {
        setHistorico(result.data);
        // Quando o histórico carregar, buscamos as rações para a espécie do animal
        if (!silent && result.data?.especie) {
          const racoesData = await racoesDaClinicaAPI.listar(
            result.data.especie === "Cao" ? 1 : 2,
          );
          setRacoes(racoesData);
        }
        return result.data;
      } else {
        throw new Error(result.message || "Histórico não encontrado.");
      }
    } catch (err) {
      message.error(`Erro ao buscar histórico: ${err.message}`);
      return null;
    } finally {
      if (!silent) setLoading(false);
    }
  }, [acompanhamentoId]);

  useEffect(() => {
    if (acompanhamentoId) {
      fetchHistorico();
    }
  }, [acompanhamentoId, fetchHistorico]);

  // --- LÓGICA DE AVALIAÇÕES ---
  const handleOpenAvaliacaoModal = useCallback((consulta) => {
    setSelectedConsulta(consulta);
    setIsAvaliacaoModalOpen(true);
    setLoadingAvaliacao(false); // Mantém o estado para não quebrar outros lugares
    
    // Agora puxamos as informações da avaliação diretamente do histórico do paciente
    if (consulta.avaliacao) {
      setAvaliacaoData(consulta.avaliacao);
    } else {
      setAvaliacaoData(null);
    }
  }, []);

  const handleCloseAvaliacaoModal = () => {
    setIsAvaliacaoModalOpen(false);
    setSelectedConsulta(null);
    setAvaliacaoData(null);
  };

  const handleSubmitAvaliacao = async (formValues) => {
    if (!selectedConsulta) return;
    const consultaId = selectedConsulta.consultaId || selectedConsulta.id;
    try {
      await avaliacoesDaClinicaAPI.upsert(consultaId, formValues);
      message.success("Avaliação salva com sucesso!");
      handleCloseAvaliacaoModal();
      await fetchHistorico(); // Recarrega os dados do acompanhamento
    } catch (error) {
      message.error(formatError(error, "Falha ao salvar a avaliação."));
      throw error;
    }
  };

  const handleDeleteAvaliacao = async (avaliacaoId) => {
    try {
      await avaliacoesDaClinicaAPI.deletar(avaliacaoId);
      message.success("Avaliação excluída com sucesso!");
      handleCloseAvaliacaoModal();
      await fetchHistorico();
    } catch (error) {
      message.error(formatError(error, "Falha ao excluir a avaliação."));
    }
  };


  // --- LÓGICA DE DIETAS ---
  const handleCloseModalCriarDieta = () => {
    setIsModalCriarDietaOpen(false);
    setAvaliacaoSelecionado(null);
  };

  const handleOpenModalCriarDieta = (avaliacao) => {
    setIsModalCriarDietaOpen(true);
    setAvaliacaoSelecionado(avaliacao);
  };

  const handleOpenModalVisualizarDieta = (dieta) => {
    setDietaParaVisualizar(dieta);
    setIsModalVisualizarDietaOpen(true);
  };

  const handleCloseModalVisualizarDieta = () => {
    setIsModalVisualizarDietaOpen(false);
    setDietaParaVisualizar(null);
  };

  const handleOpenModalEditarDieta = (dieta, avaliacao) => {
    setDietaParaEditar(dieta);
    setAvaliacaoSelecionado(avaliacao);
    setIsModalEditarDietaOpen(true);
  };

  const handleCloseModalEditarDieta = () => {
    setIsModalEditarDietaOpen(false);
    setDietaParaEditar(null);
    setAvaliacaoSelecionado(null);
  };

  const handleCriarDieta = async (values) => {
    if (!historico?.acompanhamento?.id || !avaliacaoSelecionado?.id) return;

    setIsSubmitting(true);
    try {
      const payloadDieta = {
        ...values,
        acompanhamentoId: historico.acompanhamento.id,
        avaliacaoId: avaliacaoSelecionado.id,
      };

      const result = await acompanhamentoAPI.criarDieta(payloadDieta);

      if (result.succeeded) {
        message.success("Dieta calculada e salva com sucesso!");
        handleCloseModalCriarDieta();
        await fetchHistorico();
      } else {
        message.error(result.message || "Falha ao criar dieta.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleEditarDieta = async (values) => {
    if (!dietaParaEditar?.id) return;

    setIsSubmitting(true);
    try {
      const result = await acompanhamentoAPI.editarDieta(
        dietaParaEditar.id,
        values,
      );
      if (result.succeeded) {
        message.success("Dieta atualizada com sucesso!");
        handleCloseModalEditarDieta();
        await fetchHistorico(); 
      } else {
        message.error(result.message || "Falha ao atualizar dieta.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleExcluirDieta = async (dietaId) => {
    try {
      const result = await acompanhamentoAPI.deletarDieta(dietaId);
      if (result.succeeded) {
        message.success(result.message || "Dieta excluída com sucesso!");
        await fetchHistorico(); 
      } else {
        message.error(result.message || "Falha ao excluir dieta.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado."));
    }
  };

  const handleIniciarDieta = async (dietaId) => {
    try {
      const result = await acompanhamentoAPI.iniciarDieta(dietaId);
      if (result.succeeded) {
        message.success("Dieta iniciada com sucesso!");
        await fetchHistorico();
      } else {
        message.error(result.message || "Falha ao iniciar dieta.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado."));
    }
  };

  const handleConcluirDieta = async (dietaId) => {
    try {
      const result = await acompanhamentoAPI.concluirDieta(dietaId);
      if (result.succeeded) {
        message.success("Dieta concluída com sucesso!");
        await fetchHistorico();
      } else {
        message.error(result.message || "Falha ao concluir dieta.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado."));
    }
  };

  const atualizarDietaVisualizada = (dados, dietaId) => {
    const dietaAtualizada = (dados?.consultas || [])
      .map((c) => c.avaliacao?.dieta)
      .find((d) => d?.id === dietaId);
    if (dietaAtualizada) {
      setDietaParaVisualizar(dietaAtualizada);
    }
  };

  const handleRegistrarPeso = async (dietaId, dadosPeso) => {
    try {
      const result = await acompanhamentoAPI.registrarPesoDieta(dietaId, dadosPeso);
      if (result.succeeded) {
        message.success("Peso registrado com sucesso!");
        const dados = await fetchHistorico({ silent: true });
        atualizarDietaVisualizada(dados, dietaId);
        return true;
      }
      message.error(result.message || "Falha ao registrar peso.");
      return false;
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado ao registrar o peso."));
      return false;
    }
  };

  const handleRemoverPeso = async (dietaId, registroId) => {
    try {
      const result = await acompanhamentoAPI.deletarPeso(dietaId, registroId);
      if (result.succeeded) {
        message.success("Registro de peso removido!");
        const dados = await fetchHistorico({ silent: true });
        atualizarDietaVisualizada(dados, dietaId);
      } else {
        message.error(result.message || "Falha ao remover registro.");
      }
    } catch (error) {
      message.error(formatError(error, "Ocorreu um erro inesperado ao remover o registro."));
    }
  };

  const handleDeleteConsulta = async (consultaId) => {
    try {
      await consultasDaClinicaAPI.deletar(consultaId);
      message.success("Consulta excluída com sucesso!");
      await fetchHistorico();
    } catch (error) {
      message.error(formatError(error, "Falha ao excluir a consulta."));
    }
  };

  const handleEditConsulta = (consulta) => {
    setConsultaToEdit(consulta);
    setIsConsultaModalOpen(true);
  };

  const handleCloseConsultaModal = () => {
    setIsConsultaModalOpen(false);
    setConsultaToEdit(null);
  };

  const handleSubmitConsulta = async (values) => {
    try {
      if (consultaToEdit) {
        await consultasDaClinicaAPI.atualizar(consultaToEdit.id, values);
        message.success("Consulta atualizada com sucesso!");
      }
      handleCloseConsultaModal();
      await fetchHistorico();
    } catch (error) {
      message.error(formatError(error, "Falha ao salvar consulta."));
    }
  };

  return {
    historico,
    loading,
    racoes,
    isModalCriarDietaOpen,
    isModalVisualizarDietaOpen,
    isModalEditarDietaOpen,
    avaliacaoSelecionado,
    dietaParaVisualizar,
    dietaParaEditar,
    isSubmitting,
    isAvaliacaoModalOpen,
    selectedConsulta,
    avaliacaoData,
    loadingAvaliacao,
    isConsultaModalOpen,
    consultaToEdit,
    handleOpenAvaliacaoModal,
    handleCloseAvaliacaoModal,
    handleSubmitAvaliacao,
    handleDeleteAvaliacao,
    handleOpenModalCriarDieta,
    handleCloseModalCriarDieta,
    handleOpenModalVisualizarDieta,
    handleCloseModalVisualizarDieta,
    handleOpenModalEditarDieta,
    handleCloseModalEditarDieta,
    handleCriarDieta,
    handleEditarDieta,
    handleExcluirDieta,
    handleIniciarDieta,
    handleConcluirDieta,
    handleRegistrarPeso,
    handleRemoverPeso,
    handleDeleteConsulta,
    handleEditConsulta,
    handleCloseConsultaModal,
    handleSubmitConsulta,
    isEditAnimalModalOpen,
    handleOpenEditAnimalModal,
    handleCloseEditAnimalModal,
    handleSubmitEditAnimal,
  };
};
