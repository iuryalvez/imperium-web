import { Tag } from "antd";

const formatarPeso = (min, max) => {
  if (min == null && max == null) return "-";
  if (min === max || max == null) return `${min} kg`;
  if (min == null) return `${max} kg`;
  return `${min} a ${max} kg`;
};

const colunas = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: true, // ordenação feita no servidor
  },
  {
    title: "Espécie",
    dataIndex: "especie",
    key: "especie",
    // Filtro aplicado no servidor (ver traduzirFiltrosRaca)
    filters: [
      { text: "Cão", value: 1 },
      { text: "Gato", value: 2 },
    ],
    filterMultiple: false,
    render: (especie) => <Tag color={especie === "Cao" ? "blue" : "green"}>{especie}</Tag>,
  },
  {
    title: "Peso Fêmea",
    key: "pesoFemea",
    render: (_, r) => formatarPeso(r.pesoMinimoReferenciaFemea, r.pesoMaximoReferenciaFemea),
  },
  {
    title: "Peso Macho",
    key: "pesoMacho",
    render: (_, r) => formatarPeso(r.pesoMinimoReferenciaMacho, r.pesoMaximoReferenciaMacho),
  },
  {
    title: "Status",
    dataIndex: "isAtivo",
    key: "isAtivo",
    // Filtro aplicado no servidor (ver traduzirFiltrosRaca)
    filters: [
      { text: "Ativa", value: true },
      { text: "Inativa", value: false },
    ],
    filterMultiple: false,
    render: (isAtivo) => (isAtivo ? <Tag color="green">Ativa</Tag> : <Tag color="red">Inativa</Tag>),
  },
  {
    title: "Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: true, // ordenação feita no servidor (padrão: mais recentes primeiro)
    render: (data) => (data ? new Date(data).toLocaleDateString("pt-BR") : "-"),
  },
];

/** Coluna extra da visão de cópias (raças próprias das clínicas). */
export const colunaClinica = {
  title: "Clínica",
  dataIndex: "nomeClinica",
  key: "nomeClinica",
};

/** Converte os filtros de coluna da Table em filtros do servidor (RacaFiltro). */
export const traduzirFiltrosRaca = (filtros) => ({
  especie: filtros.especie?.[0],
  ativo: filtros.isAtivo?.[0],
});

export default colunas;
