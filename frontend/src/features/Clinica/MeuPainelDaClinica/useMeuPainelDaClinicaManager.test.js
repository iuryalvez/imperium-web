import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { meuPainelDaClinicaAPI } from "../../../api/clinica/meuPainelDaClinicaAPI";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { useAuth } from "../../../services/AuthContext";
import { useMeuPainelDaClinicaManager } from "./useMeuPainelDaClinicaManager";

jest.mock("../../../api/clinica/meuPainelDaClinicaAPI", () => ({
  meuPainelDaClinicaAPI: { obter: jest.fn() },
}));

jest.mock("../../../api/clinica/consultasDaClinicaAPI", () => ({
  consultasDaClinicaAPI: { listar: jest.fn() },
}));

jest.mock("../../../services/AuthContext", () => ({ useAuth: jest.fn() }));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

const painel = { nome: "Vera", email: "vera@teste.local", nomeClinica: "Clínica Bem-Estar" };
const paginaVazia = { itens: [], total: 0, pagina: 1, tamanhoPagina: 5 };

const montarHook = async (perfis) => {
  useAuth.mockReturnValue({ perfis });
  const hook = renderHook(() => useMeuPainelDaClinicaManager());
  await waitFor(() => {
    expect(hook.result.current.loading).toBe(false);
    expect(hook.result.current.consultasMarcadas.loading).toBe(false);
    expect(hook.result.current.consultasAtendidas.loading).toBe(false);
    expect(hook.result.current.consultasPendentes.loading).toBe(false);
  });
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  meuPainelDaClinicaAPI.obter.mockResolvedValue(painel);
  consultasDaClinicaAPI.listar.mockResolvedValue(paginaVazia);
});

describe("carga do painel", () => {
  test("carrega os dados do usuário logado", async () => {
    const { result } = await montarHook(["TECNICO"]);

    expect(meuPainelDaClinicaAPI.obter).toHaveBeenCalledTimes(1);
    expect(result.current.painel).toEqual(painel);
  });

  test("erro do backend mostra a mensagem", async () => {
    meuPainelDaClinicaAPI.obter.mockRejectedValue({ message: "Usuário não encontrado." });

    const { result } = await montarHook(["TECNICO"]);

    expect(message.error).toHaveBeenCalledWith("Usuário não encontrado.");
    expect(result.current.painel).toBeNull();
  });
});

describe("seções por perfil", () => {
  test("TECNICO não busca consultas", async () => {
    const { result } = await montarHook(["TECNICO"]);

    expect(consultasDaClinicaAPI.listar).not.toHaveBeenCalled();
    expect(result.current.ehSecretario).toBe(false);
    expect(result.current.ehVeterinario).toBe(false);
    expect(result.current.podeVerDashboard).toBe(false);
  });

  test("SECRETARIO busca só as consultas que marcou, 5 por página", async () => {
    const { result } = await montarHook(["SECRETARIO"]);

    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 5, marcadasPorMim: true });
    expect(result.current.ehSecretario).toBe(true);
  });

  test("VETERINARIO busca as atendidas por ele e as pendentes da clínica (mais antigas primeiro)", async () => {
    const { result } = await montarHook(["VETERINARIO"]);

    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 5, atendidasPorMim: true });
    expect(consultasDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 5, status: "Pendente", ordem: "asc" });
    expect(result.current.ehVeterinario).toBe(true);
  });

  test("GESTOR vê o atalho do dashboard e vai para a tela da clínica", async () => {
    const { result } = await montarHook(["GESTOR"]);

    expect(result.current.podeVerDashboard).toBe(true);
    act(() => result.current.handleIrParaDashboard());
    expect(mockNavigate).toHaveBeenCalledWith("/clinica/editar");
  });

  test("página da seção de pendentes troca só nessa lista", async () => {
    const { result } = await montarHook(["VETERINARIO"]);

    act(() => result.current.consultasPendentes.handleTableChange({ current: 2, pageSize: 5 }, {}, {}));

    await waitFor(() =>
      expect(consultasDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 2, tamanhoPagina: 5, status: "Pendente", ordem: "asc" })
    );
  });
});

describe("acompanhamento", () => {
  test("vai para o acompanhamento só quando há id", async () => {
    const { result } = await montarHook(["VETERINARIO"]);

    act(() => result.current.handleIrParaAcompanhamento(undefined));
    expect(mockNavigate).not.toHaveBeenCalled();

    act(() => result.current.handleIrParaAcompanhamento(30));
    expect(mockNavigate).toHaveBeenCalledWith("/clinica/animais/acompanhamento/30");
  });
});
