import api from '../index';
import endpoints from '../../services/Endpoints';

export const perfilClinicaAPI = {
    getClinicaDashboard: async () => {
        console.log('[perfilClinica] Iniciando busca do dashboard da clínica...');
        try {
            const response = await api.get(endpoints.clinica.check);
            console.log('[perfilClinica] Dados da clínica carregados com sucesso.');
            return response.data; // Retorna o objeto ServiceResult
        } catch (error) {
            console.error('[perfilClinica] Erro ao buscar dados da clínica:', error);
            throw error.response ? error.response.data : error;
        }
    },

    updateOwnClinica: async (dadosAtualizados) => {
        console.log('[perfilClinica] Iniciando atualização. Payload:', JSON.stringify(dadosAtualizados));
        try {
            const response = await api.put(endpoints.clinica.editar, dadosAtualizados);
            console.log('[perfilClinica] Clínica atualizada com sucesso.');
            return response.data; // Retorna o objeto ServiceResult
        } catch (error) {
            console.error('[perfilClinica] Erro ao atualizar clínica:', error);
            throw error.response ? error.response.data : error;
        }
    },
};