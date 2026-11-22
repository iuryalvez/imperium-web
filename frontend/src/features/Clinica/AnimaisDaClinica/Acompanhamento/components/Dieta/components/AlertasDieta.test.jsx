import { render, screen } from "@testing-library/react";
import { Form } from "antd";
import FormEmagrecimento, { PERFIL_SUGERIDO_CAO, PERFIL_SUGERIDO_GATO } from "./FormEmagrecimento";
import FormLactacao from "./FormLactacao";

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

const niveisRestricao = [
  { value: 1, label: "Severa", fator: 0.5, percentualPerdaSemanal: 0.02 },
  { value: 2, label: "Moderada", fator: 0.6, percentualPerdaSemanal: 0.015 },
  { value: 3, label: "Branda", fator: 0.7, percentualPerdaSemanal: 0.01 },
];

const perfis = [
  { value: 2, label: "Gato Doméstico - Magro ou Ideal, ECC ≤ 5 (100 × PC^0,67)", tiposDieta: [1, 5], k: 100, p: 0.67 },
  { value: 3, label: "Gato Doméstico - Obeso, ECC > 5 (130 × PC^0,40)", tiposDieta: [1, 5], k: 130, p: 0.4 },
  { value: 4, label: "Cão Ativo (130 × PC^0,75)", tiposDieta: [1, 5], k: 130, p: 0.75 },
  { value: 8, label: "Cão Inativo (95 × PC^0,75)", tiposDieta: [1, 5], k: 95, p: 0.75 },
];

let formCapturado;
const ComForm = ({ initialValues, children }) => {
  const [form] = Form.useForm();
  formCapturado = form;
  return <Form form={form} initialValues={initialValues}>{children(form)}</Form>;
};

const renderEmagrecimento = (especie, nivelRestricao) =>
  render(
    <ComForm initialValues={{ nivelRestricao }}>
      {(form) => (
        <FormEmagrecimento form={form} especie={especie} perfis={perfis} niveisRestricao={niveisRestricao} pesoAtual={10} />
      )}
    </ComForm>,
  );

const textoAlertaGato = /pode ser prejudicial ao gato/i;

test("gato com restrição moderada (1,5%/semana) mostra o alerta de perda acima do recomendado", () => {
  renderEmagrecimento(2, 2);
  expect(screen.getByText(textoAlertaGato)).toBeInTheDocument();
});

test("gato com restrição branda (1%/semana) não mostra o alerta", () => {
  renderEmagrecimento(2, 3);
  expect(screen.queryByText(textoAlertaGato)).not.toBeInTheDocument();
});

test("cão com restrição severa não mostra o alerta do gato", () => {
  renderEmagrecimento(1, 1);
  expect(screen.queryByText(textoAlertaGato)).not.toBeInTheDocument();
});

test("o emagrecimento sugere Cão Inativo para cão e Gato Magro ou Ideal para gato", () => {
  renderEmagrecimento(1, 2);
  expect(formCapturado.getFieldValue("perfilMetabolico")).toBe(PERFIL_SUGERIDO_CAO);

  renderEmagrecimento(2, 2);
  expect(formCapturado.getFieldValue("perfilMetabolico")).toBe(PERFIL_SUGERIDO_GATO);
});

const renderLactacao = (numeroFilhotes) =>
  render(
    <ComForm initialValues={{ numeroFilhotes, semanaLactacao: 2 }}>
      {(form) => <FormLactacao form={form} especie={1} perfis={[]} />}
    </ComForm>,
  );

test("lactação com mais de 8 filhotes avisa que está fora da faixa da fonte", () => {
  renderLactacao(9);
  expect(screen.getByText(/fora da faixa da fonte/i)).toBeInTheDocument();
});

test("lactação com 8 filhotes não mostra o aviso de fora da faixa", () => {
  renderLactacao(8);
  expect(screen.queryByText(/fora da faixa da fonte/i)).not.toBeInTheDocument();
});
