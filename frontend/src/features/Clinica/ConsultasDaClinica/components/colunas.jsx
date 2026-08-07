import React from "react";
import { Tag } from "antd";
import dayjs from "dayjs";

const colunas = [
  {
    title: "Data da Consulta",
    dataIndex: "dataConsulta",
    key: "dataConsulta",
    render: (data) => data ? dayjs(data).format("DD/MM/YYYY HH:mm") : '-',
    sorter: (a, b) =>
      dayjs(a.dataConsulta).unix() - dayjs(b.dataConsulta).unix(),
  },
  {
    title: "Paciente",
    dataIndex: "nomeAnimal",
    key: "nomeAnimal",
    sorter: (a, b) => (a.nomeAnimal || '').localeCompare(b.nomeAnimal || ''),
  },
  {
    title: "Tutor",
    dataIndex: "nomeTutor",
    key: "nomeTutor",
    sorter: (a, b) => (a.nomeTutor || '').localeCompare(b.nomeTutor || ''),
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
    render: (preenchido) => (
      <Tag color={preenchido ? "green" : "orange"}>
        {preenchido ? "Preenchido" : "Pendente"}
      </Tag>
    ),
    filters: [
      { text: "Preenchido", value: true },
      { text: "Pendente", value: false },
    ],
    onFilter: (value, record) => record.avaliacaoPreenchida === value,
  },
];

export default colunas;
