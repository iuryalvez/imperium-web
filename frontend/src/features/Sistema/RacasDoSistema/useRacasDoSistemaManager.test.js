import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { racasAPI } from "../../../api/sistema/racasAPI";
import { useRacasDoSistemaManager } from "./useRacasDoSistemaManager";

jest.mock("../../../api/sistema/racasAPI", () => ({
  racasAPI: {
    listar: jest.fn(),
    copias: jest.fn(),
    cadastrar: jest.fn(),
    editar: jest.fn(),
    deletar: jest.fn(),
    setStatus: jest.fn(),
  },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const labrador = { id: 1, nome: "Labrador", especie: "Cao", isSistema: true, isAtivo: true };
const copia = { id: 50, nome: "Labrador da clínica", nomeClinica: "Clínica Bem-Estar", isSistema: false, isAtivo: true };

const montarHook = async () => {
  const hook = renderHook(() => useRacasDoSistemaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  racasAPI.listar.mockResolvedValue({ itens: [labrador], total: 1, pagina: 1, tamanhoPagina: 10 });
  racasAPI.copias.mockResolvedValue({ itens: [copia], total: 1, pagina: 1, tamanhoPagina: 10 });
});

describe("catálogo e cópias", () => {
  test("ao montar, lista o catálogo do sistema", async () => {
    const { result } = await montarHook();

    expect(result.current.visao).toBe("sistema");
    expect(racasAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(racasAPI.copias).not.toHaveBeenCalled();
    expect(result.current.racas).toEqual([labrador]);
  });

  test("alternar a visão lista as cópias das clínicas e volta para o catálogo", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleAlternarVisao());
    await waitFor(() => expect(result.current.racas).toEqual([copia]));
    expect(result.current.visao).toBe("copias");
    expect(racasAPI.copias).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });

    act(() => result.current.handleAlternarVisao());
    await waitFor(() => expect(result.current.racas).toEqual([labrador]));
    expect(result.current.visao).toBe("sistema");
  });

  test("alternar a visão estando na página 2 volta para a página 1", async () => {
    const { result } = await montarHook();
    act(() => result.current.handleTableChange({ current: 2, pageSize: 10 }, {}, {}));
    await waitFor(() => expect(racasAPI.listar).toHaveBeenLastCalledWith({ pagina: 2, tamanhoPagina: 10 }));

    act(() => result.current.handleAlternarVisao());

    await waitFor(() => expect(racasAPI.copias).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 }));
    expect(result.current.paginacao.current).toBe(1);
  });

  test("filtros de espécie e status da coluna vão para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { especie: [1], isAtivo: [true] }, {}));

    await waitFor(() => expect(racasAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, especie: 1, ativo: true }));
  });
});

describe("cadastro e edição", () => {
  test("cadastro envia os valores e mostra a mensagem do backend", async () => {
    racasAPI.cadastrar.mockResolvedValue({ message: "Raça cadastrada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit({ nome: "Beagle", especie: 1 }));

    expect(racasAPI.cadastrar).toHaveBeenCalledWith({ nome: "Beagle", especie: 1 });
    expect(message.success).toHaveBeenCalledWith("Raça cadastrada com sucesso.");
    expect(result.current.isModalOpen).toBe(false);
    expect(racasAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia a raça editada com os valores novos", async () => {
    racasAPI.editar.mockResolvedValue({ message: "Raça atualizada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarRaca(labrador));

    await act(() => result.current.handleSubmit({ nome: "Labrador Retriever", especie: 1 }));

    expect(racasAPI.editar).toHaveBeenCalledWith({ ...labrador, nome: "Labrador Retriever", especie: 1 });
    expect(message.success).toHaveBeenCalledWith("Raça atualizada com sucesso.");
  });

  test("erro do backend mostra a mensagem, mantém o modal e não recarrega", async () => {
    racasAPI.editar.mockRejectedValue({ message: "Raça não encontrada." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarRaca(labrador));

    await act(() => result.current.handleSubmit({ nome: "X", especie: 1 }));

    expect(message.error).toHaveBeenCalledWith("Raça não encontrada.");
    expect(result.current.isModalOpen).toBe(true);
    expect(racasAPI.listar).toHaveBeenCalledTimes(1);
  });
});

describe("ações da linha", () => {
  test("excluir chama deletar(id) e recarrega", async () => {
    racasAPI.deletar.mockResolvedValue({ message: "Raça excluída com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(labrador));

    expect(racasAPI.deletar).toHaveBeenCalledWith(1);
    expect(message.success).toHaveBeenCalledWith("Raça excluída com sucesso.");
    expect(racasAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("excluir raça em uso por animais (409) mostra a mensagem do backend", async () => {
    racasAPI.deletar.mockRejectedValue({ message: "Esta raça está em uso por animais e não pode ser excluída. Desative-a." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(labrador));

    expect(message.error).toHaveBeenCalledWith("Esta raça está em uso por animais e não pode ser excluída. Desative-a.");
    expect(result.current.loadingRowKey).toBeNull();
    expect(racasAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("status inverte o atual", async () => {
    racasAPI.setStatus.mockResolvedValue({ message: "Raça desativada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(labrador));

    expect(racasAPI.setStatus).toHaveBeenCalledWith(1, false);
    expect(message.success).toHaveBeenCalledWith("Raça desativada com sucesso.");
  });
});
