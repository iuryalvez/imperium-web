import { Tag } from "antd";

const colunas = [
    {
      title: "Nome",
      dataIndex: "nome",
      key: "nome",
      sorter: true, // ordenação feita no servidor
      // Rações do sistema são compartilhadas entre as clínicas
      render: (nome, record) => (
        <>
          {nome} {record.isSistema && <Tag color="blue">Sistema</Tag>}
        </>
      ),
    },
    {
      title: "Espécie",
      dataIndex: "especie",
      key: "especie",
      render: (especie) => (
        <Tag color={especie === "Cao" ? "blue" : "green"}>{especie}</Tag>
      ),
      // Filtro aplicado no servidor (ver traduzirFiltrosRacao)
      filters: [ { text: "Cão", value: 1 }, { text: "Gato", value: 2 } ],
      filterMultiple: false,
    },
    {
      title: "Unidade",
      dataIndex: "isPorcentagem",
      key: "isPorcentagem",
      render: (isPorcentagem) => isPorcentagem ? <Tag>%</Tag> : <Tag>g/kg</Tag>,
    },
    {
      title: "Umidade",
      dataIndex: "umidade",
      key: "umidade",
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Proteína Bruta",
      dataIndex: "proteinaBruta",
      key: "proteinaBruta",
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Extrato Etéreo",
      dataIndex: "extratoEtereo",
      key: "extratoEtereo",
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Matéria Mineral",
      dataIndex: "materiaMineral",
      key: "materiaMineral",
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Fibra Bruta",
      dataIndex: "fibraBruta",
      key: "fibraBruta",
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Status",
      dataIndex: "isAtivo",
      key: "isAtivo",
      // Filtro aplicado no servidor (ver traduzirFiltrosRacao)
      filters: [
        { text: "Ativa", value: true },
        { text: "Inativa", value: false },
      ],
      filterMultiple: false,
      render: (isAtivo, record) => (
        <>
          {isAtivo ? <Tag color="green">Ativa</Tag> : <Tag color="red">Inativa</Tag>}
          {record.isOculta && <Tag>Oculta</Tag>}
        </>
      ),
    },
    {
      title: "Cadastro",
      dataIndex: "dataCadastro",
      key: "dataCadastro",
      sorter: true, // ordenação feita no servidor (padrão: mais recentes primeiro)
      render: (data) => (data ? new Date(data).toLocaleDateString("pt-BR") : "-"),
    },
    {
      title: "Matéria Seca",
      dataIndex: "materiaSeca",
      key: "materiaSeca",
      render: (value) => `${value.toLocaleString("pt-BR")}%`,
    },
];

/** Coluna extra da visão do admin para as rações próprias das clínicas ("cópias"). */
export const colunaClinica = {
  title: "Clínica",
  dataIndex: "nomeClinica",
  key: "nomeClinica",
};

/** Converte os filtros de coluna da Table em filtros do servidor (RacaoFiltro). */
export const traduzirFiltrosRacao = (filtros) => ({
  especie: filtros.especie?.[0],
  ativo: filtros.isAtivo?.[0],
});

export default colunas;