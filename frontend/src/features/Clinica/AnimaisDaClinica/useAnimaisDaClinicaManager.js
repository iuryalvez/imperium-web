import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { animaisDaClinicaAPI } from "../../../api/clinica/animaisDaClinicaAPI";
import { message } from "antd";

export const useAnimaisDaClinicaManager = () => {
  const [animais, setAnimais] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(true);
  const [loadingRowKey] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Busca foi removida temporariamente a pedido do usuário

  // Estados para o modal de ANIMAL (Cadastro/Edição)
  const [isModalAnimalOpen, setIsModalAnimalOpen] = useState(false);
  const [editingAnimal, setEditingAnimal] = useState(null);
  // Detalhes
  const [viewingAnimal, setViewingAnimal] = useState(null); // Guarda o animal a ser visualizado
  const [isModalDetalhesOpen, setIsModalDetalhesOpen] = useState(false);

  // Estado para o modal de RAÇAS
  const [isModalRacasOpen, setIsModalRacasOpen] = useState(false);

  const navigate = useNavigate();

  const fetchAnimais = useCallback(async () => {
    setLoading(true);
    try {
      const params = { page, pageSize };
      const response = await animaisDaClinicaAPI.listar(params);
      setAnimais(response.data || []);
      setTotal(response.totalCount || 0);
    } catch (error) {
      message.error(error.message);
      setAnimais([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  // useEffect que resetava a página foi removido porque o handleApplyFilters já faz isso
  
  useEffect(() => {
    fetchAnimais();
  }, [fetchAnimais]);

  const handleTableChange = (pagination) => {
    setPage(pagination.current);
    setPageSize(pagination.pageSize);
  };

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
      let successMessage = "";
      if (editingAnimal) {
        await animaisDaClinicaAPI.editar({ ...editingAnimal, ...values });
        successMessage = "Animal atualizado com sucesso!";
      } else {
        await animaisDaClinicaAPI.cadastrar(values);
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
    animais,
    total,
    page,
    pageSize,
    loading,
    loadingRowKey,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleOpenModalRacas,
    handleCloseModalRacas,
    handleNavigateToAcompanhamento,
    handleSubmit,
    handleDelete,
    handleOpenModalDetalhes,
    handleCloseModalDetalhes,
  };
};
