import api from "./index"; // configuração do axios
import endpoints from "../services/Endpoints"; // endpoints

export const autenticacaoAPI = {
  login: async ({ email, senha, captchaToken, rememberMe }) => {
    try {
      const response = await api.post(endpoints.autenticacao.login, {
        email,
        senha,
        captchaToken,
        rememberMe,
      });
      return response;
    } catch (error) {
      console.error("Erro ao fazer login:", error);
      throw error.response ? error.response.data : error;
    }
  },

  logout: async () => {
    try {
      // POST: um site externo não consegue deslogar o usuário com um simples link
      const response = await api.post(endpoints.autenticacao.logout);
      return response.data;
    } catch (error) {
      console.error("Erro ao fazer logout:", error);
      throw error.response ? error.response.data : error;
    }
  },

  escolherVinculo: async (vinculoId) => {
    try {
      const response = await api.post(
        endpoints.autenticacao.escolherVinculo(vinculoId),
      );
      return response;
    } catch (error) {
      console.error("Erro ao escolher vínculo:", error);
      throw error.response ? error.response.data : error;
    }
  },

  escolherVinculoAdmin: async (clinicaId, perfis) => {
    try {
      const response = await api.post(
        endpoints.autenticacao.escolherVinculoAdmin,
        { clinicaId, perfis },
      );
      return response;
    } catch (error) {
      console.error("Erro ao escolher vínculo:", error);
      throw error.response ? error.response.data : error;
    }
  },

  trocarVinculo: async () => {
    try {
      const response = await api.post(endpoints.autenticacao.trocarVinculo);
      return response;
    } catch (error) {
      console.error("Erro ao trocar vínculo:", error);
      throw error.response ? error.response.data : error;
    }
  },

  recuperarSenha: async ({ email, captchaToken }) => {
    try {
      const response = await api.post(endpoints.autenticacao.recuperarSenha, {
        email,
        captchaToken,
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao recuperar senha:", error);
      throw error.response ? error.response.data : error;
    }
  },

  resetPassword: async ({ userId, token, newPassword }) => {
    try {
      const response = await api.post(endpoints.autenticacao.resetPassword, {
        userId,
        token,
        newPassword,
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao redefinir senha:", error);
      throw error.response ? error.response.data : error;
    }
  },

  listarVinculos: async () => {
    try {
      const response = await api.get(endpoints.autenticacao.listarVinculos);
      // console.log("Resposta na api do listarVinculos:", response.data);
      return response.data;
    } catch (error) {
      console.error("Erro ao listar vínculos:", error);
      throw error.response ? error.response.data : error;
    }
  },

  verificarAutenticacao: async () => {
    try {
      const response = await api.get(endpoints.autenticacao.checarStatus);
      // console.log(response.data)
      return response.data;
      // response.data tem o seguinte formato:
      // {status = logado}, para usuários que só têm token de login
      // {
      //  status = autenticado,
      //  role = NOME
      // }                  para usuários com token de vínculo
      // ou
      // {status = invalido} caso não haja token válido
    } catch (error) {
      return false; // Em caso de erro (ex.: 401), retorna false
    }
  },
};
