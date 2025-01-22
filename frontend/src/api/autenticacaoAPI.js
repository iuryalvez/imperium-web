import api from './index';  // configuração do axios
import endpoints from '../services/Endpoints'; // endpoints

export const autenticacao = {
    signup: async (userData) => {
        try {
            const response = await api.post(endpoints.autenticacao.signup, userData);
            return response.data;
        } catch (error) {
            console.error('Erro ao cadastrar o usuário:', error);
            throw error.response ? error.response.data : error;
        }
    },

    login: async ({ email, senha, rememberMe }) => {
        console.log("---------------------------Entrou na api---------------------------");
        console.log("\nemail na api: ", email, "\nsenha na api: ", senha);
        try {
            const response = await api.post(endpoints.autenticacao.login, { email, senha, rememberMe });
            return response;
        } catch (error) {
            console.error('Erro ao fazer login:', error);
            throw error.response ? error.response.data : error;
        }
    },

    logout: async () => {
        try {
            const response = await api.get(endpoints.autenticacao.logout);
            return response.data;
        } catch (error) {
            console.error('Erro ao fazer logout:', error);
            throw error.response ? error.response.data : error;
        }
    },

    escolherVinculo: async (id) => {
        try {
            const response = await api.post(endpoints.autenticacao.escolherVinculo(id));
            return response;
        } catch (error) {
            console.error('Erro ao escolher vínculo:', error);
            throw error.response ? error.response.data : error;
        }
    },

    recuperarSenha: async (email) => {
        try {
            const response = await api.post(endpoints.autenticacao.recuperarSenha, { email });
            return response.data;
        } catch (error) {
            console.error('Erro ao recuperar senha:', error);
            throw error.response ? error.response.data : error;
        }
    },

    listarVinculos: async () => {
        try {
            const response = await api.get(endpoints.autenticacao.listarVinculos);
            // console.log("Resposta na api do listarVinculos:", response.data);
            return response.data;
        } catch (error) {
            console.error('Erro ao listar vínculos:', error);
            throw error.response ? error.response.data : error;
        }
    },

    verificarAutenticacao: async () => {
        try {
            const response = await api.get(endpoints.autenticacao.checarStatus);
            console.log("response.data: ", response.data)
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
