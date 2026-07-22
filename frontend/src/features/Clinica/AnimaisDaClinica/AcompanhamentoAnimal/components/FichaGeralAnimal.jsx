import React from "react";
import { Card, Typography, Row, Col, Divider } from "antd";
import { ManOutlined, WomanOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

// Ícones PNG transformados em máscara CSS para herdar as cores do Tailwind
import PetIconSrc from "../../../../../assets/animal/pet-14-svgrepo-com.png";
import WeightIconSrc from "../../../../../assets/animal/pet-weight-weigh-a-pet-small-pet-svgrepo-com.png";
import FoodIconSrc from "../../../../../assets/animal/dog-food-pet-svgrepo-com.png";

const { Title, Text } = Typography;

const CustomIconMask = ({ src, className = "w-4 h-4" }) => (
  <div
    className={`${className} bg-current inline-block`}
    style={{
      WebkitMask: `url(${src}) center/contain no-repeat`,
      mask: `url(${src}) center/contain no-repeat`,
    }}
  />
);

const FichaGeralAnimal = ({ animal, relatorioRecente, dietaRecente }) => {
  if (!animal) return null;

  const nomeRaca = animal.raca?.nome || "SRD";
  const dataRelatorio = relatorioRecente?.dataConsulta
    ? dayjs(relatorioRecente.dataConsulta).format("DD/MM/YYYY")
    : "Sem registros";

  // Alguns dados da dieta podem vir de relatórios anteriores, então pegamos a data vinculada à dieta, se existir.
  const dataDieta = dietaRecente?.dataCalculo // Supondo que tenha data, senão usa a do relatório
    ? dayjs(dietaRecente.dataCalculo).format("DD/MM/YYYY")
    : dataRelatorio;

  return (
    <Card
      className="shadow-sm mb-6 border-cinza-desativado"
      bodyStyle={{ padding: "24px" }}
    >
      <Title
        level={3}
        className="text-verde-escuro mt-0 mb-6 border-b pb-2 border-gray-100"
      >
        Ficha Geral - {animal.nome}
      </Title>

      <Row gutter={[32, 24]}>
        {/* Coluna 1: Dados Estáticos do Animal */}
        <Col xs={24} md={8} className="md:border-r border-gray-100">
          <div className="flex items-center gap-2 mb-3 text-verde-medio">
            <CustomIconMask src={PetIconSrc} className="w-5 h-5" />
            <Text className="text-verde-escuro font-semibold text-lg uppercase tracking-wide">
              Características
            </Text>
          </div>
          <div className="flex flex-col gap-2">
            <Text className="text-cinza-escuro">
              <strong>Espécie:</strong>{" "}
              {animal.especie === "Cao" ? "Cão" : "Gato"}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Raça:</strong> {nomeRaca}
            </Text>
            <Text className="text-cinza-escuro flex items-center gap-1">
              <strong>Sexo:</strong> {animal.sexo}
              {animal.sexo === "Macho" ? (
                <ManOutlined className="text-blue-500" />
              ) : (
                <WomanOutlined className="text-pink-500" />
              )}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Idade:</strong> {animal.idade} anos
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Fase da Vida:</strong> {animal.faseVida || "Não definida"}
            </Text>
          </div>
        </Col>

        {/* Coluna 2: Dados Clínicos (Variáveis do Relatório) */}
        <Col xs={24} md={8} className="md:border-r border-gray-100">
          <div className="flex items-center gap-2 mb-3 text-verde-medio">
            <CustomIconMask src={WeightIconSrc} className="w-5 h-5" />
            <Text className="text-verde-escuro font-semibold text-lg uppercase tracking-wide">
              Avaliação Física
            </Text>
          </div>
          <Text className="block text-xs text-gray-400 mb-3">
            Última avaliação: {dataRelatorio}
          </Text>

          <div className="flex flex-col gap-2">
            <Text className="text-cinza-escuro">
              <strong>Peso Atual:</strong>{" "}
              {relatorioRecente?.peso
                ? `${relatorioRecente.peso} kg`
                : "Não avaliado"}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Escore Corporal (ECC):</strong>{" "}
              {relatorioRecente?.condicaoCorporal || "Não avaliado"}
            </Text>
            <Text className="text-cinza-escuro">
              <strong>Grau de Atividade:</strong>{" "}
              {relatorioRecente?.grauAtividade || "Não avaliado"}
            </Text>
          </div>
        </Col>

        {/* Coluna 3: Dados Nutricionais (Variáveis da Dieta) */}
        <Col xs={24} md={8}>
          <div className="flex items-center gap-2 mb-3 text-verde-medio">
            <CustomIconMask src={FoodIconSrc} className="w-5 h-5" />
            <Text className="text-verde-escuro font-semibold text-lg uppercase tracking-wide">
              Plano Nutricional
            </Text>
          </div>
          {dietaRecente ? (
            <>
              <Text className="block text-xs text-gray-400 mb-3">
                Cálculo realizado em: {dataDieta}
              </Text>
              <div className="flex flex-col gap-2">
                <Text className="text-cinza-escuro">
                  <strong>Ração Base:</strong>{" "}
                  {dietaRecente.racaoUtilizada || "Não informada"}
                </Text>
                <Text className="text-cinza-escuro">
                  <strong>Tipo de Dieta:</strong>{" "}
                  {dietaRecente.tipoDieta || "Não informado"}
                </Text>
                <Text className="text-cinza-escuro">
                  <strong>Necessidade (NEM):</strong>{" "}
                  {dietaRecente.necessidadeEnergeticaKcal
                    ? `${dietaRecente.necessidadeEnergeticaKcal.toFixed(2)} kcal/dia`
                    : "N/A"}
                </Text>
                <Text className="text-cinza-escuro">
                  <strong>Quantidade Diária:</strong>{" "}
                  {dietaRecente.quantidadeAlimentoGramas
                    ? `${dietaRecente.quantidadeAlimentoGramas.toFixed(2)} g`
                    : "N/A"}
                </Text>
              </div>
            </>
          ) : (
            <div className="h-full flex flex-col justify-center text-gray-400 text-sm mt-4">
              Nenhuma dieta foi calculada ou vinculada recentemente para este
              animal.
            </div>
          )}
        </Col>
      </Row>
    </Card>
  );
};

export default FichaGeralAnimal;
