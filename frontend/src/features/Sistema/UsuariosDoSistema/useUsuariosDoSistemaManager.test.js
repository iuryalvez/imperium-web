import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";
import { useUsuariosDoSistemaManager } from "./useUsuariosDoSistemaManager";

jest.mock("../../../api/sistema/usuariosDoSistemaAPI.js", () => ({
  usuariosDoSistemaAPI: { listar: jest.fn(), cadastrar: jest.fn(), editar: jest.fn(), setStatus: jest.fn() },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

// Cada linha da tabela é um vínculo (usuário de clínica) ou um admin
const admin = { compositeKey: "admin-1", id: 1, upcId: -1, nome: "Admin", isAdmin: true, isAtivo: true, perfis: ["ADMIN"] };
const vinculo = { compositeKey: "link-210", id: 21, upcId: 210, nome: "Vera", isAdmin: false, isAtivo: false, perfis: ["VETERINARIO", "INATIVO"] };

const montarHook = async () => {
  const hook = renderHook(() => useUsuariosDoSistemaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  usuariosDoSistemaAPI.listar.mockResolvedValue([admin, vinculo]);
});

describe("lista", () => {
  test("ao montar, busca todos os usuários e vínculos", async () => {
    const { result } = await montarHook();

    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(1);
    expect(result.current.usuarios).toEqual([admin, vinculo]);
  });

  test("erro ao carregar mostra a mensagem e deixa a lista vazia", async () => {
    usuariosDoSistemaAPI.listar.mockRejectedValue(new Error("Falha ao buscar usuários"));

    const { result } = await montarHook();

    expect(message.error).toHaveBeenCalledWith("Falha ao buscar usuários");
    expect(result.current.usuarios).toEqual([]);
  });
});

describe("cadastro", () => {
  test("envia os valores, mostra sucesso, fecha e recarrega", async () => {
    usuariosDoSistemaAPI.cadastrar.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleAbrirModalCadastro());
    const valores = { nome: "Novo", email: "novo@teste.local", senha: "Senha@123", perfis: ["TECNICO"], clinicaIds: [4], isAtivo: true };

    await act(() => result.current.handleFormFinish(valores));

    expect(usuariosDoSistemaAPI.cadastrar).toHaveBeenCalledWith(valores);
    expect(message.success).toHaveBeenCalledWith("Usuário cadastrado com sucesso!");
    expect(result.current.isModalVisible).toBe(false);
    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("erro do backend mostra a mensagem e mantém o modal", async () => {
    usuariosDoSistemaAPI.cadastrar.mockRejectedValue(new Error("O perfil ADMIN não pode ser combinado com perfis da clínica."));
    const { result } = await montarHook();
    act(() => result.current.handleAbrirModalCadastro());

    await act(() => result.current.handleFormFinish({ perfis: ["ADMIN", "GESTOR"] }));

    expect(message.error).toHaveBeenCalledWith("O perfil ADMIN não pode ser combinado com perfis da clínica.");
    expect(result.current.isModalVisible).toBe(true);
    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(1);
  });
});

describe("edição e switch de status", () => {
  test("vínculo: envia a linha com os valores novos e perfis sem INATIVO", async () => {
    usuariosDoSistemaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vinculo));

    await act(() => result.current.handleFormFinish({ nome: "Vera Lúcia", perfis: ["VETERINARIO", "INATIVO", "GESTOR"], isAtivo: false }));

    expect(usuariosDoSistemaAPI.editar).toHaveBeenCalledWith({ ...vinculo, nome: "Vera Lúcia", perfis: ["VETERINARIO", "GESTOR"], isAtivo: false });
    expect(usuariosDoSistemaAPI.setStatus).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Usuário atualizado com sucesso!");
  });

  test("admin: os perfis vão como estão (ADMIN não é filtrado)", async () => {
    usuariosDoSistemaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(admin));

    await act(() => result.current.handleFormFinish({ nome: "Admin 2", isAtivo: true }));

    expect(usuariosDoSistemaAPI.editar).toHaveBeenCalledWith({ ...admin, nome: "Admin 2", isAtivo: true });
  });

  test("switch ligado num vínculo inativo: salva e depois ativa pela rota de status", async () => {
    usuariosDoSistemaAPI.editar.mockResolvedValue({});
    usuariosDoSistemaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vinculo));

    await act(() => result.current.handleFormFinish({ nome: "Vera", perfis: ["VETERINARIO"], isAtivo: true }));

    expect(usuariosDoSistemaAPI.setStatus).toHaveBeenCalledWith(vinculo, true);
    expect(usuariosDoSistemaAPI.editar.mock.invocationCallOrder[0]).toBeLessThan(usuariosDoSistemaAPI.setStatus.mock.invocationCallOrder[0]);
    expect(message.success).toHaveBeenCalledWith("Usuário atualizado e ativado com sucesso!");
    expect(result.current.isModalVisible).toBe(false);
    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("switch desligado num admin: inativa pela rota de status", async () => {
    usuariosDoSistemaAPI.editar.mockResolvedValue({});
    usuariosDoSistemaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(admin));

    await act(() => result.current.handleFormFinish({ nome: "Admin", isAtivo: false }));

    expect(usuariosDoSistemaAPI.setStatus).toHaveBeenCalledWith(admin, false);
    expect(message.success).toHaveBeenCalledWith("Usuário atualizado e inativado com sucesso!");
  });

  test("status recusado: mostra o erro, mantém o modal e recarrega a lista (os dados já foram salvos)", async () => {
    usuariosDoSistemaAPI.editar.mockResolvedValue({});
    usuariosDoSistemaAPI.setStatus.mockRejectedValue(new Error("Vínculo do usuário não encontrado."));
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vinculo));

    await act(() => result.current.handleFormFinish({ nome: "Vera", perfis: ["VETERINARIO"], isAtivo: true }));

    expect(message.error).toHaveBeenCalledWith("Vínculo do usuário não encontrado.");
    expect(result.current.isModalVisible).toBe(true);
    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("perfil inválido na edição: mostra o erro e não mexe no status", async () => {
    usuariosDoSistemaAPI.editar.mockRejectedValue(new Error("Perfil inválido: DONO. Use GESTOR, VETERINARIO, SECRETARIO ou TECNICO."));
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vinculo));

    await act(() => result.current.handleFormFinish({ perfis: ["DONO"], isAtivo: true }));

    expect(message.error).toHaveBeenCalledWith("Perfil inválido: DONO. Use GESTOR, VETERINARIO, SECRETARIO ou TECNICO.");
    expect(usuariosDoSistemaAPI.setStatus).not.toHaveBeenCalled();
  });
});

describe("botão de status da tabela", () => {
  test("inverte o status da linha e recarrega", async () => {
    usuariosDoSistemaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(vinculo));

    expect(usuariosDoSistemaAPI.setStatus).toHaveBeenCalledWith(vinculo, true);
    expect(message.success).toHaveBeenCalledWith("Usuário ativado com sucesso!");
    expect(usuariosDoSistemaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("erro mostra a mensagem do backend", async () => {
    usuariosDoSistemaAPI.setStatus.mockRejectedValue(new Error("Usuário administrador não encontrado."));
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(admin));

    expect(usuariosDoSistemaAPI.setStatus).toHaveBeenCalledWith(admin, false);
    expect(message.error).toHaveBeenCalledWith("Usuário administrador não encontrado.");
    expect(result.current.loadingRowKey).toBeNull();
  });
});
