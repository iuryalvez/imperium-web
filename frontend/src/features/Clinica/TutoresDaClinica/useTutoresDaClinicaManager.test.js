import { renderHook, act, waitFor } from "@testing-library/react";
import { message } from "antd";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI.js";
import { useTutoresDaClinicaManager } from "./useTutoresDaClinicaManager";

// A camada api/ é mockada inteira: nada de axios nem de backend no teste
jest.mock("../../../api/clinica/tutoresDaClinicaAPI.js", () => ({
  tutoresDaClinicaAPI: {
    listar: jest.fn(),
    cadastrar: jest.fn(),
    editar: jest.fn(),
    deletar: jest.fn(),
    setStatus: jest.fn(),
  },
}));

// Só o message do antd é trocado, para conferir as mensagens mostradas ao usuário
jest.mock("antd", () => ({
  ...jest.requireActual("antd"),
  message: { success: jest.fn(), error: jest.fn() },
}));

const tutorAna = { id: 7, nome: "Ana", cpf: "52998224725", email: "ana@teste.local", isAtivo: true };
const paginaComAna = { itens: [tutorAna], total: 1, pagina: 1, tamanhoPagina: 10 };

/** Monta o hook e espera a primeira carga da lista. */
const montarHook = async () => {
  const hook = renderHook(() => useTutoresDaClinicaManager());
  await waitFor(() => expect(hook.result.current.loading).toBe(false));
  return hook;
};

beforeEach(() => {
  jest.clearAllMocks();
  tutoresDaClinicaAPI.listar.mockResolvedValue(paginaComAna);
});

describe("lista paginada", () => {
  test("ao montar, busca a 1ª página com 10 itens e expõe tutores e total", async () => {
    const { result } = await montarHook();

    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledWith({ pagina: 1, tamanhoPagina: 10 });
    expect(result.current.tutores).toEqual([tutorAna]);
    expect(result.current.paginacao.total).toBe(1);
  });

  test("busca por texto vai para o servidor depois do debounce, na página 1", async () => {
    const { result } = await montarHook();

    act(() => result.current.setBusca("ana"));

    await waitFor(() =>
      expect(tutoresDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, busca: "ana" })
    );
  });

  test("troca de página, tamanho e ordenação da tabela vai para o servidor", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 2, pageSize: 20 }, {}, { columnKey: "nome", order: "ascend" }));

    await waitFor(() =>
      expect(tutoresDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 2, tamanhoPagina: 20, ordenarPor: "nome", ordem: "asc" })
    );
  });

  test("filtro de status da coluna vira o filtro 'ativo' e volta para a página 1", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleTableChange({ current: 3, pageSize: 10 }, { isAtivo: [false] }, {}));

    await waitFor(() =>
      expect(tutoresDaClinicaAPI.listar).toHaveBeenLastCalledWith({ pagina: 1, tamanhoPagina: 10, ativo: false })
    );
  });

  test("erro ao carregar mostra a mensagem do backend e deixa a lista vazia", async () => {
    tutoresDaClinicaAPI.listar.mockRejectedValue({ message: "Falha no servidor." });

    const { result } = await montarHook();

    expect(message.error).toHaveBeenCalledWith("Falha no servidor.");
    expect(result.current.tutores).toEqual([]);
  });
});

describe("cadastro e edição", () => {
  const valoresDoFormulario = {
    nome: "Bruno",
    cpf: "529.982.247-25",
    telefone: "(62) 99999-0000",
    whatsapp: undefined,
    cep: "74000-000",
    email: "bruno@teste.local",
    complemento: null,
  };

  test("cadastro envia os documentos sem máscara e campos vazios como texto vazio", async () => {
    tutoresDaClinicaAPI.cadastrar.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valoresDoFormulario));

    expect(tutoresDaClinicaAPI.cadastrar).toHaveBeenCalledWith({
      nome: "Bruno",
      cpf: "52998224725",
      telefone: "62999990000",
      whatsapp: "",
      cep: "74000000",
      email: "bruno@teste.local",
      complemento: "",
    });
    expect(tutoresDaClinicaAPI.editar).not.toHaveBeenCalled();
  });

  test("cadastro com sucesso mostra mensagem, fecha o modal e recarrega a lista", async () => {
    tutoresDaClinicaAPI.cadastrar.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valoresDoFormulario));

    expect(message.success).toHaveBeenCalledWith("Tutor cadastrado com sucesso!");
    expect(result.current.isModalOpen).toBe(false);
    expect(result.current.isSubmitting).toBe(false);
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("edição envia o tutor que está sendo editado com os valores novos, pelo editar", async () => {
    tutoresDaClinicaAPI.editar.mockResolvedValue({});
    const { result } = await montarHook();
    act(() => result.current.handleEditarTutor(tutorAna));

    await act(() => result.current.handleSubmit({ ...valoresDoFormulario, nome: "Ana Maria" }));

    expect(tutoresDaClinicaAPI.editar).toHaveBeenCalledWith(
      expect.objectContaining({ id: 7, nome: "Ana Maria", cpf: "52998224725", telefone: "62999990000" })
    );
    expect(tutoresDaClinicaAPI.cadastrar).not.toHaveBeenCalled();
    expect(message.success).toHaveBeenCalledWith("Tutor atualizado com sucesso!");
    expect(result.current.editingTutor).toBeNull();
  });

  test("erro do backend mostra a mensagem dele, mantém o modal aberto e não recarrega", async () => {
    tutoresDaClinicaAPI.cadastrar.mockRejectedValue({ succeeded: false, message: "CPF já cadastrado nesta clínica." });
    const { result } = await montarHook();
    act(() => result.current.handleOpenModalCadastro());

    await act(() => result.current.handleSubmit(valoresDoFormulario));

    expect(message.error).toHaveBeenCalledWith("CPF já cadastrado nesta clínica.");
    expect(message.success).not.toHaveBeenCalled();
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.isSubmitting).toBe(false);
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
  });

  test("erro sem mensagem usa o texto padrão", async () => {
    tutoresDaClinicaAPI.cadastrar.mockRejectedValue({});
    const { result } = await montarHook();

    await act(() => result.current.handleSubmit(valoresDoFormulario));

    expect(message.error).toHaveBeenCalledWith("Erro ao salvar tutor.");
  });
});

describe("excluir", () => {
  test("chama deletar com o id, mostra sucesso e recarrega", async () => {
    tutoresDaClinicaAPI.deletar.mockResolvedValue(undefined);
    const { result } = await montarHook();

    await act(() => result.current.handleDeletarTutor(tutorAna));

    expect(tutoresDaClinicaAPI.deletar).toHaveBeenCalledWith(7);
    expect(message.success).toHaveBeenCalledWith("Tutor removido com sucesso!");
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
    expect(result.current.loadingRowKey).toBeNull();
  });

  test("tutor com consultas: mostra a mensagem do backend (409) e não recarrega", async () => {
    tutoresDaClinicaAPI.deletar.mockRejectedValue({
      message: "Este tutor possui consultas registradas e não pode ser excluído. Desative-o.",
    });
    const { result } = await montarHook();

    await act(() => result.current.handleDeletarTutor(tutorAna));

    expect(message.error).toHaveBeenCalledWith("Este tutor possui consultas registradas e não pode ser excluído. Desative-o.");
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(1);
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("mudar status", () => {
  test("tutor ativo é desativado: setStatus(id, false), mensagem e recarga", async () => {
    tutoresDaClinicaAPI.setStatus.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(tutorAna));

    expect(tutoresDaClinicaAPI.setStatus).toHaveBeenCalledWith(7, false);
    expect(message.success).toHaveBeenCalledWith("Tutor desativado com sucesso!");
    expect(tutoresDaClinicaAPI.listar).toHaveBeenCalledTimes(2);
  });

  test("tutor inativo é ativado: setStatus(id, true)", async () => {
    tutoresDaClinicaAPI.setStatus.mockResolvedValue({ succeeded: true });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus({ ...tutorAna, isAtivo: false }));

    expect(tutoresDaClinicaAPI.setStatus).toHaveBeenCalledWith(7, true);
    expect(message.success).toHaveBeenCalledWith("Tutor ativado com sucesso!");
  });

  test("erro do backend mostra a mensagem e libera o loading da linha", async () => {
    tutoresDaClinicaAPI.setStatus.mockRejectedValue({ message: "Tutor não encontrado nesta clínica." });
    const { result } = await montarHook();

    await act(() => result.current.handleToggleStatus(tutorAna));

    expect(message.error).toHaveBeenCalledWith("Tutor não encontrado nesta clínica.");
    expect(result.current.loadingRowKey).toBeNull();
  });
});

describe("modais", () => {
  test("abrir cadastro limpa a edição; editar guarda o tutor; fechar limpa tudo", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleEditarTutor(tutorAna));
    expect(result.current.isModalOpen).toBe(true);
    expect(result.current.editingTutor).toEqual(tutorAna);

    act(() => result.current.handleOpenModalCadastro());
    expect(result.current.editingTutor).toBeNull();

    act(() => result.current.handleCloseModal());
    expect(result.current.isModalOpen).toBe(false);
  });

  test("detalhes guarda o tutor visualizado", async () => {
    const { result } = await montarHook();

    act(() => result.current.handleOpenModalDetalhes(tutorAna));

    expect(result.current.isOpenModalDetalhes).toBe(true);
    expect(result.current.viewingTutor).toEqual(tutorAna);
  });
});
