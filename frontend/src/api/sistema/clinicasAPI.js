import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const clinicasAPI = {
    listar: async (params = {}) => {
        try {
            const response = await api.get(endpoints.sistema.clinica.listar, { params });
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar clínicas:', error);
            throw error.response ? error.response.data : error;
        }
    },
    cadastrar: async (dadosClinica) => {
        try {
            const response = await api.post(endpoints.sistema.clinica.cadastrar, dadosClinica);
            return response.data;
        } catch (error) {
            console.error(`Erro ao cadastrar clínica com os dados: ${dadosClinica}`, error);
            throw error.response ? error.response.data : error;
        }
    },
    editar: async (dadosAtualizadosClinica) => {
        try {
            const response = await api.put(endpoints.sistema.clinica.editar, dadosAtualizadosClinica);
            return response.data;
        } catch (error) {
            console.error(`Erro ao editar clínica com os dados: ${dadosAtualizadosClinica}`, error);
            throw error.response ? error.response : error;
        }
    }, 
    buscarPorNome: async (nomeClinica) => {
        const response = await api.get(endpoints.clinica.buscarPorNome(nomeClinica));
        return response.data;
    },
    deletar: async(clinicaId) => {
        try {
            const response = await api.delete(endpoints.sistema.clinica.deletar(clinicaId));
            return response.data;
        } catch (error) {
            console.error(`Erro ao deletar clínica de id: ${clinicaId}`, error);
        }
    },
};
