import React from "react";
import { Table, Button, Tag, Menu, Dropdown } from "antd";
import {
  MoreOutlined,
  LineChartOutlined,
  CalculatorOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";

const TabelaConsultas = ({
  consultas,
  onEdit,
  onDelete,
  onOpenAvaliacao,
  onVerDieta,
  onCalcularDieta,
  onIrParaAcompanhamento,
  loading,
}) => {
  const colunas = [
    {
      title: "Data da Consulta",
      dataIndex: "dataConsulta",
      render: (data) => dayjs(data).format("DD/MM/YYYY HH:mm"),
      sorter: (a, b) =>
        dayjs(a.dataConsulta).unix() - dayjs(b.dataConsulta).unix(),
    },
    {
      title: "Paciente",
      dataIndex: "nomeAnimal",
      sorter: (a, b) => a.nomeAnimal.localeCompare(b.nomeAnimal),
    },
    {
      title: "Tutor",
      dataIndex: "nomeTutor",
      sorter: (a, b) => a.nomeTutor.localeCompare(b.nomeTutor),
    },
    { title: "Veterinário", dataIndex: "nomeUsuario" },
    {
      title: "Avaliação",
      dataIndex: "avaliacaoPreenchida",
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
    {
      title: "Ações",
      key: "acoes",
      render: (
        _,
        consulta,
      ) => (
        <Dropdown
          overlay={
            <Menu>
              <Menu.Item
                key="edit-avaliacao"
                onClick={() => onOpenAvaliacao(consulta)}
              >
                {consulta.avaliacaoPreenchida
                  ? "Editar Avaliação Clínica"
                  : "Preencher Avaliação Clínica"}
              </Menu.Item>

              <Menu.Divider />

              {/* Se o DTO mandar a prop dietaId preenchida, mostramos Ver. Se não, Calcular. */}
              {consulta.dietaId ? (
                <Menu.Item
                  key="view-diet"
                  onClick={() => onVerDieta(consulta)}
                >
                  Ver Dieta Prescrita
                </Menu.Item>
              ) : (
                <Menu.Item
                  key="calc-diet"
                  icon={<CalculatorOutlined />}
                  onClick={() => onCalcularDieta(consulta)}
                  disabled={!consulta.avaliacaoPreenchida}
                >
                  Calcular Dieta
                </Menu.Item>
              )}

              <Menu.Divider />

              <Menu.Item
                key="timeline"
                icon={<LineChartOutlined />}
                onClick={() => onIrParaAcompanhamento(consulta.animalId)}
              >
                Ver Acompanhamento
              </Menu.Item>
            </Menu>
          }
        >
          <Button type="text" icon={<MoreOutlined />} />
        </Dropdown>
      ),
    },
  ];

  return (
    <Table
      dataSource={consultas}
      columns={colunas}
      rowKey="id"
      loading={loading}
    />
  );
};

export default TabelaConsultas;
