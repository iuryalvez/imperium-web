import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { autenticacaoAPI } from '../api/autenticacaoAPI';
import { useNotification } from './NotificationContext';

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [isLogged, setIsLogged] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(null);
  const [perfis, setPerfis] = useState([]);
  const [userName, setUserName] = useState("");
  const [clinicaNome, setClinicaNome] = useState("");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const abrirNotificacao = useNotification(); // Contexto de notificação

  const checkAuth = useCallback(async () => {
    try {
      const resposta = await autenticacaoAPI.verificarAutenticacao();
  
      switch (resposta.status) {
        case 'autenticado':
          setIsLogged(true);
          setIsAuthenticated(true);
          setIsAdmin(resposta.isAdmin);
          setUserName(resposta.name || "");
          setClinicaNome(resposta.clinicaNome || "");
          setPerfis(resposta.perfis || []);
          break;
          
        case 'logado':
          setIsLogged(true);
          setIsAuthenticated(false);
          setIsAdmin(resposta.isAdmin);
          setUserName(resposta.name || "");
          setPerfis([]);
          break;
  
        default:
          setIsLogged(false);
          setIsAuthenticated(false);
          setIsAdmin(false);
          setUserName("");
          setClinicaNome("");
          setPerfis([]);
          break;
      }
  
      return resposta; // <-- Retorna os dados
    } catch (error) {
      abrirNotificacao('error', 'Erro de autenticação', 'Não foi possível verificar a autenticação.');
      setIsAuthenticated(false);
      setIsLogged(false);
      setIsAdmin(false);
      setUserName("");
      setClinicaNome("");
      setPerfis([]);
      return null; // <-- Em caso de erro
    } finally {
      setLoading(false);
    }
  }, [abrirNotificacao]);
  

  useEffect(() => {
    if (loading) checkAuth();
  }, [loading, isAuthenticated, isLogged, isAdmin, navigate, checkAuth]);
  
  const login = async ({ email, senha }) => {
    try {
      const response = await autenticacaoAPI.login({ email, senha });

      if (response.status === 200) {
        abrirNotificacao('success', 'Login bem-sucedido!', 'Você foi autenticado com sucesso.');

        const authData = await checkAuth(); // agora authData tem isAdmin

        if (authData?.isAdmin) {
          navigate('/sistema');
        } else {
          navigate('/sistema/vinculos');
        }
      }
    } catch (error) {
      console.log("Erro completo:", error);
  
      // Verificando se o erro possui as propriedades esperadas
      if (error.errors) {
        Object.keys(error.errors).forEach((campo) => {
          error.errors[campo].forEach((mensagem) => {
            abrirNotificacao('error', 'Erro de validação', mensagem);
          });
        });
      } else if (error.status === 400) {
        abrirNotificacao('error', 'Erro de validação', 'Verifique os campos e tente novamente.');
      } else {
        abrirNotificacao('error', 'Erro inesperado', 'Algo deu errado. Por favor, tente novamente.');
      }
    }
  };
  
  const escolherVinculo = async (upcId) => {
    try {
      const response = await autenticacaoAPI.escolherVinculo(upcId);
      if (response.status === 200) {
        abrirNotificacao('success', 'Vínculo selecionado!', 'Vínculo acessado com sucesso.');
        await checkAuth();
        navigate('/clinica/inicio');
      }
    } catch (error) {
      abrirNotificacao('error', 'Erro ao selecionar vínculo', 'Não foi possível selecionar o vínculo.');
      throw error;
    }
  };
  
  const escolherVinculoAdmin = async (clinicaId, perfis) => {
    try {
      const response = await autenticacaoAPI.escolherVinculoAdmin(clinicaId, perfis);
      if (response.status === 200) {
        abrirNotificacao('success', 'Vínculo selecionado!', 'Vínculo criado com sucesso.');
        await checkAuth();
        navigate('/clinica/inicio');
      }
    } catch (error) {
      abrirNotificacao('error', 'Erro ao selecionar vínculo', 'Não foi possível selecionar o vínculo.');
      throw error;
    }
  };

  const trocarVinculo = async () => {
    try {
      const authData = await checkAuth();
      
      await autenticacaoAPI.trocarVinculo();
      
      abrirNotificacao('info', 'Troca de vínculo', 'Escolha um novo vínculo para continuar.');
      
      if (authData?.isAdmin) navigate('/sistema'); // Redireciona para escolher novo vínculo
      else navigate('sistema/vinculos')
      
      await checkAuth();

    } catch (error) {
      abrirNotificacao('error', 'Erro ao trocar vínculo', 'Não foi possível trocar de vínculo.');
    }
  };

  const logout = async () => {
    try {
      await autenticacaoAPI.logout();
      abrirNotificacao('success', 'Logout realizado', 'Você saiu do sistema com sucesso.');
      await checkAuth();
      navigate('/login');
    } catch (error) {
      abrirNotificacao('error', 'Erro ao fazer logout', 'Não foi possível realizar o logout.');
    }
  };

  return (
    <AuthContext.Provider
      value={{
        isLogged,
        isAuthenticated,
        isAdmin,
        userName,
        clinicaNome,
        perfis,
        loading,
        login,
        escolherVinculo,
        logout,
        trocarVinculo,
        escolherVinculoAdmin
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
