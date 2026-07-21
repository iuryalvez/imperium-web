import React from "react";
import { Card, Tag, Typography, Row, Col, Statistic } from "antd";

const { Title, Text } = Typography;

const FichaGeralAnimal = ({ animal, dieta }) => {
  if (!animal) return null;

  const tutores =
    animal.tutores?.length > 0
      ? animal.tutores.map((t) => t.nome).join(", ")
      : "Nenhum tutor associado";

  // Fallback to "SRD" if breed is somehow missing
  const nomeRaca = animal.raca?.nome || "SRD";

  return (
    <Card
      className="shadow-sm mb-6 border-cinza-desativado"
      bodyStyle={{ padding: "24px" }}
    >
      <Row gutter={[24, 24]} align="middle">
        {/* Basic Identification */}
        <Col xs={24} md={10} className="border-r border-cinza-desativado">
          <Title level={3} className="text-verde-escuro m-0">
            {animal.nome}
          </Title>
          <Text className="text-verde-medio text-lg font-medium">
            {nomeRaca}
          </Text>

          <div className="mt-4 flex flex-col gap-1">
            <Text className="text-cinza-escuro">
              <strong>Tutor(es):</strong> {tutores}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Espécie:</strong> {animal.especie === "Cao" ? "Cão" : "Gato"} -
              <strong></strong> {animal.sexo}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Idade:</strong> {animal.idade} anos
            </Text>
          </div>
        </Col>

        {/* Clinical Status */}
        <Col
          xs={24}
          md={7}
          className="border-r border-cinza-desativado flex flex-col justify-center"
        >
          <div className="mb-4">
            <Text className="text-cinza-escuro text-xs uppercase tracking-wide">
              Fase da Vida
            </Text>
            <br />
            <Tag className="bg-verde-fraco text-verde-escuro border-verde-claro mt-1">
              {animal.faseVida}
            </Tag>
          </div>
          <div>
            <Text className="text-cinza-escuro text-xs uppercase tracking-wide">
              Escore de Condição Corporal
            </Text>
            <br />
            <Tag color="orange" className="mt-1">
              {animal.condicaoCorporal}
            </Tag>
          </div>
        </Col>

        {/* Dynamic Statistics */}
        <Col xs={24} md={7}>
          <Statistic
            title={<Text className="text-cinza-escuro">Peso Atual</Text>}
            value={animal.pesoAtual || "Não avaliado"}
            suffix={animal.pesoAtual ? "kg" : ""}
            valueStyle={{ color: "#064420" }}
            style={{ marginBottom: 16 }}
          />
          <Statistic
            title={
              <Text className="text-cinza-escuro">
                Kcal Diárias (Último Cálculo)
              </Text>
            }
            value={dieta ? dieta.necessidadeEnergeticaKcal : "N/A"}
            precision={2}
            suffix={dieta ? "kcal" : ""}
            valueStyle={{ color: "#064420" }}
          />
        </Col>
      </Row>
    </Card>
  );
};

export default FichaGeralAnimal;
