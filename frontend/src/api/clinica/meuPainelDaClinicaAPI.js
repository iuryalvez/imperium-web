import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const meuPainelDaClinicaAPI = {
  /** Resumo do usuário logado na clínica: dados pessoais e uma seção por perfil (GET /clinica/meu-painel). */
  obter: async () => {
    try {
      const response = await api.get(endpoints.clinica.meuPainel);
      return response.data;
    } catch (error) {
      console.error("Erro ao carregar o Meu Painel:", error);
      throw error.response ? error.response.data : error;
    }
  },
};
