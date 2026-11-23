import api from '../index';
import endpoints from '../../services/Endpoints';

export const perfilClinicaAPI = {
    getClinicaDashboard: async () => {
        try {
            const response = await api.get(endpoints.clinica.check);
            return response.data; // Retorna o objeto ServiceResult
        } catch (error) {
            console.error('[perfilClinica] Erro ao buscar dados da clínica:', error);
            throw error.response ? error.response.data : error;
        }
    },

    updateOwnClinica: async (dadosAtualizados) => {
        try {
            const response = await api.put(endpoints.clinica.editar, dadosAtualizados);
            return response.data; // Retorna o objeto ServiceResult
        } catch (error) {
            console.error('[perfilClinica] Erro ao atualizar clínica:', error);
            throw error.response ? error.response.data : error;
        }
    },
};