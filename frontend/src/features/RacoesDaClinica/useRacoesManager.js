import { useState, useCallback, useEffect } from 'react';
import { message } from 'antd';
import { racoesDaClinicaAPI } from '../../api/clinica/racoesDaClinicaAPI';

export const useRacoesManager = () => {
    // Todos os estados ficam aqui
    const [racoes, setRacoes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingRacao, setEditingRacao] = useState(null); // Guarda a ração sendo editada

    // A lógica de busca de dados
    const fetchRacoes = useCallback(async () => {
        setLoading(true);
        try {
            const data = await racoesDaClinicaAPI.listar();
            setRacoes(data);
        } catch (error) {
            message.error("Erro ao carregar rações.");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchRacoes();
    }, [fetchRacoes]);

    // Todas as funções de manipulação (handlers)
    const handleOpenModal = (racao = null) => {
        setEditingRacao(racao); // Se for null, é cadastro. Se tiver dados, é edição.
        setIsModalOpen(true);
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setEditingRacao(null); // Limpa o registro selecionado
    };

    const handleSubmit = async (values) => {
        try {
            if (editingRacao) { // Lógica de Edição
                await racoesDaClinicaAPI.editar({ ...editingRacao, ...values });
                message.success("Ração atualizada com sucesso!");
            } else { // Lógica de Cadastro
                await racoesDaClinicaAPI.cadastrar(values);
                message.success("Ração cadastrada com sucesso!");
            }
            handleCloseModal();
            fetchRacoes(); // Re-busca os dados
        } catch (error) {
            message.error(error.message || "Falha na operação.");
        }
    };

    const handleDelete = async (racaoId) => {
        try {
            await racoesDaClinicaAPI.deletar(racaoId);
            message.success("Ração excluída com sucesso!");
            fetchRacoes(); // Re-busca os dados
        } catch (error) {
            message.error("Erro ao excluir ração.");
        }
    };

    // O hook retorna uma "API" para o componente de UI usar
    return {
        racoes,
        loading,
        isModalOpen,
        editingRacao,
        handleOpenModal,
        handleCloseModal,
        handleSubmit,
        handleDelete,
    };
};