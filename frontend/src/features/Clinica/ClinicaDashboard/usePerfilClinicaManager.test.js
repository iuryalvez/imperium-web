import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { perfilClinicaAPI } from "../../../api/clinica/perfilClinicaAPI";
import { useAuth } from "../../../services/AuthContext";
import { usePerfilClinicaManager } from "./usePerfilClinicaManager";

jest.mock("../../../api/clinica/perfilClinicaAPI", () => ({
  perfilClinicaAPI: { getClinicaDashboard: jest.fn(), updateOwnClinica: jest.fn() },
}));

// O AuthContext real importa a api de autenticação (axios): só o useAuth é usado aqui
jest.mock("../../../services/AuthContext", () => ({ useAuth: jest.fn() }));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const dashboard = { id: 1, nomeClinica: "Clínica Bem-Estar", totalAnimais: 12, totalTutores: 8 };
const authData = { nomeClinica: "Clínica Bem-Estar", perfis: ["GESTOR"] };
const setAuthData = jest.fn();

const montarHook = async () => {
  const hook = renderHook(() => usePerfilClinicaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  useAuth.mockReturnValue({ authData, setAuthData });
  perfilClinicaAPI.getClinicaDashboard.mockResolvedValue({ succeeded: true, data: dashboard });
});

describe("carga do dashboard", () => {
  test("ao montar, busca o dashboard e expõe os dados", async () => {
    const { result } = await montarHook();

    expect(perfilClinicaAPI.getClinicaDashboard).toHaveBeenCalledTimes(1);
    expect(result.current.dadosClinica).toEqual(dashboard);
  });

  test("ServiceResult sem sucesso mostra a mensagem dele", async () => {
    perfilClinicaAPI.getClinicaDashboard.mockResolvedValue({ succeeded: false, message: "Clínica não encontrada." });

    const { result } = await montarHook();

    expect(message.error).toHaveBeenCalledWith("Clínica não encontrada.");
    expect(result.current.dadosClinica).toBeNull();
  });

  test("erro de rede sem mensagem usa o texto padrão", async () => {
    perfilClinicaAPI.getClinicaDashboard.mockRejectedValue({});

    await montarHook();

    expect(message.error).toHaveBeenCalledWith("Falha de comunicação com o servidor.");
  });
});

describe("editar dados da clínica", () => {
  const valores = {
    nomeClinica: "Clínica Bem-Estar",
    cnpj: "11.222.333/0001-81",
    cep: "74000-000",
    telefone: "(62) 3222-0000",
    whatsapp: undefined,
    email: "contato@teste.local",
  };

  test("envia CNPJ, CEP e telefones sem máscara e vazios como texto vazio", async () => {
    perfilClinicaAPI.updateOwnClinica.mockResolvedValue({ succeeded: true, message: "Clínica atualizada com sucesso." });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(perfilClinicaAPI.updateOwnClinica).toHaveBeenCalledWith({
      nomeClinica: "Clínica Bem-Estar",
      cnpj: "11222333000181",
      cep: "74000000",
      telefone: "6232220000",
      whatsapp: "",
      email: "contato@teste.local",
    });
    expect(message.success).toHaveBeenCalledWith("Clínica atualizada com sucesso.");
    expect(perfilClinicaAPI.getClinicaDashboard).toHaveBeenCalledTimes(2);
    expect(result.current.isModalOpen).toBe(false);
  });

  test("nome igual ao do cabeçalho não mexe no authData", async () => {
    perfilClinicaAPI.updateOwnClinica.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(setAuthData).not.toHaveBeenCalled();
  });

  test("nome novo atualiza o nome da clínica no cabeçalho (authData)", async () => {
    perfilClinicaAPI.updateOwnClinica.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit({ ...valores, nomeClinica: "Clínica Renovada" }));

    expect(setAuthData).toHaveBeenCalledWith({ ...authData, nomeClinica: "Clínica Renovada" });
  });

  test("CNPJ de outra clínica (409): mostra a mensagem do backend, mantém o modal e não recarrega", async () => {
    perfilClinicaAPI.updateOwnClinica.mockRejectedValue({ succeeded: false, message: "Este CNPJ já está em uso por outra clínica." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModal());

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Este CNPJ já está em uso por outra clínica.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(perfilClinicaAPI.getClinicaDashboard).toHaveBeenCalledTimes(1);
    expect(setAuthData).not.toHaveBeenCalled();
  });
});
