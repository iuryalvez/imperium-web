import { useState, useEffect, useCallback } from "react";
import { useParams } from "react-router-dom";
import { acompanhamentoAPI } from "../../../../api/clinica/acompanhamentoAPI";
import { racoesDaClinicaAPI } from "../../../../api/clinica/racoesDaClinicaAPI";
import { message } from "antd";

export const useAcompanhamentoAnimalManager = () => {
  const { acompanhamentoId } = useParams(); // <-- Pega o ID correto da URL
  const [historico, setHistorico] = useState(null);
  const [loading, setLoading] = useState(true);
  const [racoes, setRacoes] = useState([]);

  // Estado para o modal de CRIAÇÃO de dieta
  const [isModalCriarDietaOpen, setIsModalCriarDietaOpen] = useState(false);
  const [relatorioSelecionado, setRelatorioSelecionado] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Estado para o modal de EDIÇÃO de dieta
  const [isModalEditarDietaOpen, setIsModalEditarDietaOpen] = useState(false);
  const [dietaParaEditar, setDietaParaEditar] = useState(null);

  // Estado para o modal de VISUALIZAÇÃO de dieta
  const [isModalVisualizarDietaOpen, setIsModalVisualizarDietaOpen] =
    useState(false);
  const [dietaParaVisualizar, setDietaParaVisualizar] = useState(null);

  // Usamos useCallback para evitar recriações desnecessárias da função
  const fetchHistorico = useCallback(async () => {
    try {
      setLoading(true);
      const result = await acompanhamentoAPI.getHistoricoCompleto(
        acompanhamentoId
      );
      if (result.succeeded) {
        setHistorico(result.data);
        // Quando o histórico carregar, buscamos as rações para a espécie do animal
        if (result.data?.especie) {
          const racoesData = await racoesDaClinicaAPI.listar(
            result.data.especie === "Cao" ? 1 : 2
          );
          setRacoes(racoesData);
        }
      } else {
        throw new Error(result.message || "Histórico não encontrado.");
      }
    } catch (err) {
      message.error(`Erro ao buscar histórico: ${err.message}`);
    } finally {
      setLoading(false);
    }
  }, [acompanhamentoId]);

  useEffect(() => {
    if (acompanhamentoId) {
      fetchHistorico();
    }
  }, [acompanhamentoId, fetchHistorico]);

  const handleCloseModalCriarDieta = () => {
    setIsModalCriarDietaOpen(false);
    setRelatorioSelecionado(null);
  };

  const handleOpenModalCriarDieta = async (relatorio) => {
    setIsModalCriarDietaOpen(true);
    setRelatorioSelecionado(relatorio);
  };

  const handleOpenModalVisualizarDieta = (dieta) => {
    setDietaParaVisualizar(dieta);
    setIsModalVisualizarDietaOpen(true);
  };

  const handleCloseModalVisualizarDieta = () => {
    setIsModalVisualizarDietaOpen(false);
    setDietaParaVisualizar(null);
  };

  const handleOpenModalEditarDieta = (dieta) => {
    setDietaParaEditar(dieta);
    setIsModalEditarDietaOpen(true);
  };

  const handleCloseModalEditarDieta = () => {
    setIsModalEditarDietaOpen(false);
    setDietaParaEditar(null);
  };

  const handleCriarDieta = async (values) => {
    if (!historico?.acompanhamento?.id) return;

    setIsSubmitting(true);
    try {
      const result = await acompanhamentoAPI.criarDieta(
        historico.acompanhamento.id,
        values
      );
      if (result.succeeded) {
        message.success("Dieta calculada e salva com sucesso!");
        handleCloseModalCriarDieta();
        await fetchHistorico(); // Recarrega os dados para mostrar a nova dieta na lista
      } else {
        message.error(result.message || "Falha ao criar dieta.");
      }
    } catch (error) {
      message.error(error.message || "Ocorreu um erro inesperado.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleEditarDieta = async (values) => {
    if (!dietaParaEditar?.id) return;

    setIsSubmitting(true);
    try {
      // Assumindo que você tem uma função 'dietaAPI.editar'
      const result = await acompanhamentoAPI.editarDieta(
        dietaParaEditar.id,
        values
      );
      if (result.succeeded) {
        message.success("Dieta atualizada com sucesso!");
        handleCloseModalEditarDieta();
        await fetchHistorico(); // Recarrega os dados
      } else {
        message.error(result.message || "Falha ao atualizar dieta.");
      }
    } catch (error) {
      message.error(error.message || "Ocorreu um erro inesperado.");
    } finally {
      setIsSubmitting(false);
    }
  };

  // NOVA: Função para EXCLUIR a dieta
  const handleExcluirDieta = async (dietaId) => {
    try {
      // Assumindo que você tem uma função 'dietaAPI.excluir'
      const result = await acompanhamentoAPI.deletarDieta(dietaId);
      if (result.succeeded) {
        message.success(result.message || "Dieta excluída com sucesso!");
        await fetchHistorico(); // Recarrega os dados
      } else {
        message.error(result.message || "Falha ao excluir dieta.");
      }
    } catch (error) {
      message.error(error.message || "Ocorreu um erro inesperado.");
    }
  };

  // Retornamos o hook e a função para recarregar
  return {
    historico,
    loading,
    racoes,
    isModalCriarDietaOpen,
    isModalVisualizarDietaOpen,
    isModalEditarDietaOpen,
    relatorioSelecionado,
    dietaParaVisualizar,
    dietaParaEditar,
    isSubmitting,
    handleOpenModalCriarDieta,
    handleCloseModalCriarDieta,
    handleOpenModalVisualizarDieta,
    handleCloseModalVisualizarDieta,
    handleOpenModalEditarDieta,
    handleCloseModalEditarDieta,
    handleCriarDieta,
    handleEditarDieta,
    handleExcluirDieta,
  };
};
