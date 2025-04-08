import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const clinicasAPI = {
    listar: async () => {
        try {
            const response = await api.get(endpoints.sistema.clinica.listar);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar clínicas:', error);
            throw error.response ? error.response.data : error;
        }
    },
    cadastrar: async (userData) => { // nao sei se ta certo o post
        try {
            const response = await api.post(endpoints.sistema.clinica.cadastrar, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar clínicas:', error);
            throw error.response ? error.response.data : error;
        }
    },
};