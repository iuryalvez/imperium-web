import api from './index';  // configuração do axios
import endpoints from '../services/Endpoints'; // endpoints

export const sistema = {
    listarClinicas: async (userData) => {
        try {
            const response = await api.get(endpoints.sistema.listarClinicas, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar clínicas:', error);
            throw error.response ? error.response.data : error;
        }
    },
    
    novaClinica: async (userData) => { // nao sei se ta certo o post
        try {
            const response = await api.post(endpoints.sistema.novaClinica, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar animais:', error);
            throw error.response ? error.response.data : error;
        }
    },

    listarUsuarios: async (userData) => {
        try {
            const response = await api.get(endpoints.sistema.listarUsuarios, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar usuários:', error);
            throw error.response ? error.response.data : error;
        }
    },
    
    listarAnimais: async (userData) => {
        try {
            const response = await api.get(endpoints.sistema.listarAnimais, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar animais:', error);
            throw error.response ? error.response.data : error;
        }
    }
};
