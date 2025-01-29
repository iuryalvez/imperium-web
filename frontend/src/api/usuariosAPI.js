import api from './index';  // configuração do axios
import endpoints from '../services/Endpoints'; // endpoints

export const usuario = {
    listar: async (userData) => {
        try {
            const response = await api.get(endpoints.usuario.listar, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar usuários:', error);
            throw error.response ? error.response.data : error;
        }
    }
};
