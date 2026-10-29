import dayjs from "dayjs";
import { perfisSistema, renderPerfis } from "../../../../../utils/renderers";
import { formatarCPF, formatarTelefone } from "../../../../../utils/formatters";
import { Tag } from "antd";

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
    title: "Perfis",
    dataIndex: "perfis",
    key: "perfis",
    filters: perfisSistema.map((p) => ({ text: p.label, value: p.value })),
    filterMultiple: false, // filtro aplicado no servidor
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
    filterMultiple: false, // filtro aplicado no servidor
    render: (isAtivoNaClinica) =>
      isAtivoNaClinica ? <Tag color="green">Ativo</Tag> : <Tag color="red">Inativo</Tag>,
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: true, // ordenação feita no servidor
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY") : "-"),
  },
  {
    title: "Último Acesso",
    dataIndex: "dataUltimoAcesso",
    key: "dataUltimoAcesso",
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

/** Converte os filtros de coluna da Table em filtros do servidor (UsuarioClinicaFiltro). */
export const traduzirFiltrosUsuarioClinica = (filtros) => ({
  perfil: filtros.perfis?.[0],
  ativo: filtros.isAtivoNaClinica?.[0],
});

export default colunas;