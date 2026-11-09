import React from "react";
import { Card, Typography, Row, Col, Button, Collapse, Space, Tooltip } from "antd";
import {
  ManOutlined,
  WomanOutlined,
  ProfileOutlined,
  EditOutlined,
} from "@ant-design/icons";

const { Title, Text } = Typography;
const { Panel } = Collapse;

const FichaGeralAnimal = ({
  data,
  avaliacaoRecente,
  dietaRecente,
  onOpenDieta,
  onEditAnimal,
}) => {
  if (!data) return null;

  const nomeRaca = data.raca?.nome || "SRD";


  return (
    <Card
      className="shadow-sm border-cinza-desativado"
      styles={{ body: { padding: "16px 24px" } }}
    >
      {/* O nome do animal já está no título da página */}
      <div className="flex flex-wrap justify-between items-center mb-4 gap-3">
        <Title level={4} className="text-verde-escuro m-0">
          Ficha Geral
        </Title>
        <Space wrap>
          {onEditAnimal && (
            <Tooltip title="Editar animal">
              <Button
                icon={<EditOutlined />}
                aria-label="Editar animal"
                onClick={() => onEditAnimal(data)}
              />
            </Tooltip>
          )}
          {dietaRecente && (
            <Button
              type="primary"
              icon={<ProfileOutlined />}
              onClick={() => onOpenDieta(dietaRecente)}
            >
              Ver plano nutricional atual
            </Button>
          )}
        </Space>
      </div>

      <Collapse ghost defaultActiveKey={["1"]}>
        <Panel
          header={
            <Text strong className="text-gray-500">
              Detalhes do Paciente
            </Text>
          }
          key="1"
        >
          {/* >= lg a ficha fica na coluna lateral estreita: as duas colunas de dados viram uma */}
          <Row gutter={[32, 8]}>
            {/* Informações Básicas */}
            <Col xs={24} md={12} xl={24}>
              <div className="flex flex-col gap-2">
                <Text>
                  <strong className="text-verde-escuro">Espécie:</strong>{" "}
                  <span className="text-gray-900">
                    {data.especie === "Cao" ? "Cão" : "Gato"}
                  </span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Raça:</strong>{" "}
                  <span className="text-gray-900">{nomeRaca}</span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Idade:</strong>{" "}
                  <span className="text-gray-900">{data.idadeFormatada}</span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Fase da Vida:</strong>{" "}
                  <span className="text-gray-900">
                    {data.faseVidaAtualLabel || "Não definida"}
                  </span>
                </Text>
              </div>
            </Col>

            {/* Informações Clínicas Mais Recentes */}
            <Col xs={24} md={12} xl={24}>
              <div className="flex flex-col gap-2">
                <Text className="flex items-center gap-1">
                  <strong className="text-verde-escuro">Sexo:</strong>{" "}
                  <span className="text-gray-900">{data.sexo === "Femea" ? "Fêmea" : data.sexo}</span>
                  {data.sexo === "Macho" ? (
                    <ManOutlined className="text-blue-500" />
                  ) : (
                    <WomanOutlined className="text-pink-500" />
                  )}
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Peso Atual:</strong>{" "}
                  <span className="text-gray-900">
                    {data?.pesoAtual
                      ? `${data.pesoAtual} kg`
                      : "Não avaliado"}
                  </span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">
                    Escore Corporal (ECC):
                  </strong>{" "}
                  <span className="text-gray-900">
                    {data.condicaoCorporalAtualLabel || "Não avaliado"}
                  </span>
                </Text>
                {data.especieAnimal === 1 && (
                  <Text>
                    <strong className="text-verde-escuro">
                      Grau de Atividade:
                    </strong>{" "}
                    <span className="text-gray-900">
                      {avaliacaoRecente?.grauAtividade || "Não avaliado"}
                    </span>
                  </Text>
                )}
              </div>
            </Col>
          </Row>
        </Panel>
      </Collapse>
    </Card>
  );
};

export default FichaGeralAnimal;
