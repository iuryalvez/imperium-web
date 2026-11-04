import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { acompanhamentoAPI } from "../../../../api/clinica/acompanhamentoAPI";
import { avaliacoesDaClinicaAPI } from "../../../../api/clinica/avaliacoesDaClinicaAPI";
import { consultasDaClinicaAPI } from "../../../../api/clinica/consultasDaClinicaAPI";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";
import { useAcompanhamentoManager } from "./useAcompanhamentoManager";

jest.mock("../../../../api/clinica/acompanhamentoAPI", () => ({
  acompanhamentoAPI: {
    getHistoricoCompleto: jest.fn(),
    criarDieta: jest.fn(),
    editarDieta: jest.fn(),
    deletarDieta: jest.fn(),
    iniciarDieta: jest.fn(),
    concluirDieta: jest.fn(),
    registrarPesoDieta: jest.fn(),
    deletarPeso: jest.fn(),
  },
}));
jest.mock("../../../../api/clinica/avaliacoesDaClinicaAPI", () => ({
  avaliacoesDaClinicaAPI: { upsert: jest.fn(), deletar: jest.fn() },
}));
jest.mock("../../../../api/clinica/consultasDaClinicaAPI", () => ({
  consultasDaClinicaAPI: { editar: jest.fn(), deletar: jest.fn() },
}));
jest.mock("../../../../api/clinica/animaisDaClinicaAPI", () => ({
  animaisDaClinicaAPI: { editar: jest.fn() },
}));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useParams: () => ({ acompanhamentoId: "30" }),
}));

const dieta = { id: 90, status: 2, registrosPeso: [] };
const avaliacao = { id: 70, peso: 20, dieta };
const consultaComAvaliacao = { id: 40, consultaId: 40, avaliacao };
const consultaPendente = { id: 41, avaliacao: null };
const historico = { id: 3, nome: "Rex", acompanhamento: { id: 30 }, consultas: [consultaComAvaliacao, consultaPendente] };

const montarHook = async () => {
  const hook = renderHook(() => useAcompanhamentoManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  acompanhamentoAPI.getHistoricoCompleto.mockResolvedValue({ succeeded: true, data: historico });
});

describe("carga do histórico", () => {
  test("ao montar, busca o histórico pelo id do acompanhamento da rota", async () => {
    const { result } = await montarHook();

    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledWith("30");
    expect(result.current.historico).toEqual(historico);
  });

  test("ServiceResult sem sucesso mostra a mensagem dele", async () => {
    acompanhamentoAPI.getHistoricoCompleto.mockResolvedValue({ succeeded: false, message: "Acompanhamento não encontrado nesta clínica." });

    const { result } = await montarHook();

    expect(message.error).toHaveBeenCalledWith("Erro ao buscar histórico: Acompanhamento não encontrado nesta clínica.");
    expect(result.current.historico).toBeNull();
  });

  test("erro do backend (404) mostra a mensagem dele", async () => {
    acompanhamentoAPI.getHistoricoCompleto.mockRejectedValue({ succeeded: false, message: "Acompanhamento não encontrado nesta clínica." });

    await montarHook();

    expect(message.error).toHaveBeenCalledWith("Erro ao buscar histórico: Acompanhamento não encontrado nesta clínica.");
  });
});

describe("avaliações", () => {
  test("abrir a avaliação de uma consulta já avaliada carrega os dados dela", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenAvaliacaoModal(consultaComAvaliacao));

    expect(result.current.isAvaliacaoModalOpen).toBe(true);
    expect(result.current.avaliacaoData).toEqual(avaliacao);
    act(() => result.current.handleOpenAvaliacaoModal(consultaPendente));
    expect(result.current.avaliacaoData).toBeNull();
  });

  test("salvar chama upsert(consultaId, valores), fecha e recarrega", async () => {
    avaliacoesDaClinicaAPI.upsert.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenAvaliacaoModal(consultaPendente));

    await act(() => result.current.handleSubmitAvaliacao({ peso: 21 }));

    expect(avaliacoesDaClinicaAPI.upsert).toHaveBeenCalledWith(41, { peso: 21 });
    expect(message.success).toHaveBeenCalledWith("Avaliação salva com sucesso!");
    expect(result.current.isAvaliacaoModalOpen).toBe(false);
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);
  });

  test("sem consulta selecionada não salva", async () => {
    const { result } = await montarHook();

    await act(() => result.current.handleSubmitAvaliacao({ peso: 21 }));

    expect(avaliacoesDaClinicaAPI.upsert).not.toHaveBeenCalled();
  });

  test("erro com lista de erros junta a mensagem e os erros, e relança para o formulário", async () => {
    const erro = { message: "Dados inválidos", errors: ["Peso obrigatório", "Ração inativa"] };
    avaliacoesDaClinicaAPI.upsert.mockRejectedValue(erro);
    const { result } = await montarHook();
    act(() => result.current.handleOpenAvaliacaoModal(consultaPendente));

    await act(async () => {
      await expect(result.current.handleSubmitAvaliacao({})).rejects.toBe(erro);
    });

    expect(message.error).toHaveBeenCalledWith("Dados inválidos: Peso obrigatório, Ração inativa");
    expect(result.current.isAvaliacaoModalOpen).toBe(true);
  });

  test("excluir avaliação chama deletar(id), fecha e recarrega; erro mostra a mensagem", async () => {
    avaliacoesDaClinicaAPI.deletar.mockResolvedValueOnce(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleDeleteAvaliacao(70));

    expect(avaliacoesDaClinicaAPI.deletar).toHaveBeenCalledWith(70);
    expect(message.success).toHaveBeenCalledWith("Avaliação excluída com sucesso!");
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);

    avaliacoesDaClinicaAPI.deletar.mockRejectedValueOnce({ message: "Avaliação não encontrada." });
    await act(() => result.current.handleDeleteAvaliacao(70));
    expect(message.error).toHaveBeenCalledWith("Avaliação não encontrada.");
  });
});

describe("dietas", () => {
  test("criar envia os valores com o acompanhamento e a avaliação, fecha e recarrega", async () => {
    acompanhamentoAPI.criarDieta.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCriarDieta(avaliacao));

    await act(() => result.current.handleCriarDieta({ tipoDieta: 1, racaoId: 8 }));

    expect(acompanhamentoAPI.criarDieta).toHaveBeenCalledWith({ tipoDieta: 1, racaoId: 8, acompanhamentoId: 30, avaliacaoId: 70 });
    expect(message.success).toHaveBeenCalledWith("Dieta calculada e salva com sucesso!");
    expect(result.current.isModalCriarDietaOpen).toBe(false);
    expect(result.current.isSubmitting).toBe(false);
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);
  });

  test("criar sem avaliação selecionada não chama a api", async () => {
    const { result } = await montarHook();

    await act(() => result.current.handleCriarDieta({ tipoDieta: 1 }));

    expect(acompanhamentoAPI.criarDieta).not.toHaveBeenCalled();
  });

  test("criar recusado (ServiceResult sem sucesso) mostra a mensagem e mantém o modal", async () => {
    acompanhamentoAPI.criarDieta.mockResolvedValue({ succeeded: false, message: "A ração selecionada está inativa." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCriarDieta(avaliacao));

    await act(() => result.current.handleCriarDieta({ tipoDieta: 1, racaoId: 8 }));

    expect(message.error).toHaveBeenCalledWith("A ração selecionada está inativa.");
    expect(result.current.isModalCriarDietaOpen).toBe(true);
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(1);
  });

  test("criar com erro do backend mostra a mensagem dele", async () => {
    acompanhamentoAPI.criarDieta.mockRejectedValue({ message: "A ração selecionada não existe ou não pertence a esta clínica." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCriarDieta(avaliacao));

    await act(() => result.current.handleCriarDieta({ racaoId: 999 }));

    expect(message.error).toHaveBeenCalledWith("A ração selecionada não existe ou não pertence a esta clínica.");
    expect(result.current.isSubmitting).toBe(false);
  });

  test("editar chama editarDieta(id, valores), fecha e recarrega", async () => {
    acompanhamentoAPI.editarDieta.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalEditarDieta(dieta, avaliacao));

    await act(() => result.current.handleEditarDieta({ observacoes: "Ajuste" }));

    expect(acompanhamentoAPI.editarDieta).toHaveBeenCalledWith(90, { observacoes: "Ajuste" });
    expect(message.success).toHaveBeenCalledWith("Dieta atualizada com sucesso!");
    expect(result.current.isModalEditarDietaOpen).toBe(false);
    expect(result.current.dietaParaEditar).toBeNull();
  });

  test.each([
    ["handleExcluirDieta", "deletarDieta", "Dieta excluída com sucesso!"],
    ["handleIniciarDieta", "iniciarDieta", "Dieta iniciada com sucesso!"],
    ["handleConcluirDieta", "concluirDieta", "Dieta concluída com sucesso!"],
  ])("%s chama %s(id), mostra sucesso e recarrega", async (acao, funcaoApi, mensagem) => {
    acompanhamentoAPI[funcaoApi].mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current[acao](90));

    expect(acompanhamentoAPI[funcaoApi]).toHaveBeenCalledWith(90);
    expect(message.success).toHaveBeenCalledWith(mensagem);
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);
  });

  test.each([
    ["handleExcluirDieta", "deletarDieta"],
    ["handleIniciarDieta", "iniciarDieta"],
    ["handleConcluirDieta", "concluirDieta"],
  ])("%s com erro do backend mostra a mensagem e não recarrega", async (acao, funcaoApi) => {
    acompanhamentoAPI[funcaoApi].mockRejectedValue({ message: "Dieta não encontrada ou acesso negado." });
    const { result } = await montarHook();

    await act(() => result.current[acao](90));

    expect(message.error).toHaveBeenCalledWith("Dieta não encontrada ou acesso negado.");
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(1);
  });
});

describe("registros de peso", () => {
  test("registrar peso recarrega sem o loading da tela e atualiza a dieta aberta", async () => {
    const dietaComPeso = { ...dieta, registrosPeso: [{ id: 1, semana: 1, peso: 19.5 }] };
    acompanhamentoAPI.registrarPesoDieta.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalVisualizarDieta(dieta));
    acompanhamentoAPI.getHistoricoCompleto.mockResolvedValue({
      succeeded: true,
      data: { ...historico, consultas: [{ ...consultaComAvaliacao, avaliacao: { ...avaliacao, dieta: dietaComPeso } }] },
    });

    let retorno;
    await act(async () => {
      retorno = await result.current.handleRegistrarPeso(90, { semana: 1, peso: 19.5 });
    });

    expect(retorno).toBe(true);
    expect(acompanhamentoAPI.registrarPesoDieta).toHaveBeenCalledWith(90, { semana: 1, peso: 19.5 });
    expect(message.success).toHaveBeenCalledWith("Peso registrado com sucesso!");
    expect(result.current.loading).toBe(false);
    expect(result.current.dietaParaVisualizar).toEqual(dietaComPeso);
  });

  test("semana repetida: mostra a mensagem do backend e devolve false", async () => {
    acompanhamentoAPI.registrarPesoDieta.mockRejectedValue({ message: "Já existe um registro para a semana 1." });
    const { result } = await montarHook();

    let retorno;
    await act(async () => {
      retorno = await result.current.handleRegistrarPeso(90, { semana: 1, peso: 19.5 });
    });

    expect(retorno).toBe(false);
    expect(message.error).toHaveBeenCalledWith("Já existe um registro para a semana 1.");
  });

  test("remover peso chama deletarPeso(dietaId, registroId)", async () => {
    acompanhamentoAPI.deletarPeso.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current.handleRemoverPeso(90, 1));

    expect(acompanhamentoAPI.deletarPeso).toHaveBeenCalledWith(90, 1);
    expect(message.success).toHaveBeenCalledWith("Registro de peso removido!");
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);
  });
});

describe("consultas e animal do acompanhamento", () => {
  test("excluir consulta chama deletar(id) e recarrega; consulta com avaliação mostra o erro (409)", async () => {
    consultasDaClinicaAPI.deletar.mockResolvedValueOnce(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleDeleteConsulta(41));

    expect(consultasDaClinicaAPI.deletar).toHaveBeenCalledWith(41);
    expect(message.success).toHaveBeenCalledWith("Consulta excluída com sucesso!");
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);

    consultasDaClinicaAPI.deletar.mockRejectedValueOnce({ message: "Não é possível excluir uma consulta que já possui um relatório preenchido." });
    await act(() => result.current.handleDeleteConsulta(40));
    expect(message.error).toHaveBeenCalledWith("Não é possível excluir uma consulta que já possui um relatório preenchido.");
  });

  test("editar consulta chama editar(id, valores), fecha e recarrega", async () => {
    consultasDaClinicaAPI.editar.mockResolvedValue(undefined);
    const { result } = await montarHook();
    act(() => result.current.handleEditConsulta(consultaPendente));

    await act(() => result.current.handleSubmitConsulta({ dataConsulta: "2030-05-01T10:00:00Z", animalId: 3 }));

    expect(consultasDaClinicaAPI.editar).toHaveBeenCalledWith(41, { dataConsulta: "2030-05-01T10:00:00Z", animalId: 3 });
    expect(message.success).toHaveBeenCalledWith("Consulta atualizada com sucesso!");
    expect(result.current.isConsultaModalOpen).toBe(false);
  });

  test("editar consulta cancelada mostra o erro do backend", async () => {
    consultasDaClinicaAPI.editar.mockRejectedValue({ message: "Consulta cancelada não pode ser editada. Reabra-a antes." });
    const { result } = await montarHook();
    act(() => result.current.handleEditConsulta(consultaPendente));

    await act(() => result.current.handleSubmitConsulta({ dataConsulta: "2030-05-01T10:00:00Z", animalId: 3 }));

    expect(message.error).toHaveBeenCalledWith("Consulta cancelada não pode ser editada. Reabra-a antes.");
    expect(result.current.isConsultaModalOpen).toBe(true);
  });

  test("editar o animal envia o id do animal do histórico", async () => {
    animaisDaClinicaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleOpenEditAnimalModal());

    await act(() => result.current.handleSubmitEditAnimal({ nome: "Rex II", peso: 22 }));

    expect(animaisDaClinicaAPI.editar).toHaveBeenCalledWith({ nome: "Rex II", peso: 22, id: 3 });
    expect(message.success).toHaveBeenCalledWith("Animal atualizado com sucesso!");
    expect(result.current.isEditAnimalModalOpen).toBe(false);
    expect(acompanhamentoAPI.getHistoricoCompleto).toHaveBeenCalledTimes(2);
  });
});
