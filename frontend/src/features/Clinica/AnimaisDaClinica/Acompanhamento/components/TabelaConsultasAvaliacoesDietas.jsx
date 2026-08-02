import React from "react";
import { Table, Button, Tag, Space, Popconfirm, Typography } from "antd";
import { EyeOutlined, EditOutlined, DeleteOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

import ImperiumIconSrc from "../../../../../assets/imperium-icon.png";

const { Text } = Typography;

const TabelaConsultasAvaliacoesDietas = ({
  avaliacoes,
  onOpenAvaliacao,
  onOpenModalCriarDieta,
  onOpenModalVisualizarDieta,
  onOpenModalEditarDieta,
  onExcluirDieta,
}) => {
  const columns = [
    {
      title: "Data da Consulta",
      dataIndex: "dataConsulta",
      key: "dataConsulta",
      render: (text) => (
        <Text className="text-verde-escuro font-bold">
          {dayjs(text).format("DD/MM/YYYY")}
        </Text>
      ),
      sorter: (a, b) => new Date(a.dataConsulta) - new Date(b.dataConsulta),
      defaultSortOrder: "descend",
    },
    {
      title: "Profissional",
      dataIndex: "profissionalResponsavel",
      key: "profissionalResponsavel",
    },
    {
      title: "Peso",
      dataIndex: "peso",
      key: "peso",
      render: (peso) => `${peso} kg`,
    },
    {
      title: "Condição Corporal",
      dataIndex: "condicaoCorporal",
      key: "condicaoCorporal",
      render: (condicao) => <Tag color="orange">{condicao}</Tag>,
    },
    {
      title: "Grau de Atividade",
      dataIndex: "grauAtividade",
      key: "grauAtividade",
      render: (grau) => (
        grau ? <Tag className="bg-white border-gray-200 text-gray-600">{grau}</Tag> : "-"
      ),
    },
    {
      title: "Avaliação",
      key: "avaliacao",
      render: (_, avaliacao) => (
        <Button
          type="link"
          icon={<EditOutlined />}
          onClick={() => onOpenAvaliacao(avaliacao)}
        >
          {avaliacao.condicaoCorporal ? "Editar Avaliação" : "Preencher Avaliação"}
        </Button>
      ),
    },
    {
      title: "Dieta",
      key: "dietaActions",
      render: (_, avaliacao) => {
        if (avaliacao.dieta) {
          return (
            <Space size="middle">
              <Button
                type="link"
                icon={<EyeOutlined />}
                onClick={() => onOpenModalVisualizarDieta(avaliacao.dieta)}
              >
                Visualizar
              </Button>
              <Button
                type="link"
                icon={<EditOutlined />}
                onClick={() => onOpenModalEditarDieta(avaliacao.dieta)}
              >
                Editar
              </Button>
              <Popconfirm
                title="Excluir a Dieta"
                description="Tem certeza que deseja excluir esta dieta?"
                onConfirm={() => onExcluirDieta(avaliacao.dieta.id)}
                okText="Sim"
                cancelText="Não"
              >
                <Button type="link" danger icon={<DeleteOutlined />}>
                  Excluir
                </Button>
              </Popconfirm>
            </Space>
          );
        }

        return (
          <Button
            type="primary"
            className="bg-verde-medio hover:bg-verde-realce border-none"
            icon={
              <img
                src={ImperiumIconSrc}
                alt="Calcular"
                className="w-4 h-4 object-contain brightness-0 invert"
              />
            }
            onClick={() => onOpenModalCriarDieta(avaliacao)}
          >
            Calcular Dieta
          </Button>
        );
      },
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={avaliacoes}
      rowKey="id"
      pagination={{ pageSize: 5 }}
      className="shadow-sm border border-gray-200 rounded-md bg-white"
    />
  );
};

export default TabelaConsultasAvaliacoesDietas;
