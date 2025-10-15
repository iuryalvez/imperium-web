// src/pages/AcompanhamentoAnimal/components/ListaRelatorios.jsx

import React from "react";
import { List, Button, Tag, Descriptions, Space, Popconfirm } from "antd"; // Adiciona Space
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from "@ant-design/icons"; // Adiciona EyeOutlined
import dayjs from "dayjs";

const ListaRelatorios = ({
  relatorios,
  onOpenModalCriarDieta,
  onOpenModalVisualizarDieta,
  onOpenModalEditarDieta,
  onExcluirDieta,
}) => {
  return (
    <List
      itemLayout="vertical"
      dataSource={relatorios}
      renderItem={(relatorio) => {
        // Lógica para definir quais botões mostrar
        const actions = [];
        if (relatorio.dieta) {
          // Se tem dieta, mostra o grupo de botões de gestão
          actions.push(
            <Space key="diet-actions">
              <Button
                icon={<EyeOutlined />}
                onClick={() => onOpenModalVisualizarDieta(relatorio.dieta)}
              >
                Visualizar
              </Button>
              <Button
                icon={<EditOutlined />}
                onClick={() => onOpenModalEditarDieta(relatorio.dieta)}
              >
                Editar
              </Button>
              <Popconfirm
                title="Excluir a Dieta"
                description="Tem certeza que deseja excluir esta dieta? Esta ação não pode ser desfeita."
                onConfirm={() => onExcluirDieta(relatorio.dieta.id)}
                okText="Sim, Excluir"
                cancelText="Cancelar"
              >
                <Button
                  icon={<DeleteOutlined />}
                  danger
                >
                  Excluir
                </Button>
              </Popconfirm>
            </Space>
          );
        } else {
          // Se não tem dieta, mostra o botão de calcular
          actions.push(
            <Button
              key="create"
              icon={<PlusOutlined />}
              onClick={() => onOpenModalCriarDieta(relatorio)}
            >
              Calcular Dieta
            </Button>
          );
        }

        return (
          <List.Item
            key={relatorio.id}
            actions={actions} // Usa o array de ações definido acima
          >
            <Descriptions
              title={`Consulta de ${dayjs(relatorio.dataConsulta).format(
                "DD/MM/YYYY"
              )}`}
              size="small"
              column={2}
              bordered
            >
              <Descriptions.Item label="Profissional">
                {relatorio.profissionalResponsavel}
              </Descriptions.Item>
              <Descriptions.Item label="Peso Registrado">
                {`${relatorio.peso} kg`}
              </Descriptions.Item>
              <Descriptions.Item label="Condição Corporal">
                <Tag>{relatorio.condicaoCorporal}</Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Grau de Atividade">
                <Tag>{relatorio.grauAtividade}</Tag>
              </Descriptions.Item>
            </Descriptions>
            {/* A renderização direta do <DetalhesDieta /> é removida daqui */}
          </List.Item>
        );
      }}
    />
  );
};

export default ListaRelatorios;
