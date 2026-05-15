import dayjs from "dayjs";
import { perfisSistema, renderPerfis } from "../../../../../utils/renderers";
import { formatarCPF, formatarTelefone } from "../../../../../utils/formatters";
import { Tag } from "antd";

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
    title: "Perfis",
    dataIndex: "perfis",
    key: "perfis",
    filters: perfisSistema.map((p) => ({ text: p.label, value: p.value })),
    onFilter: (value, record) => record.perfis.includes(value),
    render: renderPerfis,
  },
  {
    title: "Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    // Sorter seguro que lida com valores nulos
    sorter: (a, b) => (a.nomeClinica || "").localeCompare(b.nomeClinica || ""),
    // Renderiza apenas se houver nome de clínica
    render: (text) => text || "N/A",
  },
  {
    title: "Status",
    dataIndex: "isAtivo",
    key: "isAtivo",
    filters: [
      { text: "Ativo", value: true },
      { text: "Inativo", value: false },
    ],
    onFilter: (value, record) => record.isAtivo === value,
    render: (isAtivo) =>
      isAtivo ? <Tag color="green">Ativo</Tag> : <Tag color="red">Inativo</Tag>,
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: (a, b) =>
      dayjs(a.dataCadastro).unix() - dayjs(b.dataCadastro).unix(),
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY") : "-"),
  },
  {
    title: "Último Acesso",
    dataIndex: "dataUltimoAcesso",
    key: "dataUltimoAcesso",
    sorter: (a, b) =>
      dayjs(a.dataUltimoAcesso).unix() - dayjs(b.dataUltimoAcesso).unix(),
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY HH:mm") : "Nunca"),
  },
  {
    title: "CPF",
    dataIndex: "cpf",
    key: "cpf",
    render: (cpf) => formatarCPF(cpf),
  },
  {
    title: "Telefone",
    dataIndex: "telefone",
    key: "telefone",
    render: (telefone) => formatarTelefone(telefone),
  },
];

export default colunas;