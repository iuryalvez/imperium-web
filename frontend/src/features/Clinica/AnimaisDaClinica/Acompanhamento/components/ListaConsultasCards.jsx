import React from "react";
import {
  Card,
  Button,
  Space,
  Typography,
  Empty,
  Popconfirm,
  Tooltip,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  CalculatorOutlined,
  ProfileOutlined,
  PlayCircleOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";

const { Text } = Typography;

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
  onIniciarDieta,
  onConcluirDieta,
}) => {
  if (!consultas || consultas.length === 0) {
    return (
      <Empty description="Nenhuma consulta encontrada para este animal." />
    );
  }

  return (
    <div className="flex flex-col gap-3">
      {consultas.map((consulta) => {
        const hasAvaliacao = !!consulta.avaliacao;
        const hasDieta = hasAvaliacao && !!consulta.avaliacao.dieta;

        return (
          <Card
            key={consulta.id}
            size="small"
            className="shadow-sm border-gray-200"
            styles={{ body: { padding: "8px 12px" } }}
            title={
              <div className="flex justify-between items-center w-full">
                <span>
                  Consulta em{" "}
                  {dayjs(consulta.dataConsulta).format("DD/MM/YYYY HH:mm")}
                </span>
                <span className="font-normal text-sm text-gray-500">
                  Profissional: {consulta.profissionalResponsavel}
                </span>
              </div>
            }
          >
            <div className="flex flex-col gap-1.5">
              {/* Gerenciamento da Consulta */}
              <div className="flex items-center gap-2 flex-wrap">
                <Text strong className="w-24">
                  Consulta:
                </Text>
                <Space>
                  <Tooltip
                    title={
                      hasAvaliacao
                        ? "Não é possível editar uma consulta com avaliação preenchida"
                        : ""
                    }
                  >
                    <div>
                      <Button
                        size="small"
                        icon={<EditOutlined />}
                        onClick={() => onEditConsulta(consulta)}
                        disabled={hasAvaliacao}
                      >
                        Editar Consulta
                      </Button>
                    </div>
                  </Tooltip>
                  <Tooltip
                    title={
                      hasAvaliacao
                        ? "Não é possível excluir uma consulta com avaliação preenchida"
                        : ""
                    }
                  >
                    <div>
                      <Popconfirm
                        title="Excluir Consulta"
                        description="Tem certeza que deseja excluir esta consulta?"
                        onConfirm={() => onDeleteConsulta(consulta.id)}
                        okText="Sim"
                        cancelText="Não"
                        disabled={hasAvaliacao}
                      >
                        <Button
                          size="small"
                          danger
                          icon={<DeleteOutlined />}
                          disabled={hasAvaliacao}
                        >
                          Excluir Consulta
                        </Button>
                      </Popconfirm>
                    </div>
                  </Tooltip>
                </Space>
              </div>

              {/* Gerenciamento da Avaliação */}
              <div className="flex items-center gap-2 flex-wrap">
                <Text strong className="w-24">
                  Avaliação:
                </Text>
                <Space>
                  <Tooltip
                    title={
                      hasDieta
                        ? "Não é possível editar uma avaliação com dieta preenchida"
                        : ""
                    }
                  >
                    <div>
                      <Button
                        size="small"
                        type={hasAvaliacao ? "default" : "primary"}
                        icon={<EditOutlined />}
                        onClick={() => onOpenAvaliacao(consulta)}
                        disabled={hasDieta}
                      >
                        {hasAvaliacao ? "Editar Avaliação" : "Preencher Avaliação"}
                      </Button>
                    </div>
                  </Tooltip>
                  {hasAvaliacao && (
                    <Tooltip
                      title={
                        hasDieta
                          ? "Não é possível excluir uma avaliação com dieta preenchida"
                          : ""
                      }
                    >
                      <div>
                        <Popconfirm
                          title="Excluir avaliação"
                          description="Tem certeza que deseja excluir esta avaliação?"
                          onConfirm={() =>
                            onDeleteAvaliacao(consulta.avaliacao.id)
                          }
                          okText="Sim"
                          cancelText="Não"
                          disabled={hasDieta}
                        >
                          <Button
                            size="small"
                            danger
                            icon={<DeleteOutlined />}
                            disabled={hasDieta}
                          >
                            Excluir Avaliação
                          </Button>
                        </Popconfirm>
                      </div>
                    </Tooltip>
                  )}
                </Space>
              </div>

              {/* Gerenciamento da Dieta */}
              {hasAvaliacao && (
                <div className="flex items-center gap-2 flex-wrap">
                  <Text strong className="w-24">
                    Dieta:
                  </Text>
                  <Space>
                    {!hasDieta ? (
                      <Button
                        size="small"
                        type="primary"
                        icon={<CalculatorOutlined />}
                        className="bg-verde-escuro hover:bg-verde-medio border-none"
                        onClick={() =>
                          onOpenModalCriarDieta(consulta.avaliacao)
                        }
                      >
                        Calcular Dieta
                      </Button>
                    ) : (
                      <>
                        <Button
                          size="small"
                          icon={<ProfileOutlined />}
                          onClick={() =>
                            onOpenModalVisualizarDieta(consulta.avaliacao.dieta)
                          }
                        >
                          Ver Dieta
                        </Button>
                        <Button
                          size="small"
                          icon={<EditOutlined />}
                          disabled={consulta.avaliacao.dieta.status === 2 || consulta.avaliacao.dieta.status === 3}
                          onClick={() =>
                            onOpenModalEditarDieta(
                              consulta.avaliacao.dieta,
                              consulta.avaliacao,
                            )
                          }
                        >
                          Editar Cálculo
                        </Button>

                        {consulta.avaliacao.dieta.status === 1 && (
                          <Popconfirm
                            title="Iniciar Dieta"
                            description="Deseja iniciar esta dieta? Dietas anteriores em andamento serão concluídas."
                            onConfirm={() => onIniciarDieta(consulta.avaliacao.dieta.id)}
                            okText="Sim"
                            cancelText="Não"
                          >
                            <Button size="small" type="primary" className="bg-blue-600 hover:bg-blue-500 border-none" icon={<PlayCircleOutlined />}>
                              Iniciar
                            </Button>
                          </Popconfirm>
                        )}

                        {consulta.avaliacao.dieta.status === 2 && (
                          <Popconfirm
                            title="Concluir Dieta"
                            description="Deseja concluir esta dieta?"
                            onConfirm={() => onConcluirDieta(consulta.avaliacao.dieta.id)}
                            okText="Sim"
                            cancelText="Não"
                          >
                            <Button size="small" type="primary" className="bg-green-600 hover:bg-green-500 border-none" icon={<CheckCircleOutlined />}>
                              Concluir
                            </Button>
                          </Popconfirm>
                        )}
                        <Popconfirm
                          title="Excluir dieta"
                          description="Tem certeza que deseja excluir esta dieta?"
                          onConfirm={() =>
                            onExcluirDieta(consulta.avaliacao.dieta.id)
                          }
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
