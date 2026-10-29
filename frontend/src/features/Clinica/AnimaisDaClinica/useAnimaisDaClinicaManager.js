import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { animaisDaClinicaAPI } from "../../../api/clinica/animaisDaClinicaAPI";
import { message } from "antd";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosAnimal } from "./components/TabelaAnimaisDaClinica/colunas";

export const useAnimaisDaClinicaManager = () => {
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(animaisDaClinicaAPI.listar, {}, traduzirFiltrosAnimal);
  const animais = lista.dados;
  const loading = lista.loading;
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Estados para o modal de ANIMAL (Cadastro/Edição)
  const [isModalAnimalOpen, setIsModalAnimalOpen] = useState(false);
  const [editingAnimal, setEditingAnimal] = useState(null);
  // Detalhes
  const [viewingAnimal, setViewingAnimal] = useState(null); // Guarda o animal a ser visualizado
  const [isModalDetalhesOpen, setIsModalDetalhesOpen] = useState(false);

  // Estado para o modal de RAÇAS
  const [isModalRacasOpen, setIsModalRacasOpen] = useState(false);

  const navigate = useNavigate();

  const fetchAnimais = lista.recarregar;

  // Handlers para o modal de ANIMAL (Cadastro/Edição)
  const handleOpenModal = (animal = null) => {
    setEditingAnimal(animal); // Se for null, é cadastro. Se tiver objeto, é edição.
    setIsModalAnimalOpen(true);
  };
  const handleCloseModal = () => {
    setIsModalAnimalOpen(false);
    setEditingAnimal(null);
  };

  // Handlers para o modal de RAÇAS
  const handleOpenModalRacas = () => {
    setIsModalRacasOpen(true);
  };
  const handleCloseModalRacas = () => {
    setIsModalRacasOpen(false);
  };

  const handleNavigateToAcompanhamento = (acompanhamentoId) => {
    navigate(`/clinica/animais/acompanhamento/${acompanhamentoId}`);
  };

  // Handler para submissão do formulário de animal
  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      const { sobTutelaClinica, ...dadosParaEnviar } = values;
      let successMessage = "";
      if (editingAnimal) {
        await animaisDaClinicaAPI.editar({ ...editingAnimal, ...dadosParaEnviar });
        successMessage = "Animal atualizado com sucesso!";
      } else {
        await animaisDaClinicaAPI.cadastrar(dadosParaEnviar);
        successMessage = "Animal cadastrado com sucesso!";
      }

      message.success(successMessage);
      handleCloseModal();
      await fetchAnimais();
    } catch (error) {
      message.error(error.message || "Falha ao salvar o animal.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (animalId) => {
    try {
      await animaisDaClinicaAPI.deletar(animalId);
      message.success("Animal deletado com sucesso!");
      await fetchAnimais();
    } catch (error) {
      message.error(error.message || "Falha ao deletar o animal.");
    }
  };

  // Ativa/desativa o animal. Inativo continua no histórico, mas não entra em vínculos nem consultas novas.
  const handleToggleStatus = async (animal) => {
    setLoadingRowKey(animal.id);
    try {
      const novoStatus = !animal.isAtivo;
      await animaisDaClinicaAPI.setStatus(animal.id, novoStatus);
      message.success(`Animal ${novoStatus ? "ativado" : "desativado"} com sucesso!`);
      await fetchAnimais();
    } catch (error) {
      message.error(error.message || "Falha ao alterar o status do animal.");
    } finally {
      setLoadingRowKey(null);
    }
  };

  const handleOpenModalDetalhes = (animal) => {
    setViewingAnimal(animal);
    setIsModalDetalhesOpen(true);
  };

  const handleCloseModalDetalhes = () => {
    setIsModalDetalhesOpen(false);
    // Adiciona um pequeno delay para limpar os dados, permitindo que a animação do modal termine.
    setTimeout(() => setViewingAnimal(null), 300);
  };

  return {
    paginacao: lista.paginacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    animais,
    loading,
    loadingRowKey,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    handleOpenModal,
    handleCloseModal,
    handleOpenModalRacas,
    handleCloseModalRacas,
    handleNavigateToAcompanhamento,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
    handleOpenModalDetalhes,
    handleCloseModalDetalhes,
  };
};
