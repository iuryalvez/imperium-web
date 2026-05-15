import { Tag } from "antd";
import dayjs from "dayjs";

const colunas = [
  {
    title: "Nome da Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica),
  },
  {
    title: "Status",
    dataIndex: "isAtiva",
    key: "isAtiva",
    render: (isAtiva) => (
      <Tag color={isAtiva ? "green" : "red"}>{isAtiva ? "ATIVA" : "INATIVA"}</Tag>
    ),
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    render: (data) => data ? dayjs(data).format("DD/MM/YYYY") : "-",
  },
];

export default colunas;