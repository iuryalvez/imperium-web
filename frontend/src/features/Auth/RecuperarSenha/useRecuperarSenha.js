import { useRef, useState } from "react";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { App } from "antd";

export const useRecuperarSenha = () => {
  const [loading, setLoading] = useState(false);
  const [captchaToken, setCaptchaToken] = useState(null);
  const turnstileRef = useRef(null);
  const { notification } = App.useApp();

  const onFinish = async (values) => {
    if (!captchaToken) return;

    setLoading(true);
    try {
      const response = await autenticacaoAPI.recuperarSenha({
        email: values.email,
        captchaToken,
      });
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
      // O token do captcha vale para um envio só: pede um novo para a próxima tentativa
      turnstileRef.current?.reset();
      setCaptchaToken(null);
      setLoading(false);
    }
  };

  return {
    loading,
    captchaToken,
    setCaptchaToken,
    turnstileRef,
    onFinish,
  };
};
