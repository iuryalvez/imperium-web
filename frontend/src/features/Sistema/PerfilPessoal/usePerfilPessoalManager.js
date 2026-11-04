import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
// Importe a sua API configurada para os endpoints /sistema/perfil
import { perfilAPI } from "../../../api/sistema/perfilAPI.js";
import { sanitizePayload, sanitizeToNumbers } from "../../../utils/sanitizers.js";
import { formatarDataCadastroPerfil } from "../../../utils/formatters.js";

export const usePerfilPessoalManager = () => {
  const [perfil, setPerfil] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchPerfil = useCallback(async () => {
    setLoading(true);
    try {
      const dados = await perfilAPI.getMeuPerfil();
      dados.data = {
        ...dados.data,
        dataCadastro: formatarDataCadastroPerfil(dados.data.dataCadastro),
      }
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
      // Sanitização de máscaras para garantir o [StringLength] do backend
      const payload = sanitizePayload({
        ...values,
        telefone: sanitizeToNumbers(values.telefone),
        whatsapp: sanitizeToNumbers(values.whatsapp),
      });
      // Remove campos auxiliares que não estão no DTO (ex: confirmarSenha)
      delete payload.confirmarSenha;

      await perfilAPI.updateMeuPerfil(payload);
      message.success("Perfil atualizado com sucesso!");

      // Limpa os campos de senha após sucesso e recarrega dados
      await fetchPerfil();
    } catch (error) {
      const mensagemErro = error?.message || error?.errors?.join(" ") || "Erro ao atualizar perfil.";
      message.error(mensagemErro);
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
