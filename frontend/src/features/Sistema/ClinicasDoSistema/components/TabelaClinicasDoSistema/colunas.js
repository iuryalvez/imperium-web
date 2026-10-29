import { Tag } from "antd";
import dayjs from "dayjs";

const colunas = [
  {
    title: "Nome da Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    sorter: true, // ordenação feita no servidor
  },
  {
    title: "Status",
    dataIndex: "isAtiva",
    key: "isAtiva",
    // Filtro aplicado no servidor (ver traduzirFiltrosClinica)
    filters: [
      { text: "Ativa", value: true },
      { text: "Inativa", value: false },
    ],
    filterMultiple: false,
    render: (isAtiva) => (
      <Tag color={isAtiva ? "green" : "red"}>{isAtiva ? "ATIVA" : "INATIVA"}</Tag>
    ),
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: true, // ordenação feita no servidor (padrão: mais recentes primeiro)
    render: (data) => data ? dayjs(data).format("DD/MM/YYYY") : "-",
  },
];

/** Converte os filtros de coluna da Table em filtros do servidor (ClinicaFiltro). */
export const traduzirFiltrosClinica = (filtros) => ({
  ativa: filtros.isAtiva?.[0],
});

export default colunas;