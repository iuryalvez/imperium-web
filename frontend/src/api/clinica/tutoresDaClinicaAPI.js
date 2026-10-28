import api from "../index"; // configuração do axios
import endpoints from "../../services/Endpoints"; // endpoints

export const tutoresDaClinicaAPI = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.clinica.tutor.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar tutores:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosTutor) => {
    try {
      const response = await api.post(
        endpoints.clinica.tutor.cadastrar,
        dadosTutor,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao cadastrar tutor:", error);
      throw error.response ? error.response.data : error;
    }
  },
  editar: async (dadosTutorEditados) => {
    try {
      console.log(dadosTutorEditados);
      const response = await api.put(
        endpoints.clinica.tutor.editar,
        dadosTutorEditados,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data.data;
    } catch (error) {
      console.error("Erro ao editar tutor:", error);
      throw error.response ? error.response.data : error;
    }
  },
  deletar: async (tutorId) => {
    try {
      const response = await api.delete(
        endpoints.clinica.tutor.deletar(tutorId)
      );
      return response.data;
    } catch (error) {
      console.error(`Erro ao deletar tutor de id: ${tutorId}`, error);
      throw error.response ? error.response.data : error;
    }
  },
  // Ativa (true) ou desativa (false) o tutor na clínica
  setStatus: async (tutorId, ativo) => {
    try {
      const response = await api.put(endpoints.clinica.tutor.setStatus(tutorId), null, {
        params: { ativo },
      });
      return response.data;
    } catch (error) {
      console.error(`Erro ao alterar o status do tutor ${tutorId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
};
