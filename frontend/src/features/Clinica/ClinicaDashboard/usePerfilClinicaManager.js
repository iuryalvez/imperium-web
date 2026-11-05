import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../services/AuthContext";
import { perfilClinicaAPI } from "../../../api/clinica/perfilClinicaAPI";
import { racasDaClinicaAPI } from "../../../api/clinica/racasDaClinicaAPI";
import { sanitizePayload, sanitizeToNumbers } from "../../../utils/sanitizers";

export const usePerfilClinicaManager = () => {
  const navigate = useNavigate();
  const [dadosClinica, setDadosClinica] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  // Raças que a clínica enxerga (sistema + próprias), por espécie. Vem da mesma listagem do
  // Gerenciador de Raças, então o número bate com o "x registro(s)" de lá.
  const [totalRacas, setTotalRacas] = useState(null);

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

  // Conta só o total (página de 1 item); se falhar, o card de animais mostra só os animais.
  const fetchTotalRacas = useCallback(async () => {
    try {
      const [caes, gatos] = await Promise.all([
        racasDaClinicaAPI.listar({ pagina: 1, tamanhoPagina: 1, especie: 1 }),
        racasDaClinicaAPI.listar({ pagina: 1, tamanhoPagina: 1, especie: 2 }),
      ]);
      setTotalRacas({ caes: caes?.total ?? 0, gatos: gatos?.total ?? 0 });
    } catch {
      setTotalRacas(null);
    }
  }, []);

  useEffect(() => {
    fetchClinicaDashboard();
    fetchTotalRacas();
  }, [fetchClinicaDashboard, fetchTotalRacas]);

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
    totalRacas,
    loading,
    isSubmitting,
    isModalOpen,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleIrParaMeuPainel: () => navigate("/clinica/inicio"),
    // Atalhos da Visão Geral: cada card leva à tela da entidade
    handleIrPara: (rota) => navigate(rota),
  };
};
