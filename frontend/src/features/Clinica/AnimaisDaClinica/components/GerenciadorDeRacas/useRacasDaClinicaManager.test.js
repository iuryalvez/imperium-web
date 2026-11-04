import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { racasDaClinicaAPI } from "../../../../../api/clinica/racasDaClinicaAPI";
import { useRacasDaClinicaManager } from "./useRacasDaClinicaManager";

jest.mock("../../../../../api/clinica/racasDaClinicaAPI", () => ({
  racasDaClinicaAPI: {
    listar: jest.fn(),
    cadastrar: jest.fn(),
    editar: jest.fn(),
    deletar: jest.fn(),
    setStatus: jest.fn(),
    ocultar: jest.fn(),
  },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const racaPropria = { id: 5, nome: "Vira-lata", especie: "Cao", isSistema: false, isAtivo: true, isOculta: false };
const racaSistema = { id: 1, nome: "Labrador", especie: "Cao", isSistema: true, isAtivo: true, isOculta: false };

const montarHook = async () => {
  const hook = renderHook(() => useRacasDaClinicaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  racasDaClinicaAPI.listar.mockResolvedValue({ itens: [racaSistema, racaPropria], total: 2, pagina: 1, tamanhoPagina: 12 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página com 12 itens (grade de cards)", async () => {
    const { result } = await montarHook();

    expect(racasDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 12 });
    expect(result.current.racas).toEqual([racaSistema, racaPropria]);
  });

  test("setPagina troca página e tamanho", async () => {
    const { result } = await montarHook();

    act(() => result.current.setPagina(2, 24));

    await waitFor(() => expect(racasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 2, tamanhoPagina: 24 }));
  });

  test.each([
    ["ativos", { ativo: true }],
    ["inativos", { ativo: false }],
    ["ocultas", { oculta: true }],
    ["visiveis", { oculta: false }],
  ])("filtro de status '%s' vira %o na página 1", async (status, esperado) => {
    const { result } = await montarHook();

    act(() => result.current.handleFiltroStatus(status));

    await waitFor(() =>
      expect(racasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 12, ...esperado })
    );
  });

  test("trocar de 'ocultas' para 'todas' tira os dois filtros", async () => {
    const { result } = await montarHook();
    act(() => result.current.handleFiltroStatus("ocultas"));
    await waitFor(() => expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(2));

    act(() => result.current.handleFiltroStatus("todas"));

    await waitFor(() => expect(racasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 12 }));
  });

  test("filtro de espécie e origem pelo setFiltros", async () => {
    const { result } = await montarHook();

    act(() => result.current.setFiltros({ especie: 2, origem: "clinica" }));

    await waitFor(() =>
      expect(racasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 12, especie: 2, origem: "clinica" })
    );
  });
});

describe("cadastro e edição", () => {
  test("cadastro envia os valores e mostra a mensagem do backend", async () => {
    racasDaClinicaAPI.cadastrar.mockResolvedValue({ succeeded: true, message: "Raça cadastrada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit({ nome: "Caramelo", especie: 1 }));

    expect(racasDaClinicaAPI.cadastrar).toHaveBeenCalledWith({ nome: "Caramelo", especie: 1 });
    expect(message.success).toHaveBeenCalledWith("Raça cadastrada com sucesso.");
    expect(result.current.isModalOpen).toBe(false);
    expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("editar raça do sistema envia a raça com os valores novos e mostra o aviso de cópia", async () => {
    racasDaClinicaAPI.editar.mockResolvedValue({ succeeded: true, message: "Raça do sistema copiada para a clínica com as alterações." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal(racaSistema));

    await act(() => result.current.handleSubmit({ nome: "Labrador da clínica", especie: 1 }));

    expect(racasDaClinicaAPI.editar).toHaveBeenCalledWith({ ...racaSistema, nome: "Labrador da clínica", especie: 1 });
    expect(message.success).toHaveBeenCalledWith("Raça do sistema copiada para a clínica com as alterações.");
  });

  test("sucesso sem mensagem usa o texto padrão", async () => {
    racasDaClinicaAPI.cadastrar.mockResolvedValue(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit({ nome: "X", especie: 1 }));

    expect(message.success).toHaveBeenCalledWith("Raça salva com sucesso!");
  });

  test("erro do backend mostra a mensagem, mantém o modal e não recarrega", async () => {
    racasDaClinicaAPI.cadastrar.mockRejectedValue({ message: "O nome da raça é obrigatório." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit({ especie: 1 }));

    expect(message.error).toHaveBeenCalledWith("O nome da raça é obrigatório.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });
});

describe("ações do card", () => {
  test("excluir chama deletar(id), mostra a mensagem e recarrega", async () => {
    racasDaClinicaAPI.deletar.mockResolvedValue({ message: "Raça excluída com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(racaPropria));

    expect(racasDaClinicaAPI.deletar).toHaveBeenCalledWith(5);
    expect(message.success).toHaveBeenCalledWith("Raça excluída com sucesso.");
    expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingItemId).toBeNull();
  });

  test("excluir raça em uso mostra a mensagem do backend (409) e não recarrega", async () => {
    racasDaClinicaAPI.deletar.mockRejectedValue({ message: "Esta raça está em uso por animais e não pode ser excluída. Desative-a." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(racaPropria));

    expect(message.error).toHaveBeenCalledWith("Esta raça está em uso por animais e não pode ser excluída. Desative-a.");
    expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
    expect(result.current.loadingItemId).toBeNull();
  });

  test("status inverte o atual: ativa vira setStatus(id, false)", async () => {
    racasDaClinicaAPI.setStatus.mockResolvedValue({ message: "Raça desativada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(racaPropria));

    expect(racasDaClinicaAPI.setStatus).toHaveBeenCalledWith(5, false);
    expect(message.success).toHaveBeenCalledWith("Raça desativada com sucesso.");
  });

  test("ocultar inverte o atual: visível vira ocultar(id, true)", async () => {
    racasDaClinicaAPI.ocultar.mockResolvedValue({ message: "Raça ocultada para esta clínica." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleOculta(racaSistema));

    expect(racasDaClinicaAPI.ocultar).toHaveBeenCalledWith(1, true);
    expect(message.success).toHaveBeenCalledWith("Raça ocultada para esta clínica.");
    expect(racasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("erro sem mensagem usa o texto padrão", async () => {
    racasDaClinicaAPI.ocultar.mockRejectedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleToggleOculta({ ...racaSistema, isOculta: true }));

    expect(racasDaClinicaAPI.ocultar).toHaveBeenCalledWith(1, false);
    expect(message.error).toHaveBeenCalledWith("Erro ao executar a operação.");
  });
});
