import { Tag } from "antd";
import { formatarEspecie, formatarSexo } from "../../../../../utils/formatters";
import dayjs from "dayjs";

// Definimos a lista completa de colunas com base no GetAnimaisClinicaDto.
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
      render: (especieId) => formatarEspecie(especieId),
      filters: [ { text: "Cão", value: 1 }, { text: "Gato", value: 2 } ],
      onFilter: (value, record) => record.especie === value,
    },
    {
      title: "Raça",
      dataIndex: "racaNome",
      key: "racaNome",
      sorter: (a, b) => (a.racaNome || '').localeCompare(b.racaNome || ''),
      render: (raca) => raca || 'Não informada',
    },
    {
      title: "Sexo",
      dataIndex: "sexo",
      key: "sexo",
      render: (sexoId) => formatarSexo(sexoId),
      filters: [ { text: "Macho", value: 1 }, { text: "Fêmea", value: 2 } ],
      onFilter: (value, record) => record.sexo === value,
    },
    {
        title: "Castrado",
        dataIndex: "isCastrado",
        key: "isCastrado",
        render: (isCastrado) => <Tag color={isCastrado ? "cyan" : "orange"}>{isCastrado ? "Sim" : "Não"}</Tag>,
        filters: [ { text: "Sim", value: true }, { text: "Não", value: false } ],
        onFilter: (value, record) => record.isCastrado === value,
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
      sorter: (a, b) => dayjs(a.dataCadastro).unix() - dayjs(b.dataCadastro).unix(),
      render: (data) => data ? dayjs(data).format("DD/MM/YYYY") : '-',
    },
];

export default colunas;