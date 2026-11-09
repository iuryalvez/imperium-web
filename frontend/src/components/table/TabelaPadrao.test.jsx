import { render, screen } from "@testing-library/react";
import TabelaPadrao from "./TabelaPadrao";

// jsdom não tem matchMedia; o antd (Grid) e a TabelaPadrao usam
const simularTela = (celular) => {
  window.matchMedia = jest.fn().mockImplementation((query) => ({
    matches: query.includes("max-width") ? celular : !celular,
    media: query,
    addListener: jest.fn(),
    removeListener: jest.fn(),
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
  }));
};

const colunas = [
  { title: "Nome", dataIndex: "nome", key: "nome" },
  { title: "E-mail", dataIndex: "email", key: "email" },
  { title: "Telefone", dataIndex: "telefone", key: "telefone" },
  { title: "Ações", key: "acoes", render: () => "..." },
];
const dados = [{ id: 1, nome: "Ana", email: "ana@teste.local", telefone: "62999990000" }];

const montar = () =>
  render(
    <TabelaPadrao
      selecionarColunas
      colunasIniciais={["nome", "email"]}
      colunasCelular={["nome"]}
      columns={colunas}
      dataSource={dados}
      rowKey="id"
    />
  );

const cabecalhos = () => screen.getAllByRole("columnheader").map((th) => th.textContent);

test("no desktop abre com as colunas iniciais e mantém as ações", () => {
  simularTela(false);
  montar();
  expect(cabecalhos()).toEqual(["Nome", "E-mail", "Ações"]);
});

test("no celular abre só com as colunas do celular e as ações", () => {
  simularTela(true);
  montar();
  expect(cabecalhos()).toEqual(["Nome", "Ações"]);
});
