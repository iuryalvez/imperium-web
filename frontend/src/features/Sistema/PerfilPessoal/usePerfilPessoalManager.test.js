import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { perfilAPI } from "../../../api/sistema/perfilAPI.js";
import { usePerfilPessoalManager } from "./usePerfilPessoalManager";

jest.mock("../../../api/sistema/perfilAPI.js", () => ({
  perfilAPI: { getMeuPerfil: jest.fn(), updateMeuPerfil: jest.fn() },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const perfil = { nome: "Vera", email: "vera@teste.local", telefone: "62999990000", dataCadastro: "2025-03-15T13:45:00" };

const montarHook = async () => {
  const hook = renderHook(() => usePerfilPessoalManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  perfilAPI.getMeuPerfil.mockResolvedValue({ succeeded: true, data: { ...perfil } });
});

describe("carga do perfil", () => {
  test("carrega o perfil com a data de cadastro formatada", async () => {
    const { result } = await montarHook();

    expect(perfilAPI.getMeuPerfil).toHaveBeenCalledTimes(1);
    expect(result.current.perfil).toEqual({ ...perfil, dataCadastro: "15/03/2025" });
  });

  test("erro do backend mostra a mensagem", async () => {
    perfilAPI.getMeuPerfil.mockRejectedValue({ message: "Usuário não encontrado." });

    const { result } = await montarHook();

    expect(message.error).toHaveBeenCalledWith("Usuário não encontrado.");
    expect(result.current.perfil).toBeNull();
  });
});

describe("salvar", () => {
  test("envia telefones sem máscara, sem o confirmarSenha, mostra sucesso e recarrega", async () => {
    perfilAPI.updateMeuPerfil.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() =>
      result.current.handleSubmit({
        nome: "Vera Lúcia",
        telefone: "(62) 99999-0000",
        whatsapp: undefined,
        senhaAtual: "Antiga@1",
        novaSenha: "Nova@123",
        confirmarSenha: "Nova@123",
      })
    );

    expect(perfilAPI.updateMeuPerfil).toHaveBeenCalledWith({
      nome: "Vera Lúcia",
      telefone: "62999990000",
      whatsapp: "",
      senhaAtual: "Antiga@1",
      novaSenha: "Nova@123",
    });
    expect(message.success).toHaveBeenCalledWith("Perfil atualizado com sucesso!");
    expect(perfilAPI.getMeuPerfil).toHaveBeenCalledTimes(2);
    expect(result.current.isSubmitting).toBe(false);
  });

  test("e-mail em uso (409) mostra a mensagem do backend e não recarrega", async () => {
    perfilAPI.updateMeuPerfil.mockRejectedValue({ succeeded: false, message: "E-mail já está em uso." });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit({ email: "outro@teste.local" }));

    expect(message.error).toHaveBeenCalledWith("E-mail já está em uso.");
    expect(perfilAPI.getMeuPerfil).toHaveBeenCalledTimes(1);
    expect(result.current.isSubmitting).toBe(false);
  });

  test("erro na troca de senha com lista de errors mostra o motivo", async () => {
    perfilAPI.updateMeuPerfil.mockRejectedValue({ succeeded: false, errors: ["A senha atual está incorreta."] });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit({ senhaAtual: "errada", novaSenha: "Nova@123" }));

    expect(message.error).toHaveBeenCalledWith("A senha atual está incorreta.");
    expect(perfilAPI.getMeuPerfil).toHaveBeenCalledTimes(1);
    expect(result.current.isSubmitting).toBe(false);
  });

  test("erro sem mensagem nem errors usa o texto padrão", async () => {
    perfilAPI.updateMeuPerfil.mockRejectedValue({ succeeded: false });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit({ senhaAtual: "errada", novaSenha: "Nova@123" }));

    expect(message.error).toHaveBeenCalledWith("Erro ao atualizar perfil.");
  });
});
