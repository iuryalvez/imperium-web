import React from "react";
import { Button, Tag, Tooltip } from "antd";
import { LineChartOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";
import dayjs from "dayjs";

// Status calculado no backend: só "Cancelada" é gravado; os demais vêm de avaliação e dieta
export const STATUS_CONSULTA = {
  Pendente: { texto: "Pendente", cor: "gold" },
  EmAndamento: { texto: "Em andamento", cor: "blue" },
  Concluida: { texto: "Concluída", cor: "green" },
  Cancelada: { texto: "Cancelada", cor: "red" },
};

const colunas = [
  {
    title: "Data da Consulta",
    dataIndex: "dataConsulta",
    key: "dataConsulta",
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY HH:mm") : "-"),
    sorter: true, // ordenação feita no servidor (padrão: mais recentes primeiro)
  },
  {
    title: "Paciente",
    dataIndex: "nomeAnimal",
    key: "nomeAnimal",
    sorter: true, // ordenação feita no servidor
  },
  {
    title: "Tutor",
    dataIndex: "nomeTutor",
    key: "nomeTutor",
  },
  {
    title: "Veterinário",
    dataIndex: "nomeUsuario",
    key: "nomeUsuario",
  },
  {
    title: "Avaliação",
    dataIndex: "avaliacaoPreenchida",
    key: "avaliacaoPreenchida",
    // O link usa o id do ACOMPANHAMENTO (a rota /acompanhamento/:acompanhamentoId); sem acompanhamento, fica desabilitado
    render: (preenchido, record) =>
      !record.acompanhamentoId ? (
        <Button size="small" disabled style={{ width: "110px" }}>
          Sem acompanhamento
        </Button>
      ) : preenchido ? (
        <Tooltip title="Ir para Acompanhamento">
          <Link to={`/clinica/animais/acompanhamento/${record.acompanhamentoId}`}>
            <Button
              size="small"
              type="primary"
              icon={<LineChartOutlined />}
              style={{ width: "110px" }}
            >
              Preenchido
            </Button>
          </Link>
        </Tooltip>
      ) : (
        <Tooltip title="Ir para Acompanhamento">
          <Link to={`/clinica/animais/acompanhamento/${record.acompanhamentoId}`}>
            <Button
              size="small"
              type="dashed"
              icon={<LineChartOutlined />}
              style={{ width: "110px" }}
            >
              Pendente
            </Button>
          </Link>
        </Tooltip>
      ),
  },
  {
    title: "Status",
    dataIndex: "status",
    key: "status",
    // Filtro aplicado no servidor (ver traduzirFiltrosConsulta)
    filters: Object.entries(STATUS_CONSULTA).map(([value, { texto }]) => ({ text: texto, value })),
    filterMultiple: false,
    render: (status, record) => {
      const info = STATUS_CONSULTA[status] || { texto: status, cor: "default" };
      return (
        <Tooltip title={status === "Cancelada" && record.motivoCancelamento ? `Motivo: ${record.motivoCancelamento}` : ""}>
          <Tag color={info.cor}>{info.texto}</Tag>
        </Tooltip>
      );
    },
  },
];

/** Converte os filtros de coluna da Table em filtros do servidor (ConsultaFiltro). */
export const traduzirFiltrosConsulta = (filtros) => ({
  status: filtros.status?.[0],
});

export default colunas;
