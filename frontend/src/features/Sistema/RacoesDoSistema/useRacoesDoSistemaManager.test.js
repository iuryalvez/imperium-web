import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { racoesAPI } from "../../../api/sistema/racoesAPI";
import { useRacoesDoSistemaManager } from "./useRacoesDoSistemaManager";

jest.mock("../../../api/sistema/racoesAPI", () => ({
  racoesAPI: {
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

const padrao = { id: 2, nome: "Padrão", especie: "Cao", isSistema: true, isAtivo: true };
const copia = { id: 60, nome: "Padrão da clínica", nomeClinica: "Clínica Bem-Estar", isSistema: false, isAtivo: false };
const valores = { nome: "Senior", especie: 1, isPorcentagem: true, umidade: 10, proteinaBruta: 25, extratoEtereo: 14, materiaMineral: 7, fibraBruta: 3 };

const montarHook = async () => {
  const hook = renderHook(() => useRacoesDoSistemaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  racoesAPI.listar.mockResolvedValue({ itens: [padrao], total: 1, pagina: 1, tamanhoPagina: 10 });
  racoesAPI.copias.mockResolvedValue({ itens: [copia], total: 1, pagina: 1, tamanhoPagina: 10 });
});

describe("catálogo e cópias", () => {
  test("ao montar, lista o catálogo do sistema", async () => {
    const { result } = await montarHook();

    expect(racoesAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.racoes).toEqual([padrao]);
  });

  test("alternar a visão lista as cópias das clínicas", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleAlternarVisao());

    await waitFor(() => expect(result.current.racoes).toEqual([copia]));
    expect(result.current.visao).toBe("copias");
    expect(racoesAPI.copias).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
  });

  test("alternar a visão estando na página 2 volta para a página 1", async () => {
    const { result } = await montarHook();
    act(() => result.current.handleTableChange({ current: 2, pageSize: 10 }, {}, {}));
    await waitFor(() => expect(racoesAPI.listar).toHaveBeenLastCalledWith({ pagina: 2, tamanhoPagina: 10 }));

    act(() => result.current.handleAlternarVisao());

    await waitFor(() => expect(racoesAPI.copias).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 }));
    expect(result.current.paginacao.current).toBe(1);
  });

  test("busca vai para o servidor depois do debounce", async () => {
    const { result } = await montarHook();

    act(() => result.current.setBusca("padrão"));

    await waitFor(() => expect(racoesAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, busca: "padrão" }));
  });
});

describe("cadastro e edição", () => {
  test("cadastro envia os valores, mostra a mensagem do backend, fecha e recarrega", async () => {
    racoesAPI.cadastrar.mockResolvedValue({ message: "Ração cadastrada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valores));

    expect(racoesAPI.cadastrar).toHaveBeenCalledWith(valores);
    expect(message.success).toHaveBeenCalledWith("Ração cadastrada com sucesso.");
    expect(result.current.isModalOpen).toBe(false);
    expect(racoesAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia a ração editada com os valores novos", async () => {
    racoesAPI.editar.mockResolvedValue({ message: "Ração atualizada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarRacao(padrao));

    await act(() => result.current.handleSubmit({ nome: "Padrão 2" }));

    expect(racoesAPI.editar).toHaveBeenCalledWith({ ...padrao, nome: "Padrão 2" });
  });

  test("erro do backend mostra a mensagem e mantém o modal aberto", async () => {
    racoesAPI.cadastrar.mockRejectedValue({ message: "A espécie é inválida." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("A espécie é inválida.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
  });
});

describe("ações da linha", () => {
  test("excluir ração em uso (409) mostra a mensagem do backend e não recarrega", async () => {
    racoesAPI.deletar.mockRejectedValue({ message: "Esta ração está em uso por dietas ou avaliações e não pode ser excluída. Desative-a." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(padrao));

    expect(racoesAPI.deletar).toHaveBeenCalledWith(2);
    expect(message.error).toHaveBeenCalledWith("Esta ração está em uso por dietas ou avaliações e não pode ser excluída. Desative-a.");
    expect(racoesAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("status inverte o atual e recarrega", async () => {
    racoesAPI.setStatus.mockResolvedValue({ message: "Ração ativada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus({ ...padrao, isAtivo: false }));

    expect(racoesAPI.setStatus).toHaveBeenCalledWith(2, true);
    expect(message.success).toHaveBeenCalledWith("Ração ativada com sucesso.");
    expect(racoesAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("modais", () => {
  test("detalhes guarda a ração; fechar limpa tudo", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenModalDetalhes(padrao));
    expect(result.current.isModalDetalhesOpen).toBe(true);
    expect(result.current.viewingRacao).toEqual(padrao);

    act(() => result.current.handleCloseModals());
    expect(result.current.viewingRacao).toBeNull();
  });
});
