import api from "../index"; // configuração do axios
import endpoints from "../../services/Endpoints"; // endpoints

export const animaisDaClinicaAPI = {
  listar: async (params = {}) => {
    try {
      const response = await api.get(endpoints.clinica.animal.listar, { params });
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar animais da clínica:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosAnimal) => {
    try {
      const response = await api.post(
        endpoints.clinica.animal.cadastrar,
        dadosAnimal,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao cadastrar animal:", error);
      throw error.response ? error.response.data : error;
    }
  },
  editar: async (dadosAnimalEditados) => {
    try {
      console.log(dadosAnimalEditados);
      const response = await api.put(
        endpoints.clinica.animal.editar,
        dadosAnimalEditados,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data.data;
    } catch (error) {
      console.error("Erro ao editar animal:", error);
      throw error.response ? error.response.data : error;
    }
  },
  deletar: async (animalId) => {
    try {
      const response = await api.delete(
        endpoints.clinica.animal.deletar(animalId)
      );
      return response.data;
    } catch (error) {
      console.error(`Erro ao deletar animal de id: ${animalId}`, error);
    }
  },
};
