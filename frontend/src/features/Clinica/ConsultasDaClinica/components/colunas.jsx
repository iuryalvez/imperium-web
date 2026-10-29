import React from "react";
import { Button, Tooltip } from "antd";
import { LineChartOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";
import dayjs from "dayjs";

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
    render: (preenchido, record) =>
      preenchido ? (
        <Tooltip title="Ir para Acompanhamento">
          <Link to={`/clinica/animais/acompanhamento/${record.animalId}`}>
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
          <Link to={`/clinica/animais/acompanhamento/${record.animalId}`}>
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
];

export default colunas;
