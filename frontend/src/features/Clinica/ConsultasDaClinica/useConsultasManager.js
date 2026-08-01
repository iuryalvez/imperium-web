import { useState, useCallback, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { relatoriosDaClinicaAPI } from "../../../api/clinica/relatoriosDaClinicaAPI";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";
import { acompanhamentoAPI } from "../../../api/clinica/acompanhamentoAPI";

export const useConsultasManager = () => {
  const navigate = useNavigate();
  const [consultas, setConsultas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingConsulta, setEditingConsulta] = useState(null);

  const [isRelatorioModalOpen, setIsRelatorioModalOpen] = useState(false);
  const [selectedConsulta, setSelectedConsulta] = useState(null); // Consulta dona do relatório
  const [relatorioData, setRelatorioData] = useState(null); // Dados do relatório a ser editado
  const [loadingRelatorio, setLoadingRelatorio] = useState(false);

  const [isNextActionModalOpen, setIsNextActionModalOpen] = useState(false);
  const [idsSalvos, setIdsSalvos] = useState(null); // { relatorioId, acompanhamentoId, animalId }
  const [isModalDietaOpen, setIsModalDietaOpen] = useState(false);
  const [racoesDisponiveis, setRacoesDisponiveis] = useState([]);
  const [isSubmittingDieta, setIsSubmittingDieta] = useState(false);

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
        error.response?.data?.message || "Erro ao excluir consulta.",
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
          consulta.id,
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
      const response = await relatoriosDaClinicaAPI.upsert(
        selectedConsulta.id,
        formValues,
      );
      message.success("Relatório salvo com sucesso!");

      // 1. Prepara os dados repassados para a Dieta (incluindo dados da Raça para a UI)
      setIdsSalvos({
        relatorioId: response.data?.id || response.data?.relatorioId,
        acompanhamentoId: selectedConsulta.acompanhamentoId,
        animalId: selectedConsulta.animalId,
        // Autopreenchimento:
        peso: formValues.peso,
        racaoId: formValues.racaoId,
        tipoDieta: formValues.tipoDieta,
        nomeRaca: selectedConsulta.nomeRaca || "",
        pesoMinimoRaca: selectedConsulta.pesoMinimoRaca || "",
        pesoMaximoRaca: selectedConsulta.pesoMaximoRaca || "",
      });

      // 2. Busca as rações da clínica de acordo com a espécie
      const especie =
        selectedConsulta.especieAnimal === "Gato" ||
        selectedConsulta.especieAnimal === 2
          ? 2
          : 1;
      const racoes = await racoesDaClinicaAPI.listar(especie);
      setRacoesDisponiveis(racoes);

      handleCloseRelatorioModal();
      await fetchConsultas();
      setIsNextActionModalOpen(true); // Abre o Modal de "Próxima Ação"
    } catch (error) {
      message.error("Falha ao salvar o relatório.");
      throw error;
    }
  };

  const handleNextActionClose = () => {
    setIsNextActionModalOpen(false);
    setIdsSalvos(null);
  };

  const handleIrParaAcompanhamento = (animalId) => {
    handleNextActionClose();
    navigate(`/clinica/animais/acompanhamento/${animalId}`);
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

  const handleCalcularDietaPosRelatorio = () => {
    setIsNextActionModalOpen(false); // Fecha o pop-up
    setIsModalDietaOpen(true); // Abre o formulário da dieta
  };

  const handleCloseModalDieta = () => {
    setIsModalDietaOpen(false);
    setIdsSalvos(null);
  };

  const handleSubmitDieta = async (formValues) => {
    setIsSubmittingDieta(true);
    try {
      const payloadDieta = {
        ...formValues,
        acompanhamentoId: idsSalvos.acompanhamentoId,
        relatorioId: idsSalvos.relatorioId,
      };

      const result = await acompanhamentoAPI.criarDieta(payloadDieta);

      if (result.succeeded) {
        message.success("Dieta calculada e salva com sucesso!");
        handleCloseModalDieta();
        await fetchConsultas();
      } else {
        message.error(result.message || "Falha ao criar dieta.");
      }
    } catch (error) {
      message.error(error.message || "Ocorreu um erro inesperado.");
    } finally {
      setIsSubmittingDieta(false);
    }
  };

  const handleCalcularDietaDaTabela = async (consulta) => {
    try {
      // 1. Precisamos dos dados vitais (peso, racao) que estão dentro do relatório
      const response = await relatoriosDaClinicaAPI.getByConsultaId(
        consulta.id,
      );
      const relatorio = response.data;

      // 2. Prepara o objeto para a UI
      setIdsSalvos({
        relatorioId: relatorio.id,
        acompanhamentoId: consulta.acompanhamentoId,
        animalId: consulta.animalId,
        peso: relatorio.peso,
        racaoId: relatorio.racaoId,
        tipoDieta: relatorio.tipoDieta,
        nomeRaca: consulta.nomeRaca,
        pesoMinimoRaca: consulta.pesoMinimoRaca,
        pesoMaximoRaca: consulta.pesoMaximoRaca,
      });

      // 3. Busca a lista de rações baseada na espécie
      const especie =
        consulta.especieAnimal === "Gato" || consulta.especieAnimal === 2
          ? 2
          : 1;
      const racoes = await racoesDaClinicaAPI.listar(especie);
      setRacoesDisponiveis(racoes);

      // 4. Abre o modal
      setIsModalDietaOpen(true);
    } catch (error) {
      message.error("Não foi possível carregar os dados base para a dieta.");
    }
  };

  const handleVerDietaDaTabela = async (consulta) => {
    try {
      // Fetches the report to extract the nested diet object
      const response = await relatoriosDaClinicaAPI.getByConsultaId(
        consulta.id,
      );
      const relatorio = response.data;

      // Extract the diet (adjust 'dietaPersonalizada' to match your backend DTO property name if necessary)
      const dieta = relatorio.dieta || relatorio.dietaPersonalizada;

      if (dieta) {
        setDietaParaVisualizar(dieta);
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
    handleDeleteRelatorio,
    isNextActionModalOpen,
    idsSalvos,
    handleNextActionClose,
    handleIrParaAcompanhamento,
    isModalDietaOpen,
    racoesDisponiveis,
    isSubmittingDieta,
    handleCalcularDietaPosRelatorio,
    handleCloseModalDieta,
    handleSubmitDieta,
    handleCalcularDietaDaTabela,
    isModalVisualizarDietaOpen,
    dietaParaVisualizar,
    handleVerDietaDaTabela,
    handleCloseModalVisualizarDieta,
  };
};
