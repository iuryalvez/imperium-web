import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const clinicaAPI = {
    listarClinicas: async (userData) => {
        try {
            const response = await api.get(endpoints.sistema.clinica.listar, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar clínicas:', error);
            throw error.response ? error.response.data : error;
        }
    },
    
    novaClinica: async (userData) => { // nao sei se ta certo o post
        try {
            const response = await api.post(endpoints.sistema.clinica.nova, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar animais:', error);
            throw error.response ? error.response.data : error;
        }
    },
};