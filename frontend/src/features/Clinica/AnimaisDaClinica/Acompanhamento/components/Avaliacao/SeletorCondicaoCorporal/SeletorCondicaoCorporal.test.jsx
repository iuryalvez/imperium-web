import { render, screen, fireEvent } from "@testing-library/react";
import SeletorCondicaoCorporal, {
  RotuloCondicaoCorporal,
  silhuetasDestacadas,
} from "./SeletorCondicaoCorporal";

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

describe("silhuetasDestacadas", () => {
  it("destaca a própria silhueta no escore ímpar", () => {
    expect(silhuetasDestacadas(5)).toEqual([5]);
  });

  it("destaca as duas vizinhas no escore par", () => {
    expect(silhuetasDestacadas(4)).toEqual([3, 5]);
  });

  it("não destaca nada sem valor", () => {
    expect(silhuetasDestacadas(null)).toEqual([]);
    expect(silhuetasDestacadas(undefined)).toEqual([]);
  });
});

describe("SeletorCondicaoCorporal", () => {
  it("clicar numa silhueta escolhe aquele escore", () => {
    const escolhidos = [];
    render(<SeletorCondicaoCorporal especie="cao" value={5} onChange={(v) => escolhidos.push(v)} />);

    fireEvent.click(screen.getByRole("button", { name: "Escore 7" }));

    expect(escolhidos).toEqual([7]);
  });

  it("marca como pressionada só a silhueta do escore atual", () => {
    render(<SeletorCondicaoCorporal especie="gato" value={3} onChange={() => {}} />);

    expect(screen.getByRole("button", { name: "Escore 3" })).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByRole("button", { name: "Escore 5" })).toHaveAttribute("aria-pressed", "false");
  });

  it("mostra a descrição do escore conforme a espécie", () => {
    const { rerender } = render(<SeletorCondicaoCorporal especie="cao" value={4} onChange={() => {}} />);
    expect(screen.getByText("4 · Ideal")).toBeInTheDocument();

    rerender(<SeletorCondicaoCorporal especie="gato" value={4} onChange={() => {}} />);
    expect(screen.getByText("4 · Magro")).toBeInTheDocument();
  });

  it("mostra o percentual acima do ideal nos escores altos", () => {
    render(<SeletorCondicaoCorporal especie="cao" value={7} onChange={() => {}} />);
    expect(screen.getByText("7 · Sobrepeso (cerca de 20% acima do ideal)")).toBeInTheDocument();
  });

  it("sem valor mostra que o escore não foi informado", () => {
    render(<SeletorCondicaoCorporal especie="cao" onChange={() => {}} />);
    expect(screen.getByText(/Não informado/)).toBeInTheDocument();
  });
});

describe("RotuloCondicaoCorporal", () => {
  it("o ⓘ abre a tabela WSAVA da espécie", async () => {
    render(<RotuloCondicaoCorporal especie="gato" />);

    fireEvent.click(screen.getByRole("button", { name: "Ver tabela de condição corporal da WSAVA" }));

    expect(await screen.findByText("Escore de Condição Corporal — Gato")).toBeInTheDocument();
    expect(screen.getByRole("img", { name: /condição corporal do gato/ })).toBeInTheDocument();
  });

  it("mostra o Limpar só quando recebe onLimpar", () => {
    let limpou = false;
    const { rerender } = render(<RotuloCondicaoCorporal especie="cao" onLimpar={() => { limpou = true; }} />);

    fireEvent.click(screen.getByRole("button", { name: "Limpar" }));
    expect(limpou).toBe(true);

    rerender(<RotuloCondicaoCorporal especie="gato" />);
    expect(screen.queryByRole("button", { name: "Limpar" })).not.toBeInTheDocument();
  });
});
