import { Tag } from "antd";

const colunas = [
    {
      title: "Nome",
      dataIndex: "nome",
      key: "nome",
      sorter: (a, b) => a.nome.localeCompare(b.nome),
    },
    {
      title: "Espécie",
      dataIndex: "especie",
      key: "especie",
      render: (especie) => (
        <Tag color={especie === "Cao" ? "blue" : "green"}>{especie}</Tag>
      ),
      filters: [ { text: "Cão", value: "Cao" }, { text: "Gato", value: "Gato" } ],
      onFilter: (value, record) => record.especie.indexOf(value) === 0,
    },
    {
      title: "Unidade",
      dataIndex: "isPorcentagem",
      key: "isPorcentagem",
      filters: [
        { text: 'Porcentagem (%)', value: true },
        { text: 'Gramas (g/kg)', value: false },
      ],
      onFilter: (value, record) => record.isPorcentagem === value,
      render: (isPorcentagem) => isPorcentagem ? <Tag>%</Tag> : <Tag>g/kg</Tag>,
    },
    {
      title: "Umidade",
      dataIndex: "umidade",
      key: "umidade",
      sorter: (a, b) => a.umidade - b.umidade,
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Proteína Bruta",
      dataIndex: "proteinaBruta",
      key: "proteinaBruta",
      sorter: (a, b) => a.proteinaBruta - b.proteinaBruta,
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Extrato Etéreo",
      dataIndex: "extratoEtereo",
      key: "extratoEtereo",
      sorter: (a, b) => a.extratoEtereo - b.extratoEtereo,
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Matéria Mineral",
      dataIndex: "materiaMineral",
      key: "materiaMineral",
      sorter: (a, b) => a.materiaMineral - b.materiaMineral,
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Fibra Bruta",
      dataIndex: "fibraBruta",
      key: "fibraBruta",
      sorter: (a, b) => a.fibraBruta - b.fibraBruta,
      render: (value, record) => `${value.toLocaleString("pt-BR")}${record.isPorcentagem ? '%' : ''}`,
    },
    {
      title: "Matéria Seca",
      dataIndex: "materiaSeca",
      key: "materiaSeca",
      sorter: (a, b) => a.materiaSeca - b.materiaSeca,
      render: (value) => `${value.toLocaleString("pt-BR")}%`,
    },
];

export default colunas;