import { Tag } from "antd";
import { formatarEspecie, formatarSexo } from "../../../../../utils/formatters";
import dayjs from "dayjs";

// Definimos a lista completa de colunas com base no GetAnimaisClinicaDto.
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
      render: (especieId) => formatarEspecie(especieId),
      filters: [ { text: "Cão", value: 1 }, { text: "Gato", value: 2 } ],
      filterMultiple: false, // filtro aplicado no servidor
    },
    {
      title: "Raça",
      dataIndex: "racaNome",
      key: "racaNome",
      render: (raca) => raca || 'Não informada',
    },
    {
      title: "Sexo",
      dataIndex: "sexo",
      key: "sexo",
      render: (sexoId) => formatarSexo(sexoId),
    },
    {
        title: "Castrado",
        dataIndex: "isCastrado",
        key: "isCastrado",
        render: (isCastrado) => <Tag color={isCastrado ? "cyan" : "orange"}>{isCastrado ? "Sim" : "Não"}</Tag>,
    },
    {
      title: "Tutores",
      dataIndex: "tutores",
      key: "tutores",
      render: (tutores) => {
          if (!tutores || tutores.length === 0) return <Tag>Nenhum tutor</Tag>;
          // Mostra o nome do primeiro tutor e "+ X" se houver mais
          const primeiroTutor = tutores[0].nome;
          const outrosCount = tutores.length - 1;
          return `${primeiroTutor}${outrosCount > 0 ? ` (+${outrosCount})` : ''}`;
      }
    },
    {
      title: "Data de Cadastro",
      dataIndex: "dataCadastro",
      key: "dataCadastro",
      sorter: true, // ordenação feita no servidor
      render: (data) => data ? dayjs(data).format("DD/MM/YYYY") : '-',
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

/** Converte os filtros de coluna da Table em filtros do servidor (AnimalFiltro). */
export const traduzirFiltrosAnimal = (filtros) => ({
  especie: filtros.especie?.[0],
  ativo: filtros.isAtivo?.[0],
});

export default colunas;