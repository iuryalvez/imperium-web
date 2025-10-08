import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { acompanhamentoAPI } from "../../../api/clinica/acompanhamentoAPI";
import { message } from "antd";

export const useAcompanhamentoAnimalManager = () => {
  const { acompanhamentoId } = useParams(); // <-- Pega o ID correto da URL
  const [historico, setHistorico] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!acompanhamentoId) return;

    const fetchHistorico = async () => {
      try {
        setLoading(true);
        const result = await acompanhamentoAPI.getHistoricoCompleto(acompanhamentoId);
        if (result.succeeded) {
          setHistorico(result.data);
        } else {
          throw new Error(result.message || "Histórico não encontrado.");
        }
      } catch (err) {
        message.error(`Erro ao buscar histórico: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchHistorico();
  }, [acompanhamentoId]); // <-- Dependência correta

  return { historico, loading };
};