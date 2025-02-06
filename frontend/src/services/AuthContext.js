import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { autenticacao } from '../api/autenticacaoAPI';
import { useNotification } from './NotificationContext';

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [isLogged, setIsLogged] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(null);
  const [userName, setUserName] = useState("");
  const [clinicaNome, setClinicaNome] = useState("");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const abrirNotificacao = useNotification(); // Contexto de notificação

  const checkAuth = useCallback(async () => {
    try {
      const resposta = await autenticacao.verificarAutenticacao();
      console.log('Resposta da checkAuth: ', resposta);

      switch (resposta.status) {
        case 'autenticado':
          setIsLogged(true);
          setIsAuthenticated(true);
          setUserName(resposta.name || "");
          setClinicaNome(resposta.clinicaNome || "");
          break;
        case 'logado':
          setIsLogged(true);
          setIsAuthenticated(false);
          setIsAdmin(resposta.isAdmin);
          setUserName(resposta.name || "");
          break;
        default:
          setIsLogged(false);
          setIsAuthenticated(false);
          setUserName("");
          break;
      }
    } catch (error) {
      abrirNotificacao('error', 'Erro de autenticação', 'Não foi possível verificar a autenticação.');
      setIsAuthenticated(false);
      setIsLogged(false);
      setUserName("");
    } finally {
      setLoading(false);
    }
  }, [abrirNotificacao]);

  useEffect(() => {
    if (loading) checkAuth();
    if (isLogged && !isAuthenticated && !isAdmin) {
      navigate('/sistema/vinculos');
    }
  }, [loading, isAuthenticated, isLogged, isAdmin, navigate, checkAuth]);
  
  const login = async ({ email, senha }) => {
    try {
      const response = await autenticacao.login({ email, senha });
      if (response.status === 200) {
        abrirNotificacao('success', 'Login bem-sucedido!', 'Você foi autenticado com sucesso.');
        await checkAuth();
        if (isAdmin) navigate('/sistema');
        else navigate('/sistema/vinculos');
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
  
  const escolherVinculo = async (id) => {
    try {
      const response = await autenticacao.escolherVinculo(id);
      if (response.status === 200) {
        abrirNotificacao('success', 'Vínculo selecionado!', 'Você escolheu seu vínculo com sucesso.');
        await checkAuth();
        navigate('/clinica/home');
      }
    } catch (error) {
      abrirNotificacao('error', 'Erro ao selecionar vínculo', 'Não foi possível selecionar o vínculo.');
      throw error;
    }
  };

  const logout = async () => {
    try {
      await autenticacao.logout();
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
        loading,
        login,
        escolherVinculo,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
