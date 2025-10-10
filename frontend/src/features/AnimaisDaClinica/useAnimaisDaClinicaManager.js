import { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { animaisDaClinicaAPI } from "../../api/clinica/animaisDaClinicaAPI";
import { message } from "antd";

export const useAnimaisDaClinicaManager = () => {
  const [animais, setAnimais] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadingRowKey] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Busca
  const [searchTerm, setSearchTerm] = useState("");

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
      const dadosAnimais = await animaisDaClinicaAPI.listar();
      setAnimais(dadosAnimais);
    } catch (error) {
      message.error(error.message);
      setAnimais([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAnimais();
  }, [fetchAnimais]);

  // Usamos useMemo para que a filtragem só seja recalculada quando a lista mestra ou o termo de busca mudarem.
  const animaisFiltrados = useMemo(() => {
    if (!searchTerm) {
      return animais; // Se a busca estiver vazia, retorna todos os animais
    }
    return animais.filter((animal) =>
      animal.nome.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [animais, searchTerm]);

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
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
    animais: animaisFiltrados,
    loading,
    loadingRowKey,
    searchTerm,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    handleSearch,
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
