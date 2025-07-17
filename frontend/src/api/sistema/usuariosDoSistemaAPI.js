import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuariosDoSistemaAPI = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.sistema.usuario.listar);
      return response.data.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosUsuario) => {
    try {
      const response = await api.post(
        endpoints.sistema.usuario.cadastrar,
        dadosUsuario,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao cadastrar usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },
  editar: async (dadosUsuarioEditados) => {
    try {
      const response = await api.put(
        endpoints.sistema.usuario.editar,
        dadosUsuarioEditados,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao editar usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },
  setStatus: async (record, activate) => {
    try {
      let url;
      const payload = { activate };

      if (record.isAdmin) {
        // Se for admin, usa a rota do sistema com o ID do usuário
        url = endpoints.sistema.usuario.setStatus(record.id);
      } else {
        // Se for um usuário de clínica (mesmo na tela de sistema),
        // a ação ainda é sobre o VÍNCULO. Usamos a rota de vínculo.
        url = endpoints.clinica.usuario.setStatus(record.upcId);
      }

      const response = await api.put(url, payload);
      return response.data;
    } catch (error) {
      console.error(`Erro ao definir status para registro:`, record, error);
      throw error.response ? error.response.data : error;
    }
  },
};
