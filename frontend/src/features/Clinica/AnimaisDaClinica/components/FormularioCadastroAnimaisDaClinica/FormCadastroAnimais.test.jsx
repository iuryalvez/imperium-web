import { render, screen } from "@testing-library/react";
import { useAuth } from "../../../../../services/AuthContext";
import FormCadastroAnimal from "./FormCadastroAnimais";

jest.mock("../../../../../services/AuthContext", () => ({ useAuth: jest.fn() }));
jest.mock("../../../../../api/clinica/tutoresDaClinicaAPI", () => ({
  tutoresDaClinicaAPI: { opcoes: () => Promise.resolve([]) },
}));
jest.mock("../../../../../api/clinica/racasDaClinicaAPI", () => ({
  racasDaClinicaAPI: { opcoes: () => Promise.resolve([]) },
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

test("perfis da clínica não veem a opção de tutela da clínica", () => {
  useAuth.mockReturnValue({ isAdmin: false });
  render(<FormCadastroAnimal onFinish={jest.fn()} />);
  expect(screen.queryByText("Animal sob tutela da clínica")).not.toBeInTheDocument();
});

test("o admin vê a opção de tutela da clínica", () => {
  useAuth.mockReturnValue({ isAdmin: true });
  render(<FormCadastroAnimal onFinish={jest.fn()} />);
  expect(screen.getByText("Animal sob tutela da clínica")).toBeInTheDocument();
});
