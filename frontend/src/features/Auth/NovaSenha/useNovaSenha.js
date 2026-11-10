import { useState } from "react";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useSearchParams, useNavigate } from "react-router-dom";
import { App } from "antd";

export const useNovaSenha = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const { notification } = App.useApp();

  // O link do e-mail traz ?token=...&uid=<id do usuário> (o e-mail não vai na URL).
  // O id pode ser negativo (usuários do seed), só não pode faltar nem ser zero.
  const token = searchParams.get("token");
  const userId = Number(searchParams.get("uid"));

  const isValidLink = Boolean(token && Number.isInteger(userId) && userId !== 0);

  const onFinish = async (values) => {
    if (!isValidLink) return;

    setLoading(true);
    try {
      await autenticacaoAPI.resetPassword({
        userId,
        token,
        newPassword: values.senha,
      });

      notification.success({
        message: "Senha Redefinida",
        description: "Sua senha foi alterada com sucesso. Faça login para continuar.",
      });

      navigate("/login");
    } catch (error) {
      notification.error({
        message: "Falha na Redefinição",
        description: error.message || "Link inválido ou expirado. Tente solicitar um novo link.",
      });
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    isValidLink,
    onFinish,
  };
};
