import { useState, useEffect, useCallback, useMemo } from "react";
import { message } from "antd";
import { racasAPI } from "../../../../api/sistema/racasAPI";

export const useRacasDaClinicaManager = () => {
  const [allRacas, setAllRacas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingRaca, setEditingRaca] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [filtroEspecie, setFiltroEspecie] = useState(null); // 'Cao', 'Gato' ou null

  const fetchRacas = useCallback(async () => {
    setLoading(true);
    try {
      const data = await racasAPI.listar();
      setAllRacas(data);
    } catch (error) {
      message.error("Erro ao carregar lista de raças.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchRacas();
  }, [fetchRacas]);

  const filteredRacas = useMemo(() => {
    // Se não há filtros ativos, retorna a lista completa
    if (!searchTerm && !filtroEspecie) {
      return allRacas;
    }

    // Se há filtros, aplica a lógica
    return allRacas.filter((raca) => {
      const matchWithEspecie = !filtroEspecie || raca.especie === filtroEspecie;
      const matchWithSearch =
        !searchTerm ||
        raca.nome.toLowerCase().includes(searchTerm.toLowerCase());
      return matchWithEspecie && matchWithSearch;
    });
  }, [allRacas, filtroEspecie, searchTerm]);

  const handleDelete = async (id) => {
    try {
      await racasAPI.deletar(id);
      message.success("Raça excluída com sucesso!");
      await fetchRacas();
    } catch (error) {
      message.error(error.message || "Erro ao excluir raça.");
    }
  };

  const handleOpenModal = (raca = null) => {
    setEditingRaca(raca);
    setIsModalOpen(true);
  };
  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingRaca(null);
  };

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
    setFiltroEspecie(null); // Limpa o filtro de espécie ao fazer uma busca textual
  };

  const handleFilterByEspecie = (especie) => {
    setSearchTerm(""); // Limpa a busca textual ao filtrar por espécie
    setFiltroEspecie(especie);
  };

  // Handler para submissão do formulário de raça
  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      let successMessage = "";
      if (editingRaca) {
        await racasAPI.editar({ ...editingRaca, ...values });
        successMessage = "Raça atualizada com sucesso!";
      } else {
        await racasAPI.cadastrar(values);
        successMessage = "Raça cadastrada com sucesso!";
      }

      message.success(successMessage);
      handleCloseModal();
      await fetchRacas();
      
    } catch (error) {
      message.error(error.message || "Falha ao salvar a raça.");
    } finally {
      setIsSubmitting(false);
    }
  };

  // Retorna todos os estados e handlers que a UI precisa
  return {
    loading,
    isModalOpen,
    isSubmitting,
    editingRaca,
    filteredRacas,
    searchTerm,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleSearch,
    handleFilterByEspecie,
  };
};
