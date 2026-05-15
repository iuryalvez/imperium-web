import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { useAuth } from "../../../services/AuthContext";
import { perfilClinicaAPI } from "../../../api/clinica/perfilClinicaAPI";
import { sanitizePayload, sanitizeToNumbers } from "../../../utils/sanitizers";

export const usePerfilClinicaManager = () => {
  const [dadosClinica, setDadosClinica] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Hook global de autenticação
  const { authData, setAuthData } = useAuth();

  const fetchClinicaDashboard = useCallback(async () => {
    setLoading(true);
    try {
      const result = await perfilClinicaAPI.getClinicaDashboard();

      // Tratamento pelo ServiceResult
      if (!result.succeeded) {
        throw new Error(result.message || "Erro ao carregar os dados.");
      }

      setDadosClinica(result.data);
    } catch (error) {
      message.error(error.message || "Falha de comunicação com o servidor.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchClinicaDashboard();
  }, [fetchClinicaDashboard]);

  const handleOpenModal = () => setIsModalOpen(true);
  const handleCloseModal = () => setIsModalOpen(false);

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      // Sanitização de máscaras para garantir o [StringLength] do backend
      const payloadSanitizado = sanitizePayload({
        ...values,
        cnpj: sanitizeToNumbers(values.cnpj),
        cep: sanitizeToNumbers(values.cep),
        telefone: sanitizeToNumbers(values.telefone),
        whatsapp: sanitizeToNumbers(values.whatsapp),
      });

      const result = await perfilClinicaAPI.updateOwnClinica(payloadSanitizado);

      // Tratamento pelo ServiceResult
      if (!result.succeeded) {
        throw new Error(result.message || "Falha na validação do servidor.");
      }

      message.success(result.message || "Clínica atualizada com sucesso!");

      // Reatividade no Header caso o nome da clínica tenha sido alterado
      if (authData && payloadSanitizado.nomeClinica !== authData.nomeClinica) {
        setAuthData({
          ...authData,
          nomeClinica: payloadSanitizado.nomeClinica,
        });
      }

      handleCloseModal();
      await fetchClinicaDashboard();
    } catch (error) {
      message.error(
        error.message || "Ocorreu um erro ao salvar as alterações.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    dadosClinica,
    loading,
    isSubmitting,
    isModalOpen,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
  };
};
