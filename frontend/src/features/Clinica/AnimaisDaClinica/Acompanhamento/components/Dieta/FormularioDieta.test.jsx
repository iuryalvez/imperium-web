import { render, screen, fireEvent, waitFor, within } from "@testing-library/react";
import FormularioDieta from "./FormularioDieta";
import { acompanhamentoAPI } from "../../../../../../api/clinica/acompanhamentoAPI";

jest.mock("../../../../../../api/clinica/acompanhamentoAPI", () => ({
  acompanhamentoAPI: { getPerfisMetabolicos: jest.fn(), getNiveisRestricao: jest.fn() },
}));
jest.mock("../../../../../../api/clinica/racoesDaClinicaAPI", () => ({
  racoesDaClinicaAPI: { opcoes: () => Promise.resolve([]) },
}));

// jsdom não tem matchMedia (usado pelo Grid do antd). Função comum, e não jest.fn: o CRA
// reseta os mocks antes de cada teste (resetMocks).
const semEfeito = () => {};
window.matchMedia = (query) => ({
  matches: false,
  media: query,
  addListener: semEfeito,
  removeListener: semEfeito,
  addEventListener: semEfeito,
  removeEventListener: semEfeito,
});

// O jsdom não entende um seletor do CSS do dropdown do antd e quebra no getComputedStyle.
// O teste não depende de estilo, então devolve um estilo vazio.
window.getComputedStyle = () => ({ getPropertyValue: () => "", overflow: "", overflowX: "", overflowY: "" });

const perfis = [
  { value: 4, label: "Cão Ativo (130 × PC^0,75)", tiposDieta: [1, 5], k: 130, p: 0.75, especie: 1 },
  { value: 8, label: "Cão Inativo (95 × PC^0,75)", tiposDieta: [1, 5], k: 95, p: 0.75, especie: 1 },
  { value: 18, label: "Cão em Emagrecimento", tiposDieta: [5], k: null, p: null, especie: 1 },
];
const niveis = [{ value: 2, label: "Moderada", fator: 0.6, percentualPerdaSemanal: 0.015 }];

// Dieta de emagrecimento já salva com Cão Ativo (K 130), como na edição
const dietaSalva = {
  id: 1, especieAnimal: 1, tipoDieta: 5, pesoAtual: 38, pesoObjetivo: 34, nivelRestricao: 2,
  perfilMetabolico: 4, multiplicadorEnergetico: 130, potencia: 0.75,
};

const perfilExibido = () => document.querySelector("#perfilMetabolico")
  .closest(".ant-select").querySelector(".ant-select-selection-item").getAttribute("title");

const renderFormulario = async () => {
  acompanhamentoAPI.getPerfisMetabolicos.mockResolvedValue(perfis);
  acompanhamentoAPI.getNiveisRestricao.mockResolvedValue(niveis);
  render(<FormularioDieta onFinish={jest.fn()} initialValues={dietaSalva} />);
  await waitFor(() => expect(perfilExibido()).toBe("Cão Ativo (130 × PC^0,75)"));
};

test("trocar o perfil com um já selecionado muda na primeira escolha e preenche K e P do novo perfil", async () => {
  await renderFormulario();

  fireEvent.mouseDown(document.querySelector("#perfilMetabolico"));
  const lista = await screen.findByRole("listbox");
  fireEvent.click(within(document.body).getAllByText("Cão Inativo (95 × PC^0,75)").pop());
  expect(lista).toBeTruthy();

  await waitFor(() => expect(perfilExibido()).toBe("Cão Inativo (95 × PC^0,75)"));
  expect(document.querySelector("#multiplicadorEnergetico").value).toBe("95,0");
});

test("editar o K à mão passa o perfil para o genérico da dieta", async () => {
  await renderFormulario();

  const campoK = document.querySelector("#multiplicadorEnergetico");
  fireEvent.change(campoK, { target: { value: "110" } });

  await waitFor(() => expect(perfilExibido()).toBe("Cão em Emagrecimento"));
});
