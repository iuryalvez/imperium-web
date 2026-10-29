import { Tag } from "antd";
import { formatarCPF, formatarTelefone } from "../../../../../utils/formatters";

const colunas = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: true, // ordenação feita no servidor
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
  {
    title: "Status",
    dataIndex: "isAtivo",
    key: "isAtivo",
    filters: [
      { text: "Ativo", value: true },
      { text: "Inativo", value: false },
    ],
    filterMultiple: false, // filtro aplicado no servidor
    render: (isAtivo) =>
      isAtivo ? <Tag color="green">Ativo</Tag> : <Tag color="red">Inativo</Tag>,
  },
];

/** Converte os filtros de coluna da Table em filtros do servidor (TutorFiltro). */
export const traduzirFiltrosTutor = (filtros) => ({
  ativo: filtros.isAtivo?.[0],
});

export default colunas;