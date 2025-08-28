import React from "react";
import { Descriptions, Tag, Typography, Divider, Form } from "antd";
import dayjs from "dayjs";

// --- Funções Auxiliares (Formatters) ---

const formatarData = (data) => {
  if (!data)
    return <Typography.Text type="secondary">Não informado</Typography.Text>;
  return dayjs(data).format("DD/MM/YYYY");
};

const formatarEspecie = (value) => (value === 1 ? "Cão" : "Gato");
const formatarSexo = (value) => (value === 1 ? "Macho" : "Fêmea");

const formatarGrauAtividade = (value) => {
  switch (value) {
    case 1:
      return "Baixo / Inativo";
    case 2:
      return "Moderado / Normal";
    case 3:
      return "Alto / Atleta";
    default:
      return <Typography.Text type="secondary">Não informado</Typography.Text>;
  }
};

const renderSimNao = (value) => (
  <Tag color={value ? "green" : "red"}>{value ? "SIM" : "NÃO"}</Tag>
);

const renderTutores = (tutores = []) => {
  if (!tutores || tutores.length === 0) {
    return (
      <Typography.Text type="secondary">Nenhum tutor associado</Typography.Text>
    );
  }
  return (
    <div style={{ display: "flex", flexWrap: "wrap", gap: "4px" }}>
      {tutores.map((tutor) => (
        <Tag key={tutor.id}>{tutor.nome}</Tag>
      ))}
    </div>
  );
};

// --- Componente Principal de Detalhes ---

const DetalhesAnimaisDaClinica = ({ valores }) => {
  const isFemea = valores.sexo === 2;
  const naoInformado = (
    <Typography.Text type="secondary">Não informado</Typography.Text>
  );

  return (
    <Form>
      <Descriptions title="Tutor(es) Responsável(is)" bordered column={1}>
        {/* Assumindo que `valores.tutores` é um array de objetos tutor */}
        <Descriptions.Item label="Tutores">
          {renderTutores(valores.tutores)}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Identificação do Animal" bordered column={2}>
        <Descriptions.Item label="Nome">{valores.nome}</Descriptions.Item>
        <Descriptions.Item label="Data de Nascimento">
          {formatarData(valores.nascimento)}
        </Descriptions.Item>
        <Descriptions.Item label="Espécie">
          {formatarEspecie(valores.especie)}
        </Descriptions.Item>
        <Descriptions.Item label="Raça">
          {valores.racaNome ?? naoInformado}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Características Físicas" bordered column={2}>
        <Descriptions.Item label="Sexo">
          {formatarSexo(valores.sexo)}
        </Descriptions.Item>
        <Descriptions.Item label="Peso">
          {valores.peso
            ? `${valores.peso.toLocaleString("pt-BR", {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              })} kg`
            : naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Condição Corporal (1-9)">
          {valores.condicaoCorporal ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Grau de Atividade">
          {formatarGrauAtividade(valores.grauAtividade)}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Status Reprodutivo" bordered column={1}>
        <Descriptions.Item label="Castrado">
          {renderSimNao(valores.isCastrado)}
        </Descriptions.Item>
        {isFemea && (
          <>
            <Descriptions.Item label="Em Gestação?">
              {renderSimNao(valores.gestacao)}
            </Descriptions.Item>
            {valores.gestacao && (
              <Descriptions.Item label="Semanas de Gestação">
                {valores.semanaGestacao ?? naoInformado}
              </Descriptions.Item>
            )}
            <Descriptions.Item label="Em Lactação?">
              {renderSimNao(valores.lactacao > 0)}
            </Descriptions.Item>
            {valores.lactacao > 0 && (
              <>
                <Descriptions.Item label="Número de Filhotes">
                  {valores.lactacao}
                </Descriptions.Item>
                <Descriptions.Item label="Semanas de Lactação">
                  {valores.semanaLactacao ?? naoInformado}
                </Descriptions.Item>
              </>
            )}
          </>
        )}
      </Descriptions>
    </Form>
  );
};

export default DetalhesAnimaisDaClinica;
