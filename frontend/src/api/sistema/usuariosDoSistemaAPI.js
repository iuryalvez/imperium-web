import api from "../index";
import endpoints from "../../services/Endpoints";

export const usuariosDoSistemaAPI = {
  async listar() {
    try {
      const response = await api.get(endpoints.sistema.usuario.listar);
      return response.data.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      // Lança a mensagem de erro do backend, ou uma mensagem genérica
      throw new Error(
        error.response?.data?.message || "Falha ao buscar usuários"
      );
    }
  },

  async cadastrar(dadosUsuario) {
    try {
      const response = await api.post(
        endpoints.sistema.usuario.cadastrar,
        dadosUsuario
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao cadastrar usuário:", error);
      throw new Error(
        error.response?.data?.message || "Falha ao cadastrar usuário"
      );
    }
  },

  async editar(dadosUsuarioEditados) {
    try {
      const response = await api.put(
        endpoints.sistema.usuario.editar,
        dadosUsuarioEditados
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao editar usuário:", error);
      throw new Error(
        error.response?.data?.message || "Falha ao editar usuário"
      );
    }
  },

  async setStatus(record, activate) {
    try {
      let url;

      if (record.isAdmin) {
        url = endpoints.sistema.usuario.setStatus(record.id);
      } else {
        url = endpoints.clinica.usuario.setStatus(record.upcId);
      }
      
      const response = await api.put(url, { activate });
      return response.data;
    } catch (error) {
      console.error(`Erro ao definir status para registro:`, record, error);
      throw new Error(
        error.response?.data?.message || "Falha ao alterar status"
      );
    }
  },
};
