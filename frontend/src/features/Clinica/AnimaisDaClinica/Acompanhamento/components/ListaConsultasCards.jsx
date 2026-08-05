import React from "react";
import { Card, Button, Space, Typography, Empty, Popconfirm } from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  FileTextOutlined,
  CalculatorOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";

const { Text, Title } = Typography;

const ListaConsultasCards = ({
  consultas,
  onEditConsulta,
  onDeleteConsulta,
  onOpenAvaliacao,
  onDeleteAvaliacao,
  onOpenModalCriarDieta,
  onOpenModalVisualizarDieta,
  onOpenModalEditarDieta,
  onExcluirDieta,
}) => {
  if (!consultas || consultas.length === 0) {
    return <Empty description="Nenhuma consulta encontrada para este animal." />;
  }

  return (
    <div className="flex flex-col gap-4">
      {consultas.map((consulta) => {
        const hasAvaliacao = !!consulta.avaliacao;
        const hasDieta = hasAvaliacao && !!consulta.avaliacao.dieta;

        return (
          <Card
            key={consulta.id}
            size="small"
            className="shadow-sm border-gray-200"
            title={
              <div className="flex justify-between items-center w-full">
                <span>
                  Consulta em {dayjs(consulta.dataConsulta).format("DD/MM/YYYY HH:mm")}
                </span>
                <span className="font-normal text-sm text-gray-500">
                  Profissional: {consulta.profissionalResponsavel}
                </span>
              </div>
            }
          >
            <div className="flex flex-col gap-3">
              {/* Gerenciamento da Consulta */}
              <div className="flex items-center gap-2 flex-wrap">
                <Text strong className="w-24">Consulta:</Text>
                <Space>
                  <Button
                    size="small"
                    icon={<EditOutlined />}
                    onClick={() => onEditConsulta(consulta)}
                  >
                    Editar Consulta
                  </Button>
                  <Popconfirm
                    title="Excluir Consulta"
                    description="Tem certeza que deseja excluir esta consulta?"
                    onConfirm={() => onDeleteConsulta(consulta.id)}
                    okText="Sim"
                    cancelText="Não"
                  >
                    <Button size="small" danger icon={<DeleteOutlined />}>
                      Excluir Consulta
                    </Button>
                  </Popconfirm>
                </Space>
              </div>

              {/* Gerenciamento da Avaliação */}
              <div className="flex items-center gap-2 flex-wrap">
                <Text strong className="w-24">Avaliação:</Text>
                <Space>
                  <Button
                    size="small"
                    type={hasAvaliacao ? "default" : "primary"}
                    icon={<EditOutlined />}
                    onClick={() => onOpenAvaliacao(consulta)}
                  >
                    {hasAvaliacao ? "Editar Avaliação" : "Preencher Avaliação"}
                  </Button>
                  {hasAvaliacao && (
                    <Popconfirm
                      title="Excluir avaliação"
                      description="Tem certeza que deseja excluir esta avaliação?"
                      onConfirm={() => onDeleteAvaliacao(consulta.avaliacao.id)}
                      okText="Sim"
                      cancelText="Não"
                    >
                      <Button size="small" danger icon={<DeleteOutlined />}>
                        Excluir Avaliação
                      </Button>
                    </Popconfirm>
                  )}
                </Space>
              </div>

              {/* Gerenciamento da Dieta */}
              {hasAvaliacao && (
                <div className="flex items-center gap-2 flex-wrap">
                  <Text strong className="w-24">Dieta:</Text>
                  <Space>
                    {!hasDieta ? (
                      <Button
                        size="small"
                        type="primary"
                        icon={<CalculatorOutlined />}
                        className="bg-verde-escuro hover:bg-verde-medio border-none"
                        onClick={() => onOpenModalCriarDieta(consulta.avaliacao)}
                      >
                        Calcular Dieta
                      </Button>
                    ) : (
                      <>
                        <Button
                          size="small"
                          icon={<ProfileOutlined />}
                          onClick={() => onOpenModalVisualizarDieta(consulta.avaliacao.dieta)}
                        >
                          Ver Dieta
                        </Button>
                        <Button
                          size="small"
                          icon={<EditOutlined />}
                          onClick={() => onOpenModalEditarDieta(consulta.avaliacao.dieta)}
                        >
                          Editar Cálculo
                        </Button>
                        <Popconfirm
                          title="Excluir dieta"
                          description="Tem certeza que deseja excluir esta dieta?"
                          onConfirm={() => onExcluirDieta(consulta.avaliacao.dieta.id)}
                          okText="Sim"
                          cancelText="Não"
                        >
                          <Button size="small" danger icon={<DeleteOutlined />}>
                            Excluir Dieta
                          </Button>
                        </Popconfirm>
                      </>
                    )}
                  </Space>
                </div>
              )}
            </div>
          </Card>
        );
      })}
    </div>
  );
};

export default ListaConsultasCards;
