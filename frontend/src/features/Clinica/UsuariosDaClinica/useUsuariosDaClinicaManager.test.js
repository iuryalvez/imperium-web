import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";
import { useUsuariosDaClinicaManager } from "./useUsuariosDaClinicaManager";

jest.mock("../../../api/clinica/usuariosDaClinicaAPI.js", () => ({
  usuariosDaClinicaAPI: {
    listar: jest.fn(),
    cadastrar: jest.fn(),
    editar: jest.fn(),
    setStatus: jest.fn(),
  },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const vet = { id: 21, upcId: 210, nome: "Vera", email: "vera@teste.local", perfis: ["VETERINARIO"], isAtivoNaClinica: true };
const tecnicoInativo = { id: 22, upcId: 220, nome: "Téo", perfis: ["TECNICO", "INATIVO"], isAtivoNaClinica: false };

const montarHook = async () => {
  const hook = renderHook(() => useUsuariosDaClinicaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  usuariosDaClinicaAPI.listar.mockResolvedValue({ itens: [vet, tecnicoInativo], total: 2, pagina: 1, tamanhoPagina: 10 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página e expõe os usuários", async () => {
    const { result } = await montarHook();

    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.usuarios).toEqual([vet, tecnicoInativo]);
  });

  test("filtros de perfil e status da coluna viram 'perfil' e 'ativo'", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { perfis: ["TECNICO"], isAtivoNaClinica: [false] }, {}));

    await waitFor(() =>
      expect(usuariosDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, perfil: "TECNICO", ativo: false })
    );
  });
});

describe("cadastro", () => {
  const valores = { nome: "Nova", email: "nova@teste.local", senha: "Senha@123", perfis: ["SECRETARIO"], isAtivo: true };

  test("envia os valores do formulário, mostra sucesso, fecha e recarrega", async () => {
    usuariosDaClinicaAPI.cadastrar.mockResolvedValue({ succeeded: true, data: { usuarioId: 30 } });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valores));

    expect(usuariosDaClinicaAPI.cadastrar).toHaveBeenCalledWith(valores);
    expect(message.success).toHaveBeenCalledWith("Usuário cadastrado com sucesso!");
    expect(result.current.isModalOpen).toBe(false);
    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("e-mail que já existia no sistema: mostra a mensagem de vínculo do backend", async () => {
    usuariosDaClinicaAPI.cadastrar.mockResolvedValue({
      succeeded: true,
      message: "Este e-mail já tinha cadastro no sistema. O usuário foi vinculado à clínica com os perfis escolhidos.",
      data: { usuarioId: 9, vinculado: true },
    });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(message.success).toHaveBeenCalledWith("Este e-mail já tinha cadastro no sistema. O usuário foi vinculado à clínica com os perfis escolhidos.");
  });

  test("erro do backend mostra a mensagem, mantém o modal e não recarrega", async () => {
    usuariosDaClinicaAPI.cadastrar.mockRejectedValue({ message: "Este usuário já está vinculado a esta clínica." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Este usuário já está vinculado a esta clínica.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });
});

describe("edição de perfis e switch de status", () => {
  test("envia só upcId e perfis, sem INATIVO", async () => {
    usuariosDaClinicaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(tecnicoInativo));

    await act(() => result.current.handleSubmit({ perfis: ["TECNICO", "INATIVO", "VETERINARIO"], isAtivoNaClinica: false }));

    expect(usuariosDaClinicaAPI.editar).toHaveBeenCalledWith({ upcId: 220, perfis: ["TECNICO", "VETERINARIO"] });
    expect(usuariosDaClinicaAPI.cadastrar).not.toHaveBeenCalled();
  });

  test("switch sem mudança: não chama setStatus", async () => {
    usuariosDaClinicaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vet));

    await act(() => result.current.handleSubmit({ perfis: ["VETERINARIO", "SECRETARIO"], isAtivoNaClinica: true }));

    expect(usuariosDaClinicaAPI.setStatus).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Perfis do usuário atualizados com sucesso!");
    expect(result.current.isModalOpen).toBe(false);
    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("switch desligado: salva os perfis e depois inativa o vínculo pela rota de status", async () => {
    usuariosDaClinicaAPI.editar.mockResolvedValue({});
    usuariosDaClinicaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vet));

    await act(() => result.current.handleSubmit({ perfis: ["VETERINARIO"], isAtivoNaClinica: false }));

    expect(usuariosDaClinicaAPI.setStatus).toHaveBeenCalledWith(vet, false);
    expect(usuariosDaClinicaAPI.editar.mock.invocationCallOrder[0]).toBeLessThan(usuariosDaClinicaAPI.setStatus.mock.invocationCallOrder[0]);
    expect(message.success).toHaveBeenCalledWith("Usuário atualizado e inativado com sucesso!");
  });

  test("switch ligado num vínculo inativo: reativa", async () => {
    usuariosDaClinicaAPI.editar.mockResolvedValue({});
    usuariosDaClinicaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(tecnicoInativo));

    await act(() => result.current.handleSubmit({ perfis: ["TECNICO"], isAtivoNaClinica: true }));

    expect(usuariosDaClinicaAPI.setStatus).toHaveBeenCalledWith(tecnicoInativo, true);
    expect(message.success).toHaveBeenCalledWith("Usuário atualizado e ativado com sucesso!");
  });

  test("status recusado pelo backend (próprio vínculo): mostra o erro, mantém o modal e recarrega a lista", async () => {
    usuariosDaClinicaAPI.editar.mockResolvedValue({});
    usuariosDaClinicaAPI.setStatus.mockRejectedValue({ message: "Você não pode desativar o seu próprio vínculo." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vet));

    await act(() => result.current.handleSubmit({ perfis: ["VETERINARIO"], isAtivoNaClinica: false }));

    expect(message.error).toHaveBeenCalledWith("Você não pode desativar o seu próprio vínculo.");
    expect(message.success).not.toHaveBeenCalled();
    expect(result.current.isModalOpen).toBe(true);
    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("erro na edição dos perfis: não tenta mudar o status", async () => {
    usuariosDaClinicaAPI.editar.mockRejectedValue({ message: "Você não pode remover o seu próprio perfil de GESTOR." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarUsuario(vet));

    await act(() => result.current.handleSubmit({ perfis: ["SECRETARIO"], isAtivoNaClinica: false }));

    expect(message.error).toHaveBeenCalledWith("Você não pode remover o seu próprio perfil de GESTOR.");
    expect(usuariosDaClinicaAPI.setStatus).not.toHaveBeenCalled();
  });
});

describe("botão de status da tabela", () => {
  test("inverte o status do vínculo, mostra sucesso e recarrega", async () => {
    usuariosDaClinicaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(vet));

    expect(usuariosDaClinicaAPI.setStatus).toHaveBeenCalledWith(vet, false);
    expect(message.success).toHaveBeenCalledWith("Usuário inativado com sucesso!");
    expect(usuariosDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("erro do backend mostra a mensagem e libera a linha", async () => {
    usuariosDaClinicaAPI.setStatus.mockRejectedValue({ message: "Vínculo do usuário não encontrado nesta clínica." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(tecnicoInativo));

    expect(usuariosDaClinicaAPI.setStatus).toHaveBeenCalledWith(tecnicoInativo, true);
    expect(message.error).toHaveBeenCalledWith("Vínculo do usuário não encontrado nesta clínica.");
    expect(result.current.loadingRowKey).toBeNull();
  });
});
