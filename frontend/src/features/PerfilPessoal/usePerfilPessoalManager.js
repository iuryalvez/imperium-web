import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
// Importe a sua API configurada para os endpoints /sistema/perfil
import { perfilAPI } from "../../api/sistema/perfilAPI.js"; 

export const usePerfilPessoalManager = () => {
  const [perfil, setPerfil] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchPerfil = useCallback(async () => {
    setLoading(true);
    try {
      const dados = await perfilAPI.getMeuPerfil();
      setPerfil(dados.data);
    } catch (error) {
      message.error(error.message || "Erro ao carregar perfil.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchPerfil();
  }, [fetchPerfil]);

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      // Remove campos auxiliares que não estão no DTO (ex: confirmarSenha)
      const payload = { ...values };
      delete payload.confirmarSenha;

      await perfilAPI.updateMeuPerfil(payload);
      message.success("Perfil atualizado com sucesso!");
      
      // Limpa os campos de senha após sucesso e recarrega dados
      await fetchPerfil(); 
    } catch (error) {
      message.error(error.message || "Erro ao atualizar perfil.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    perfil,
    loading,
    isSubmitting,
    handleSubmit,
  };
};