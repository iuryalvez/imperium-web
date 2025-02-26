import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const animais = {
    listarAnimais: async (userData) => {
        try {
            const response = await api.get(endpoints.sistema.animal.listar, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar animais:', error);
            throw error.response ? error.response.data : error;
        }
    }
};
