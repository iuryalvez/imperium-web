import { renderHook, act } from "@testing-library/react";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useNovaSenha } from "./useNovaSenha";

jest.mock("../../../api/autenticacaoAPI", () => ({
  autenticacaoAPI: { resetPassword: jest.fn() },
}));

const mockNotification = { success: jest.fn(), error: jest.fn() };
jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  App: { useApp: () => ({ notification: mockNotification }) },
}));

// O link do e-mail traz ?email=...&token=...
let mockParametros = new URLSearchParams();
const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [mockParametros],
  useNavigate: () => mockNavigate,
}));

beforeEach(() => {
  jest.clearAllMocks();
  mockParametros = new URLSearchParams({ email: "vera@teste.local", token: "tok-123" });
});

test("link com e-mail e token é válido e expõe o e-mail", () => {
  const { result } = renderHook(() => useNovaSenha());

  expect(result.current.isValidLink).toBe(true);
  expect(result.current.email).toBe("vera@teste.local");
});

test("envia e-mail, token e a senha nova, avisa e vai para o login", async () => {
  autenticacaoAPI.resetPassword.mockResolvedValue({});
  const { result } = renderHook(() => useNovaSenha());

  await act(() => result.current.onFinish({ senha: "Nova@123", confirmarSenha: "Nova@123" }));

  expect(autenticacaoAPI.resetPassword).toHaveBeenCalledWith({ email: "vera@teste.local", token: "tok-123", newPassword: "Nova@123" });
  expect(mockNotification.success).toHaveBeenCalledWith(expect.objectContaining({ message: "Senha Redefinida" }));
  expect(mockNavigate).toHaveBeenCalledWith("/login");
  expect(result.current.loading).toBe(false);
});

test("link sem token é inválido e não chama a api", async () => {
  mockParametros = new URLSearchParams({ email: "vera@teste.local" });
  const { result } = renderHook(() => useNovaSenha());

  await act(() => result.current.onFinish({ senha: "Nova@123" }));

  expect(result.current.isValidLink).toBe(false);
  expect(autenticacaoAPI.resetPassword).not.toHaveBeenCalled();
});

test("token expirado: mostra a mensagem do backend e não navega", async () => {
  autenticacaoAPI.resetPassword.mockRejectedValue({ message: "Invalid token." });
  const { result } = renderHook(() => useNovaSenha());

  await act(() => result.current.onFinish({ senha: "Nova@123" }));

  expect(mockNotification.error).toHaveBeenCalledWith({ message: "Falha na Redefinição", description: "Invalid token." });
  expect(mockNavigate).not.toHaveBeenCalled();
  expect(result.current.loading).toBe(false);
});

test("erro sem mensagem usa o texto padrão", async () => {
  autenticacaoAPI.resetPassword.mockRejectedValue({});
  const { result } = renderHook(() => useNovaSenha());

  await act(() => result.current.onFinish({ senha: "Nova@123" }));

  expect(mockNotification.error).toHaveBeenCalledWith({
    message: "Falha na Redefinição",
    description: "Link inválido ou expirado. Tente solicitar um novo link.",
  });
});
