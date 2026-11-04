import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { animaisDaClinicaAPI } from "../../../api/clinica/animaisDaClinicaAPI";
import { useAnimaisDaClinicaManager } from "./useAnimaisDaClinicaManager";

jest.mock("../../../api/clinica/animaisDaClinicaAPI", () => ({
  animaisDaClinicaAPI: {
    listar: jest.fn(),
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

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

const rex = { id: 3, nome: "Rex", especie: 1, racaId: 10, isAtivo: true, acompanhamentoId: 30 };

const montarHook = async () => {
  const hook = renderHook(() => useAnimaisDaClinicaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  animaisDaClinicaAPI.listar.mockResolvedValue({ itens: [rex], total: 1, pagina: 1, tamanhoPagina: 10 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página e expõe os animais", async () => {
    const { result } = await montarHook();

    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.animais).toEqual([rex]);
    expect(result.current.paginacao.total).toBe(1);
  });

  test("filtros de espécie e status da tabela viram 'especie' e 'ativo'", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { especie: [2], isAtivo: [true] }, {}));

    await waitFor(() =>
      expect(animaisDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, especie: 2, ativo: true })
    );
  });

  test("busca por texto vai para o servidor depois do debounce", async () => {
    const { result } = await montarHook();

    act(() => result.current.setBusca("rex"));

    await waitFor(() =>
      expect(animaisDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, busca: "rex" })
    );
  });
});

describe("cadastro e edição", () => {
  const valores = { nome: "Luna", especie: 2, racaId: 11, sexo: 2, isCastrado: false, tutorIds: [1], sobTutelaClinica: false };

  test("cadastro não envia o campo de tela 'sobTutelaClinica'", async () => {
    animaisDaClinicaAPI.cadastrar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    expect(animaisDaClinicaAPI.cadastrar).toHaveBeenCalledWith({ nome: "Luna", especie: 2, racaId: 11, sexo: 2, isCastrado: false, tutorIds: [1] });
    expect(message.success).toHaveBeenCalledWith("Animal cadastrado com sucesso!");
    expect(result.current.isModalAnimalOpen).toBe(false);
    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia o animal editado com os valores novos pelo editar", async () => {
    animaisDaClinicaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal(rex));

    await act(() => result.current.handleSubmit({ ...valores, nome: "Rex II" }));

    expect(animaisDaClinicaAPI.editar).toHaveBeenCalledWith(expect.objectContaining({ id: 3, nome: "Rex II", acompanhamentoId: 30 }));
    expect(animaisDaClinicaAPI.editar.mock.calls[0][0]).not.toHaveProperty("sobTutelaClinica");
    expect(animaisDaClinicaAPI.cadastrar).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Animal atualizado com sucesso!");
  });

  test("erro do backend mostra a mensagem dele e mantém o modal aberto", async () => {
    animaisDaClinicaAPI.cadastrar.mockRejectedValue({ message: "A raça escolhida não pertence à espécie do animal." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("A raça escolhida não pertence à espécie do animal.");
    expect(result.current.isModalAnimalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("erro sem mensagem usa o texto padrão", async () => {
    animaisDaClinicaAPI.cadastrar.mockRejectedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Falha ao salvar o animal.");
  });
});

describe("excluir e status", () => {
  test("excluir chama deletar(id), mostra sucesso e recarrega", async () => {
    animaisDaClinicaAPI.deletar.mockResolvedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(3));

    expect(animaisDaClinicaAPI.deletar).toHaveBeenCalledWith(3);
    expect(message.success).toHaveBeenCalledWith("Animal deletado com sucesso!");
    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("excluir animal com consultas mostra a mensagem do backend e não recarrega", async () => {
    animaisDaClinicaAPI.deletar.mockRejectedValue({ message: "Este animal possui consultas registradas e não pode ser excluído. Desative-o." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(3));

    expect(message.error).toHaveBeenCalledWith("Este animal possui consultas registradas e não pode ser excluído. Desative-o.");
    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("status: animal ativo é desativado com setStatus(id, false) e recarrega", async () => {
    animaisDaClinicaAPI.setStatus.mockResolvedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(rex));

    expect(animaisDaClinicaAPI.setStatus).toHaveBeenCalledWith(3, false);
    expect(message.success).toHaveBeenCalledWith("Animal desativado com sucesso!");
    expect(animaisDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("status: erro mostra a mensagem do backend e libera a linha", async () => {
    animaisDaClinicaAPI.setStatus.mockRejectedValue({ message: "Animal não encontrado nesta clínica." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus({ ...rex, isAtivo: false }));

    expect(animaisDaClinicaAPI.setStatus).toHaveBeenCalledWith(3, true);
    expect(message.error).toHaveBeenCalledWith("Animal não encontrado nesta clínica.");
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("navegação e modais", () => {
  test("abre o acompanhamento do animal pela rota da clínica", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleNavigateToAcompanhamento(30));

    expect(mockNavigate).toHaveBeenCalledWith("/clinica/animais/acompanhamento/30");
  });

  test("modais de raças e detalhes abrem e fecham", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenModalRacas());
    expect(result.current.isModalRacasOpen).toBe(true);
    act(() => result.current.handleCloseModalRacas());
    expect(result.current.isModalRacasOpen).toBe(false);

    act(() => result.current.handleOpenModalDetalhes(rex));
    expect(result.current.isModalDetalhesOpen).toBe(true);
    expect(result.current.viewingAnimal).toEqual(rex);
  });
});
