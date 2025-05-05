import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const animaisDaClinicaAPI = {
    listar: async () => {
        try {
            const response = await api.get(endpoints.clinica.animal.listar);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar animais:', error);
            throw error.response ? error.response.data : error;
        }
    }
};
