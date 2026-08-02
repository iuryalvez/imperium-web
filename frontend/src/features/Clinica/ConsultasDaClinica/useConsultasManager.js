import { useState, useCallback, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { avaliacoesDaClinicaAPI } from "../../../api/clinica/avaliacoesDaClinicaAPI";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";
import { acompanhamentoAPI } from "../../../api/clinica/acompanhamentoAPI";

export const useConsultasManager = () => {
  const navigate = useNavigate();
  const [consultas, setConsultas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingConsulta, setEditingConsulta] = useState(null);

  const [isAvaliacaoModalOpen, setIsAvaliacaoModalOpen] = useState(false);
  const [selectedConsulta, setSelectedConsulta] = useState(null);
  const [avaliacaoData, setAvaliacaoData] = useState(null);
  const [loadingAvaliacao, setLoadingAvaliacao] = useState(false);

  const [isNextActionModalOpen, setIsNextActionModalOpen] = useState(false);
  const [dadosConsulta, setDadosConsulta] = useState(null);
  const [isModalDietaOpen, setIsModalDietaOpen] = useState(false);
  const [racoesDisponiveis, setRacoesDisponiveis] = useState([]);
  const [isSubmittingDieta, setIsSubmittingDieta] = useState(false);

  // Estado para controlar a dieta em edição
  const [dietaEmEdicao, setDietaEmEdicao] = useState(null);

  const [isModalVisualizarDietaOpen, setIsModalVisualizarDietaOpen] =
    useState(false);
  const [dietaParaVisualizar, setDietaParaVisualizar] = useState(null);

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
    const payload = { ...formValues };
    if (!payload.isGestante) payload.semanaGestacao = null;
    if (!payload.emLactacao) {
      payload.isLactanteNumeroFilhotes = 0;
      payload.semanaLactacao = null;
    }
    delete payload.emLactacao;

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

  const handleOpenAvaliacaoModal = useCallback(async (consulta) => {
    setSelectedConsulta(consulta);
    setIsAvaliacaoModalOpen(true);
    setLoadingAvaliacao(true);
    if (consulta.avaliacaoPreenchida) {
      try {
        const response = await avaliacoesDaClinicaAPI.getByConsultaId(
          consulta.id,
        );
        setAvaliacaoData(response.data);
      } catch (error) {
        setAvaliacaoData(null);
      } finally {
        setLoadingAvaliacao(false);
      }
    } else {
      setAvaliacaoData(null);
      setLoadingAvaliacao(false);
    }
  }, []);

  const handleCloseAvaliacaoModal = () => {
    setIsAvaliacaoModalOpen(false);
    setSelectedConsulta(null);
    setAvaliacaoData(null);
  };

  const handleSubmitAvaliacao = async (formValues) => {
    if (!selectedConsulta) return;
    try {
      const response = await avaliacoesDaClinicaAPI.upsert(
        selectedConsulta.id,
        formValues,
      );
      message.success("Avaliação salva com sucesso!");

      setDadosConsulta({
        consultaId: selectedConsulta.id, // Adicionado para a transição futura
        avaliacaoId: response.data?.id || response.data?.avaliacaoId,
        acompanhamentoId: selectedConsulta.acompanhamentoId,
        animalId: selectedConsulta.animalId,
        nomeAnimal: selectedConsulta.nomeAnimal,
        especieAnimal: selectedConsulta.especieAnimal,
        peso: formValues.peso,
        racaoId: formValues.racaoId,
        tipoDieta: formValues.tipoDieta,
        condicaoCorporal: formValues.condicaoCorporal,
        nomeRaca: selectedConsulta.nomeRaca || "",
        pesoMinimoRaca: selectedConsulta.pesoMinimoRaca || "",
        pesoMaximoRaca: selectedConsulta.pesoMaximoRaca || "",
      });

      const especie =
        selectedConsulta.especieAnimal === "Gato" ||
        selectedConsulta.especieAnimal === 2
          ? 2
          : 1;
      const racoes = await racoesDaClinicaAPI.listar(especie);
      setRacoesDisponiveis(racoes);

      handleCloseAvaliacaoModal();
      await fetchConsultas();
      setIsNextActionModalOpen(true);
    } catch (error) {
      message.error("Falha ao salvar o relatório.");
      throw error;
    }
  };

  const handleNextActionClose = () => {
    setIsNextActionModalOpen(false);
    setDadosConsulta(null);
  };

  const handleIrParaAcompanhamento = (animalId) => {
    handleNextActionClose();
    navigate(`/clinica/animais/acompanhamento/${animalId}`);
  };

  const handleDeleteAvaliacao = async (avaliacaoId) => {
    if (!selectedConsulta) return;
    try {
      await avaliacoesDaClinicaAPI.deletar(avaliacaoId);
      message.success("Avaliação excluída com sucesso!");
      handleCloseAvaliacaoModal();
      await fetchConsultas();
    } catch (error) {
      message.error("Falha ao excluir a avaliação.");
    }
  };

  const handleCalcularDietaPosAvaliacao = () => {
    setIsNextActionModalOpen(false);
    setIsModalDietaOpen(true);
  };

  const handleCloseModalDieta = () => {
    setIsModalDietaOpen(false);
    setDadosConsulta(null);
    setDietaEmEdicao(null);
  };

  const handleSubmitDieta = async (formValues) => {
    setIsSubmittingDieta(true);
    try {
      let result;
      if (dietaEmEdicao) {
        // Atualiza a dieta existente
        result = await acompanhamentoAPI.editarDieta(
          dietaEmEdicao.id,
          formValues,
        );
      } else {
        // Cria uma nova dieta
        const payloadDieta = {
          ...formValues,
          acompanhamentoId: dadosConsulta.acompanhamentoId,
          avaliacaoId: dadosConsulta.avaliacaoId,
        };
        result = await acompanhamentoAPI.criarDieta(payloadDieta);
      }

      if (result.succeeded) {
        message.success(
          dietaEmEdicao
            ? "Dieta atualizada com sucesso!"
            : "Dieta calculada e salva com sucesso!",
        );
        handleCloseModalDieta();
        await fetchConsultas();

        const consultaIdOrigem =
          dadosConsulta?.consultaId || dietaEmEdicao?.consultaOrigem?.id;
        if (consultaIdOrigem) {
          const resp =
            await avaliacoesDaClinicaAPI.getByConsultaId(consultaIdOrigem);
          const dietaAtualizada = resp.data?.dieta;
          if (dietaAtualizada) {
            // Injeta a consulta novamente para não perdermos as referências
            setDietaParaVisualizar({
              ...dietaAtualizada,
              consultaOrigem:
                dadosConsulta?.consultaOrigem || dietaEmEdicao?.consultaOrigem,
            });
            setIsModalVisualizarDietaOpen(true);
          }
        }
      } else {
        message.error(result.message || "Falha ao salvar dieta.");
      }
    } catch (error) {
      message.error(error.message || "Ocorreu um erro inesperado.");
    } finally {
      setIsSubmittingDieta(false);
    }
  };

  const handleCalcularDietaDaTabela = async (consulta) => {
    try {
      const response = await avaliacoesDaClinicaAPI.getByConsultaId(
        consulta.id,
      );
      const avaliacao = response.data;

      setDadosConsulta({
        consultaId: consulta.id, // Injetado para o UX contínuo
        consultaOrigem: consulta, // Guardamos o objeto inteiro para ações futuras
        avaliacaoId: avaliacao.id,
        acompanhamentoId: consulta.acompanhamentoId,
        animalId: consulta.animalId,
        nomeAnimal: consulta.nomeAnimal,
        especieAnimal: consulta.especieAnimal,
        peso: avaliacao.peso,
        racaoId: avaliacao.racaoId,
        tipoDieta: avaliacao.tipoDieta,
        condicaoCorporal: avaliacao.condicaoCorporal,
        nomeRaca: consulta.nomeRaca,
        pesoMinimoRaca: consulta.pesoMinimoRaca,
        pesoMaximoRaca: consulta.pesoMaximoRaca,
      });

      const especie =
        consulta.especieAnimal === "Gato" || consulta.especieAnimal === 2
          ? 2
          : 1;
      const racoes = await racoesDaClinicaAPI.listar(especie);
      setRacoesDisponiveis(racoes);

      setDietaEmEdicao(null);
      setIsModalDietaOpen(true);
    } catch (error) {
      message.error("Não foi possível carregar os dados base para a dieta.");
    }
  };

  const handleVerDietaDaTabela = async (consulta) => {
    try {
      const response = await avaliacoesDaClinicaAPI.getByConsultaId(
        consulta.id,
      );
      const avaliacao = response.data;
      const dieta = avaliacao.dieta || avaliacao.dietaPersonalizada;

      if (dieta) {
        // INJETANDO a consulta no objeto da dieta para podermos Editar a partir do footer
        setDietaParaVisualizar({ ...dieta, consultaOrigem: consulta });
        setIsModalVisualizarDietaOpen(true);
      } else {
        message.warning("Dados da dieta não encontrados neste relatório.");
      }
    } catch (error) {
      message.error("Erro ao carregar os dados da dieta.");
    }
  };

  const handleCloseModalVisualizarDieta = () => {
    setIsModalVisualizarDietaOpen(false);
    setDietaParaVisualizar(null);
  };

  // Abre a edição aproveitando a inteligência do cálculo da tabela
  const handleEditarDietaDaTabela = async (consulta) => {
    try {
      const response = await avaliacoesDaClinicaAPI.getByConsultaId(
        consulta.id,
      );
      const avaliacao = response.data;
      const dieta = avaliacao.dieta || avaliacao.dietaPersonalizada;

      if (!dieta) return message.warning("Dieta não encontrada.");

      // Popula os dados base
      setDadosConsulta({
        consultaId: consulta.id,
        consultaOrigem: consulta,
        avaliacaoId: avaliacao.id,
        acompanhamentoId: consulta.acompanhamentoId,
        animalId: consulta.animalId,
        nomeAnimal: consulta.nomeAnimal,
        especieAnimal: consulta.especieAnimal,
        peso: avaliacao.peso,
        condicaoCorporal: avaliacao.condicaoCorporal,
        nomeRaca: consulta.nomeRaca,
        pesoMinimoRaca: consulta.pesoMinimoRaca,
        pesoMaximoRaca: consulta.pesoMaximoRaca,
      });

      const especie =
        consulta.especieAnimal === "Gato" || consulta.especieAnimal === 2
          ? 2
          : 1;
      const racoes = await racoesDaClinicaAPI.listar(especie);
      setRacoesDisponiveis(racoes);

      // Define que estamos em modo de edição
      setDietaEmEdicao({ ...dieta, consultaOrigem: consulta });
      setIsModalVisualizarDietaOpen(false); // Fecha visualização se estiver aberta
      setIsModalDietaOpen(true);
    } catch (error) {
      message.error("Erro ao preparar a edição da dieta.");
    }
  };

  // Exclusão global da dieta
  const handleExcluirDieta = async (dietaId) => {
    try {
      await acompanhamentoAPI.deletarDieta(dietaId);
      message.success("Dieta excluída com sucesso!");
      handleCloseModalVisualizarDieta(); // Fecha se estiver visualizando
      handleCloseModalDieta(); // Fecha se estiver editando
      await fetchConsultas();
    } catch (error) {
      message.error("Erro ao excluir a dieta.");
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
    isAvaliacaoModalOpen,
    selectedConsulta,
    avaliacaoData,
    loadingAvaliacao,
    handleOpenAvaliacaoModal,
    handleCloseAvaliacaoModal,
    handleSubmitAvaliacao,
    handleDeleteAvaliacao,
    isNextActionModalOpen,
    dadosConsulta,
    dietaEmEdicao,
    handleNextActionClose,
    handleIrParaAcompanhamento,
    isModalDietaOpen,
    racoesDisponiveis,
    isSubmittingDieta,
    handleCalcularDietaPosAvaliacao,
    handleCloseModalDieta,
    handleSubmitDieta,
    handleCalcularDietaDaTabela,
    handleEditarDietaDaTabela,
    handleExcluirDieta,
    isModalVisualizarDietaOpen,
    dietaParaVisualizar,
    handleVerDietaDaTabela,
    handleCloseModalVisualizarDieta,
  };
};
