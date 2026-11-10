import { renderHook, act } from "@testing-library/react";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useRecuperarSenha } from "./useRecuperarSenha";

jest.mock("../../../api/autenticacaoAPI", () => ({
  autenticacaoAPI: { recuperarSenha: jest.fn() },
}));

// As telas de Auth mostram notificações pelo App.useApp() do antd
const mockNotification = { success: jest.fn(), error: jest.fn() };
jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  App: { useApp: () => ({ notification: mockNotification }) },
}));

beforeEach(() => {
  jest.clearAllMocks();
});

// O envio só acontece depois que o captcha (Turnstile) devolve um token
const renderComCaptcha = () => {
  const hook = renderHook(() => useRecuperarSenha());
  act(() => hook.result.current.setCaptchaToken("captcha-ok"));
  return hook;
};

test("sem captcha resolvido não chama a api", async () => {
  const { result } = renderHook(() => useRecuperarSenha());

  await act(() => result.current.onFinish({ email: "vera@teste.local" }));

  expect(autenticacaoAPI.recuperarSenha).not.toHaveBeenCalled();
});

test("envia o e-mail com o captcha e mostra a mensagem do backend", async () => {
  autenticacaoAPI.recuperarSenha.mockResolvedValue({ message: "Se o e-mail existir, enviamos um link." });
  const { result } = renderComCaptcha();

  await act(() => result.current.onFinish({ email: "vera@teste.local" }));

  expect(autenticacaoAPI.recuperarSenha).toHaveBeenCalledWith({ email: "vera@teste.local", captchaToken: "captcha-ok" });
  expect(result.current.captchaToken).toBeNull(); // o token do captcha vale para um envio só
  expect(mockNotification.success).toHaveBeenCalledWith({ message: "E-mail Enviado", description: "Se o e-mail existir, enviamos um link." });
  expect(result.current.loading).toBe(false);
});

test("sucesso sem mensagem usa o texto padrão (não revela se o e-mail existe)", async () => {
  autenticacaoAPI.recuperarSenha.mockResolvedValue({});
  const { result } = renderComCaptcha();

  await act(() => result.current.onFinish({ email: "ninguem@teste.local" }));

  expect(mockNotification.success).toHaveBeenCalledWith({
    message: "E-mail Enviado",
    description: "Se o e-mail estiver registrado, um link será enviado.",
  });
});

test("erro do backend mostra a mensagem dele e libera o botão", async () => {
  autenticacaoAPI.recuperarSenha.mockRejectedValue({ message: "O e-mail não é válido." });
  const { result } = renderComCaptcha();

  await act(() => result.current.onFinish({ email: "x" }));

  expect(mockNotification.error).toHaveBeenCalledWith({ message: "Erro", description: "O e-mail não é válido." });
  expect(mockNotification.success).not.toHaveBeenCalled();
  expect(result.current.loading).toBe(false);
});

test("erro sem mensagem usa o texto padrão", async () => {
  autenticacaoAPI.recuperarSenha.mockRejectedValue({});
  const { result } = renderComCaptcha();

  await act(() => result.current.onFinish({ email: "vera@teste.local" }));

  expect(mockNotification.error).toHaveBeenCalledWith({ message: "Erro", description: "Não foi possível processar a solicitação." });
});
