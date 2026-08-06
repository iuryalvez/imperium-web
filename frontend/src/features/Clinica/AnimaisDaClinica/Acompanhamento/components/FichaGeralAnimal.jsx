import React from "react";
import { Card, Typography, Row, Col, Button, Collapse, Space } from "antd";
import { ManOutlined, WomanOutlined, ProfileOutlined, EditOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

const { Title, Text } = Typography;
const { Panel } = Collapse;

const FichaGeralAnimal = ({ animal, avaliacaoRecente, dietaRecente, onOpenDieta, onEditAnimal }) => {
  if (!animal) return null;

  const nomeRaca = animal.raca?.nome || "SRD";
  const dataAvaliacao = avaliacaoRecente?.dataConsulta
    ? dayjs(avaliacaoRecente.dataConsulta).format("DD/MM/YYYY")
    : "Sem registros";

  return (
    <Card
      className="shadow-sm border-cinza-desativado"
    >
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 gap-4">
        <Title
          level={3}
          className="text-verde-escuro m-0"
        >
          Ficha Geral - {animal.nome}
        </Title>
        <Space wrap>
          <Button
            icon={<EditOutlined />}
            onClick={() => onEditAnimal(animal)}
          >
            
          </Button>
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

      <Collapse ghost defaultActiveKey={['1']}>
        <Panel header={<Text strong className="text-gray-500">Detalhes do Paciente</Text>} key="1">
          <Row gutter={[32, 16]}>
            {/* Informações Básicas */}
            <Col xs={24} md={12}>
              <div className="flex flex-col gap-2">
                <Text>
                  <strong className="text-verde-escuro">Espécie:</strong>{" "}
                  <span className="text-gray-900">{animal.especie === "Cao" ? "Cão" : "Gato"}</span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Raça:</strong>{" "}
                  <span className="text-gray-900">{nomeRaca}</span>
                </Text>
                <Text className="flex items-center gap-1">
                  <strong className="text-verde-escuro">Sexo:</strong>{" "}
                  <span className="text-gray-900">{animal.sexo}</span>
                  {animal.sexo === "Macho" ? (
                    <ManOutlined className="text-blue-500" />
                  ) : (
                    <WomanOutlined className="text-pink-500" />
                  )}
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Idade:</strong>{" "}
                  <span className="text-gray-900">{animal.idadeFormatada}</span>
                </Text>
              </div>
            </Col>

            {/* Informações Clínicas Mais Recentes */}
            <Col xs={24} md={12}>
              <div className="flex flex-col gap-2">
                <Text>
                  <strong className="text-verde-escuro">Peso Atual:</strong>{" "}
                  <span className="text-gray-900">
                    {avaliacaoRecente?.peso ? `${avaliacaoRecente.peso} kg` : "Não avaliado"}
                  </span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Escore Corporal (ECC):</strong>{" "}
                  <span className="text-gray-900">
                    {animal.condicaoCorporalAtualLabel || "Não avaliado"}
                  </span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Grau de Atividade:</strong>{" "}
                  <span className="text-gray-900">
                    {avaliacaoRecente?.grauAtividade || "Não avaliado"}
                  </span>
                </Text>
                <Text>
                  <strong className="text-verde-escuro">Fase da Vida:</strong>{" "}
                  <span className="text-gray-900">{animal.faseVidaAtualLabel || "Não definida"}</span>
                </Text>
              </div>
            </Col>
          </Row>
        </Panel>
      </Collapse>
    </Card>
  );
};

export default FichaGeralAnimal;
