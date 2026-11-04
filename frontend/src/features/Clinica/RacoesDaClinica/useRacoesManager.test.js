import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { racoesDaClinicaAPI } from "../../../api/clinica/racoesDaClinicaAPI";
import { useRacoesManager } from "./useRacoesManager";

jest.mock("../../../api/clinica/racoesDaClinicaAPI", () => ({
  racoesDaClinicaAPI: {
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

const racaoPropria = { id: 8, nome: "Premium", especie: "Cao", isSistema: false, isAtivo: true, isOculta: false };
const racaoSistema = { id: 2, nome: "Padrão", especie: "Cao", isSistema: true, isAtivo: true, isOculta: false };
const valores = { nome: "Light", especie: 1, isPorcentagem: true, umidade: 10, proteinaBruta: 25, extratoEtereo: 14, materiaMineral: 7, fibraBruta: 3 };

const montarHook = async () => {
  const hook = renderHook(() => useRacoesManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  racoesDaClinicaAPI.listar.mockResolvedValue({ itens: [racaoSistema, racaoPropria], total: 2, pagina: 1, tamanhoPagina: 10 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página e expõe as rações", async () => {
    const { result } = await montarHook();

    expect(racoesDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.racoes).toEqual([racaoSistema, racaoPropria]);
  });

  test("filtros de espécie e status da tabela e ordenação vão para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { especie: [2], isAtivo: [false] }, { columnKey: "nome", order: "descend" }));

    await waitFor(() =>
      expect(racoesDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, especie: 2, ativo: false, ordenarPor: "nome", ordem: "desc" })
    );
  });

  test("filtro de origem pelo setFiltros", async () => {
    const { result } = await montarHook();

    act(() => result.current.setFiltros({ origem: "sistema" }));

    await waitFor(() => expect(racoesDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, origem: "sistema" }));
  });
});

describe("cadastro e edição", () => {
  test("cadastro envia os valores, mostra a mensagem do backend, fecha e recarrega", async () => {
    racoesDaClinicaAPI.cadastrar.mockResolvedValue({ message: "Ração cadastrada com sucesso." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    expect(racoesDaClinicaAPI.cadastrar).toHaveBeenCalledWith(valores);
    expect(message.success).toHaveBeenCalledWith("Ração cadastrada com sucesso.");
    expect(result.current.isModalOpen).toBe(false);
    expect(racoesDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia a ração editada com os valores novos", async () => {
    racoesDaClinicaAPI.editar.mockResolvedValue({ message: "Ração do sistema copiada para a clínica com as alterações." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal(racaoSistema));

    await act(() => result.current.handleSubmit({ nome: "Padrão da clínica" }));

    expect(racoesDaClinicaAPI.editar).toHaveBeenCalledWith({ ...racaoSistema, nome: "Padrão da clínica" });
    expect(racoesDaClinicaAPI.cadastrar).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Ração do sistema copiada para a clínica com as alterações.");
  });

  test("erro do backend mostra a mensagem e mantém o modal aberto", async () => {
    racoesDaClinicaAPI.cadastrar.mockRejectedValue({ message: "A espécie é inválida." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("A espécie é inválida.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(racoesDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("erro sem mensagem usa o texto padrão", async () => {
    racoesDaClinicaAPI.cadastrar.mockRejectedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Falha na operação.");
  });
});

describe("ações da linha", () => {
  test("excluir chama deletar(id) e recarrega", async () => {
    racoesDaClinicaAPI.deletar.mockResolvedValue({ message: "Ração excluída com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(racaoPropria));

    expect(racoesDaClinicaAPI.deletar).toHaveBeenCalledWith(8);
    expect(message.success).toHaveBeenCalledWith("Ração excluída com sucesso.");
    expect(racoesDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("excluir ração em uso mostra a mensagem do backend e não recarrega", async () => {
    racoesDaClinicaAPI.deletar.mockRejectedValue({ message: "Esta ração está em uso por dietas ou avaliações e não pode ser excluída. Desative-a." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(racaoPropria));

    expect(message.error).toHaveBeenCalledWith("Esta ração está em uso por dietas ou avaliações e não pode ser excluída. Desative-a.");
    expect(racoesDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("status inverte o atual", async () => {
    racoesDaClinicaAPI.setStatus.mockResolvedValue({ message: "Ração desativada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(racaoPropria));

    expect(racoesDaClinicaAPI.setStatus).toHaveBeenCalledWith(8, false);
    expect(message.success).toHaveBeenCalledWith("Ração desativada com sucesso.");
  });

  test("ocultar inverte o atual e mostra o erro do backend quando falha", async () => {
    racoesDaClinicaAPI.ocultar.mockRejectedValue({ message: "Só rações do sistema podem ser ocultadas. Para rações da clínica, use desativar." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleOculta(racaoPropria));

    expect(racoesDaClinicaAPI.ocultar).toHaveBeenCalledWith(8, true);
    expect(message.error).toHaveBeenCalledWith("Só rações do sistema podem ser ocultadas. Para rações da clínica, use desativar.");
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("modais", () => {
  test("detalhes guarda e limpa a ração visualizada", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenViewModal(racaoPropria));
    expect(result.current.isViewModalOpen).toBe(true);
    expect(result.current.viewingRacao).toEqual(racaoPropria);

    act(() => result.current.handleCloseViewModal());
    expect(result.current.viewingRacao).toBeNull();
  });
});
