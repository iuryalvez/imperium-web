import { useState } from "react";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { App } from "antd";

export const useRecuperarSenha = () => {
  const [loading, setLoading] = useState(false);
  const { notification } = App.useApp();

  const onFinish = async (values) => {
    setLoading(true);
    try {
      const response = await autenticacaoAPI.recuperarSenha(values.email);
      notification.success({
        message: "E-mail Enviado",
        description: response.message || "Se o e-mail estiver registrado, um link será enviado.",
      });
    } catch (error) {
      notification.error({
        message: "Erro",
        description: error.message || "Não foi possível processar a solicitação.",
      });
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    onFinish,
  };
};