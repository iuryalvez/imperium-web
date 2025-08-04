import { useState, useCallback, useEffect } from 'react';
import { message } from 'antd';
import { consultasDaClinicaAPI } from '../../api/clinica/consultasDaClinicaAPI';

export const useConsultasManager = () => {
    const [consultas, setConsultas] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingConsulta, setEditingConsulta] = useState(null);

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

    const handleSubmit = async (values) => {
        try {
            if (editingConsulta) {
                await consultasDaClinicaAPI.editar(editingConsulta.id, values);
                message.success("Consulta atualizada com sucesso!");
            } else {
                await consultasDaClinicaAPI.cadastrar(values);
                message.success("Consulta agendada com sucesso!");
            }
            handleCloseModal();
            fetchConsultas();
        } catch (error) {
            message.error(error.message || "Falha ao salvar consulta.");
            throw error; // Re-lança o erro para o formulário poder manter o modal aberto
        }
    };

    const handleDelete = async (consultaId) => {
        try {
            await consultasDaClinicaAPI.deletar(consultaId);
            message.success("Consulta excluída com sucesso!");
            fetchConsultas();
        } catch (error) {
            message.error(error.response?.data?.message || "Erro ao excluir consulta.");
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
    };
};