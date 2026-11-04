import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import { useAuth } from "../../../services/AuthContext.js";
import { useClinicasDoSistemaManager } from "./useClinicasDoSistemaManager";

jest.mock("../../../api/sistema/clinicasAPI.js", () => ({
  clinicasAPI: { listar: jest.fn(), cadastrar: jest.fn(), editar: jest.fn(), deletar: jest.fn() },
}));

jest.mock("../../../services/AuthContext.js", () => ({ useAuth: jest.fn() }));

jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const clinica = { id: 4, nomeClinica: "Clínica Bem-Estar", cnpj: "11222333000181", isAtiva: true };
const escolherVinculoAdmin = jest.fn();

const montarHook = async () => {
  const hook = renderHook(() => useClinicasDoSistemaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  useAuth.mockReturnValue({ escolherVinculoAdmin });
  clinicasAPI.listar.mockResolvedValue({ itens: [clinica], total: 1, pagina: 1, tamanhoPagina: 10 });
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página e expõe as clínicas", async () => {
    const { result } = await montarHook();

    expect(clinicasAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.clinicas).toEqual([clinica]);
  });

  test("filtro de status da coluna vira 'ativa' e a ordenação por nome vai para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 1, pageSize: 10 }, { isAtiva: [false] }, { columnKey: "nomeClinica", order: "ascend" }));

    await waitFor(() =>
      expect(clinicasAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, ativa: false, ordenarPor: "nomeClinica", ordem: "asc" })
    );
  });
});

describe("cadastro e edição", () => {
  const valores = { nomeClinica: "Nova", cnpj: "11.222.333/0001-81", cep: "74000-000", telefone: "(62) 3222-0000", whatsapp: undefined, isAtiva: true };

  test("cadastro envia documentos sem máscara e vazios como texto vazio, fecha e recarrega", async () => {
    clinicasAPI.cadastrar.mockResolvedValue({ id: 5 });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valores));

    expect(clinicasAPI.cadastrar).toHaveBeenCalledWith({
      nomeClinica: "Nova",
      cnpj: "11222333000181",
      cep: "74000000",
      telefone: "6232220000",
      whatsapp: "",
      isAtiva: true,
    });
    expect(message.success).toHaveBeenCalledWith("Clínica cadastrada com sucesso!");
    expect(result.current.isModalOpen).toBe(false);
    expect(clinicasAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia a clínica editada com os valores novos pelo editar", async () => {
    clinicasAPI.editar.mockResolvedValue(undefined);
    const { result } = await montarHook();
    act(() => result.current.handleEditarClinica(clinica));

    await act(() => result.current.handleSubmit({ ...valores, nomeClinica: "Bem-Estar II" }));

    expect(clinicasAPI.editar).toHaveBeenCalledWith(expect.objectContaining({ id: 4, nomeClinica: "Bem-Estar II", cnpj: "11222333000181" }));
    expect(clinicasAPI.cadastrar).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Clínica atualizada com sucesso!");
    expect(result.current.editingClinica).toBeNull();
  });

  test("CNPJ em uso (409) mostra a mensagem do backend, mantém o modal e não recarrega", async () => {
    clinicasAPI.editar.mockRejectedValue({ succeeded: false, message: "Este CNPJ já está em uso." });
    const { result } = await montarHook();
    act(() => result.current.handleEditarClinica(clinica));

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Este CNPJ já está em uso.");
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(clinicasAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("erro sem mensagem usa o texto padrão", async () => {
    clinicasAPI.cadastrar.mockRejectedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valores));

    expect(message.error).toHaveBeenCalledWith("Erro ao salvar clínica.");
  });
});

describe("simular acesso", () => {
  test("chama escolherVinculoAdmin com a clínica e os perfis escolhidos e fecha o modal", async () => {
    escolherVinculoAdmin.mockResolvedValue(undefined);
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalAcesso(clinica));

    await act(() => result.current.handleSimularAcesso(["GESTOR", "VETERINARIO"]));

    expect(escolherVinculoAdmin).toHaveBeenCalledWith(4, ["GESTOR", "VETERINARIO"]);
    expect(result.current.isModalAcessoOpen).toBe(false);
    expect(result.current.accessingClinica).toBeNull();
  });

  test("falha mostra a mensagem de erro", async () => {
    escolherVinculoAdmin.mockRejectedValue(new Error("x"));
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalAcesso(clinica));

    await act(() => result.current.handleSimularAcesso(["GESTOR"]));

    expect(message.error).toHaveBeenCalledWith("Erro ao simular acesso.");
  });
});

describe("excluir", () => {
  test("exclui clínica sem vínculos, mostra sucesso e recarrega a lista", async () => {
    clinicasAPI.deletar.mockResolvedValue(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleDeleteClinica(clinica));

    expect(clinicasAPI.deletar).toHaveBeenCalledWith(4);
    expect(message.success).toHaveBeenCalledWith("Clínica excluída com sucesso!");
    expect(clinicasAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("clínica com usuários vinculados (409) mostra a mensagem do backend e não recarrega", async () => {
    clinicasAPI.deletar.mockRejectedValue({ succeeded: false, message: "Clínica possui usuários vinculados." });
    const { result } = await montarHook();

    await act(() => result.current.handleDeleteClinica(clinica));

    expect(clinicasAPI.deletar).toHaveBeenCalledWith(4);
    expect(message.error).toHaveBeenCalledWith("Clínica possui usuários vinculados.");
    expect(clinicasAPI.listar).toHaveBeenCalledTimes(1);
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("modais", () => {
  test("detalhes e acesso guardam a clínica; fechar limpa tudo", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenModalDetalhes(clinica));
    expect(result.current.isModalDetalhesOpen).toBe(true);
    expect(result.current.viewingClinica).toEqual(clinica);

    act(() => result.current.handleCloseModals());
    expect(result.current.isModalDetalhesOpen).toBe(false);
    expect(result.current.viewingClinica).toBeNull();
  });
});
