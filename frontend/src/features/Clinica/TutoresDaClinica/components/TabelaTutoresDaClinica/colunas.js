import { formatarCPF, formatarTelefone } from "../../../../../utils/formatters";

const colunas = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  {
    title: "E-mail",
    dataIndex: "email",
    key: "email",
  },
  {
    title: "CPF",
    dataIndex: "cpf",
    key: "cpf",
    render: (cpf) => formatarCPF(cpf) || "-",
  },
  {
    title: "Telefone",
    dataIndex: "telefone",
    key: "telefone",
    render: (telefone) => formatarTelefone(telefone) || "-",
  },
  {
    title: "Endereço",
    dataIndex: "endereco",
    key: "endereco",
    render: (text) => text || "-",
  },
];

export default colunas;