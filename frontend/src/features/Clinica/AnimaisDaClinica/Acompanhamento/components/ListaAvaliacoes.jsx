import React from "react";
import {
  List,
  Button,
  Tag,
  Space,
  Popconfirm,
  Row,
  Col,
  Typography,
} from "antd";
import { EyeOutlined, EditOutlined, DeleteOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

import ImperiumIconSrc from "../../../../../assets/imperium-icon.png";

const { Text } = Typography;

const ListaAvaliacoes= ({
  avaliacoes,
  onOpenModalCriarDieta,
  onOpenModalVisualizarDieta,
  onOpenModalEditarDieta,
  onExcluirDieta,
}) => {
  return (
    <List
      itemLayout="vertical"
      dataSource={avaliacoes}
      renderItem={(avaliacao) => {
        const actions = [];
        if (avaliacao.dieta) {
          actions.push(
            <Space className="pl-4" key="diet-actions">
              <Button
                icon={<EyeOutlined />}
                onClick={() => onOpenModalVisualizarDieta(avaliacao.dieta)}
              >
                Visualizar Dieta
              </Button>
              <Button
                icon={<EditOutlined />}
                onClick={() => onOpenModalEditarDieta(avaliacao.dieta)}
              >
                Editar
              </Button>
              <Popconfirm
                title="Excluir a Dieta"
                description="Tem certeza que deseja excluir esta dieta? Esta ação não pode ser desfeita."
                onConfirm={() => onExcluirDieta(avaliacao.dieta.id)}
                okText="Sim, Excluir"
                cancelText="Cancelar"
              >
                <Button icon={<DeleteOutlined />} danger>
                  Excluir
                </Button>
              </Popconfirm>
            </Space>,
          );
        } else {
          actions.push(
            <div className="pl-4">
              <Button
                key="create"
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
              ,
            </div>,
          );
        }

        return (
          <List.Item
            key={avaliacao.id}
            actions={actions}
            className="bg-white px-5 py-4 mb-4 rounded-md shadow-sm border border-gray-200"
          >
            <div className="mb-3">
              <span className="text-verde-escuro font-bold text-base pl-4">
                Consulta de {dayjs(avaliacao.dataConsulta).format("DD/MM/YYYY")}
              </span>
            </div>

            {/* Estrutura refatorada usando Row e Col */}
            <div className="bg-gray-50 border border-gray-100 rounded-md p-4">
              <Row gutter={[16, 16]}>
                <Col xs={24} sm={12} md={6}>
                  <Text className="block text-verde-escuro font-medium text-xs uppercase mb-1">
                    Profissional
                  </Text>
                  <Text className="text-gray-700">
                    {avaliacao.profissionalResponsavel}
                  </Text>
                </Col>

                <Col xs={24} sm={12} md={6}>
                  <Text className="block text-verde-escuro font-medium text-xs uppercase mb-1">
                    Peso Registrado
                  </Text>
                  <Text className="text-gray-700">
                    {`${avaliacao.peso} kg`}
                  </Text>
                </Col>

                <Col xs={24} sm={12} md={6}>
                  <Text className="block text-verde-escuro font-medium text-xs uppercase mb-1">
                    Condição Corporal
                  </Text>
                  <Tag color="orange" className="m-0">
                    {avaliacao.condicaoCorporal}
                  </Tag>
                </Col>

                <Col xs={24} sm={12} md={6}>
                  <Text className="block text-verde-escuro font-medium text-xs uppercase mb-1">
                    Grau de Atividade
                  </Text>
                  <Tag className="m-0 bg-white border-gray-200 text-gray-600">
                    {avaliacao.grauAtividade}
                  </Tag>
                </Col>
              </Row>
            </div>
          </List.Item>
        );
      }}
    />
  );
};

export default ListaAvaliacoes;
