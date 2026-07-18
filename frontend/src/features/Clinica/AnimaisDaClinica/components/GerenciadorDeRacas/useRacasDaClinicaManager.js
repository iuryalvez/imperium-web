import { useState, useEffect, useCallback, useMemo } from "react";
import { message } from "antd";
import { racasAPI } from "../../../../../api/sistema/racasAPI";

export const useRacasDaClinicaManager = () => {
  const [allRacas, setAllRacas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingRaca, setEditingRaca] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [filtroEspecie, setFiltroEspecie] = useState(null);

  const [mostrarRacasSistema, setMostrarRacasSistema] = useState(true);

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
    return allRacas.filter((raca) => {
      const matchWithEspecie = !filtroEspecie || raca.especie === filtroEspecie;
      const matchWithSearch =
        !searchTerm ||
        raca.nome.toLowerCase().includes(searchTerm.toLowerCase());

      // Filters out system breeds if the toggle is disabled
      // System breeds are identified by the absence of a clinicaId
      const isSystemBreed =
        raca.clinicaId === null || raca.clinicaId === undefined;
      const matchWithSistema = mostrarRacasSistema || !isSystemBreed;

      return matchWithEspecie && matchWithSearch && matchWithSistema;
    });
  }, [allRacas, filtroEspecie, searchTerm, mostrarRacasSistema]);

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
    setFiltroEspecie(null);
  };

  const handleFilterByEspecie = (especie) => {
    setSearchTerm("");
    setFiltroEspecie(especie);
  };

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

  return {
    loading,
    isModalOpen,
    isSubmitting,
    editingRaca,
    filteredRacas,
    searchTerm,
    mostrarRacasSistema,
    setMostrarRacasSistema,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleSearch,
    handleFilterByEspecie,
  };
};
