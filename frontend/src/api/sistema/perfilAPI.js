import api from '../index';  // configuração do axios
import endpoints from '../../services/Endpoints'; // endpoints

export const perfilAPI = {
    getMeuPerfil: async () => {
        try {
            const response = await api.get(endpoints.sistema.usuario.perfil.check);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar perfil:', error);
            throw error.response ? error.response.data : error;
        }
    },
    updateMeuPerfil: async (dadosAtualizados) => {
        try {
            const response = await api.put(endpoints.sistema.usuario.perfil.editar, dadosAtualizados);
            return response.data;
        } catch (error) {
            console.error(`Erro ao atualizar perfil com os dados: ${dadosAtualizados}`, error);
            throw error.response ? error.response.data : error;
        }
    },
}