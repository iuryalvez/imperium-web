import dayjs from "dayjs";
import { perfisSistema, renderPerfis } from "../../../../utils/renderers";
import { formatarCPF, formatarTelefone } from "../../../../utils/formatters";
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
    title: "Status",
    dataIndex: "isAtivoNaClinica",
    key: "isAtivoNaClinica",
    filters: [
      { text: "Ativo", value: true },
      { text: "Inativo", value: false },
    ],
    onFilter: (value, record) => record.isAtivoNaClinica === value,
    render: (isAtivoNaClinica) =>
      isAtivoNaClinica ? <Tag color="green">Ativo</Tag> : <Tag color="red">Inativo</Tag>,
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