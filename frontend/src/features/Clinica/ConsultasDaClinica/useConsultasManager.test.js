import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { useConsultasManager } from "./useConsultasManager";

jest.mock("../../../api/clinica/consultasDaClinicaAPI", () => ({
  consultasDaClinicaAPI: {
    listar: jest.fn(),
    cadastrar: jest.fn(),
    editar: jest.fn(),
    deletar: jest.fn(),
    cancelar: jest.fn(),
    reabrir: jest.fn(),
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

const consulta = { id: 40, animalId: 3, tutorId: 7, dataConsulta: "2030-03-15T13:30:00Z", status: "Pendente", acompanhamentoId: 30 };

const montarHook = async () => {
  const hook = renderHook(() => useConsultasManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  consultasDaClinicaAPI.listar.mockResolvedValue({ itens: [consulta], total: 1, pagina: 1, tamanhoPagina: 10 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página e expõe as consultas", async () => {
    const { result } = await montarHook();

    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.consultas).toEqual([consulta]);
  });

  test("filtro de status da coluna vira 'status' e a ordenação por data vai para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { status: ["Cancelada"] }, { columnKey: "dataConsulta", order: "ascend" }));

    await waitFor(() =>
      expect(consultasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, status: "Cancelada", ordenarPor: "dataConsulta", ordem: "asc" })
    );
  });

  test("busca por texto (animal ou tutor) vai para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.setBusca("bidu"));

    await waitFor(() => expect(consultasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, busca: "bidu" }));
  });
});

describe("agendar e editar", () => {
  const valores = { animalId: 3, tutorId: 7, dataConsulta: "2030-04-01T09:00:00Z", emLactacao: false };

  test("agendamento envia os valores sem o campo de tela 'emLactacao'", async () => {
    consultasDaClinicaAPI.cadastrar.mockResolvedValue({ id: 41 });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    const enviado = consultasDaClinicaAPI.cadastrar.mock.calls[0][0];
    expect(enviado).toEqual(expect.objectContaining({ animalId: 3, tutorId: 7, dataConsulta: "2030-04-01T09:00:00Z" }));
    expect(enviado).not.toHaveProperty("emLactacao");
    expect(message.success).toHaveBeenCalledWith("Consulta agendada com sucesso!");
    expect(result.current.isModalOpen).toBe(false);
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição chama editar(id, valores)", async () => {
    consultasDaClinicaAPI.editar.mockResolvedValue(undefined);
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal(consulta));

    await act(() => result.current.handleSubmit(valores));

    expect(consultasDaClinicaAPI.editar).toHaveBeenCalledWith(40, expect.objectContaining({ animalId: 3, dataConsulta: "2030-04-01T09:00:00Z" }));
    expect(consultasDaClinicaAPI.cadastrar).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Consulta atualizada com sucesso!");
  });

  test("erro do backend mostra a mensagem, mantém o modal e relança o erro para o formulário", async () => {
    const erro = { message: "O animal selecionado está inativo." };
    consultasDaClinicaAPI.cadastrar.mockRejectedValue(erro);
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(async () => {
      await expect(result.current.handleSubmit(valores)).rejects.toBe(erro);
    });

    expect(message.error).toHaveBeenCalledWith("O animal selecionado está inativo.");
    expect(result.current.isModalOpen).toBe(true);
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });
});

describe("excluir, cancelar e reabrir", () => {
  test("excluir chama deletar(id), mostra sucesso e recarrega", async () => {
    consultasDaClinicaAPI.deletar.mockResolvedValue(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(40));

    expect(consultasDaClinicaAPI.deletar).toHaveBeenCalledWith(40);
    expect(message.success).toHaveBeenCalledWith("Consulta excluída com sucesso!");
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("excluir consulta com avaliação mostra a mensagem do backend (409)", async () => {
    consultasDaClinicaAPI.deletar.mockRejectedValue({ message: "Não é possível excluir uma consulta que já possui um relatório preenchido." });
    const { result } = await montarHook();

    await act(() => result.current.handleDelete(40));

    expect(message.error).toHaveBeenCalledWith("Não é possível excluir uma consulta que já possui um relatório preenchido.");
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("cancelar envia o motivo e mostra a mensagem do backend", async () => {
    consultasDaClinicaAPI.cancelar.mockResolvedValue({ message: "Consulta cancelada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleCancelar(consulta, "Tutor pediu"));

    expect(consultasDaClinicaAPI.cancelar).toHaveBeenCalledWith(40, "Tutor pediu");
    expect(message.success).toHaveBeenCalledWith("Consulta cancelada com sucesso.");
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("cancelar sem motivo envia nulo", async () => {
    consultasDaClinicaAPI.cancelar.mockResolvedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleCancelar(consulta, ""));

    expect(consultasDaClinicaAPI.cancelar).toHaveBeenCalledWith(40, null);
    expect(message.success).toHaveBeenCalledWith("Consulta cancelada com sucesso!");
  });

  test("cancelar consulta com avaliação mostra o erro do backend", async () => {
    consultasDaClinicaAPI.cancelar.mockRejectedValue({ message: "Só consultas pendentes (sem avaliação) podem ser canceladas." });
    const { result } = await montarHook();

    await act(() => result.current.handleCancelar(consulta));

    expect(message.error).toHaveBeenCalledWith("Só consultas pendentes (sem avaliação) podem ser canceladas.");
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("reabrir chama reabrir(id) e recarrega; erro mostra a mensagem", async () => {
    consultasDaClinicaAPI.reabrir.mockResolvedValueOnce({ message: "Consulta reaberta com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleReabrir(consulta));

    expect(consultasDaClinicaAPI.reabrir).toHaveBeenCalledWith(40);
    expect(message.success).toHaveBeenCalledWith("Consulta reaberta com sucesso.");
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);

    consultasDaClinicaAPI.reabrir.mockRejectedValueOnce({ message: "Só consultas canceladas podem ser reabertas." });
    await act(() => result.current.handleReabrir(consulta));
    expect(message.error).toHaveBeenCalledWith("Só consultas canceladas podem ser reabertas.");
  });
});

describe("acompanhamento", () => {
  test("vai para o acompanhamento pelo id do acompanhamento", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleIrParaAcompanhamento(30));

    expect(mockNavigate).toHaveBeenCalledWith("/clinica/animais/acompanhamento/30");
  });

  test("sem acompanhamento não navega", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleIrParaAcompanhamento(undefined));

    expect(mockNavigate).not.toHaveBeenCalled();
  });
});
