import React from "react";
import { Descriptions, Tag, Typography, Divider } from "antd";
import { formatarData, formatarEspecie, formatarSexo, formatarGrauAtividade } from "../../../../utils/formatters"; // Ajuste o caminho

const { Text } = Typography;

// Helper para renderizar "Sim" ou "Não"
const renderSimNao = (value) => (
    <Tag color={value ? "green" : "red"}>{value ? "SIM" : "NÃO"}</Tag>
);

const renderTutores = (tutores = []) => {
    if (!tutores || tutores.length === 0) return <Text type="secondary">Nenhum tutor associado</Text>;
    return (
        <div style={{ display: "flex", flexWrap: "wrap", gap: "4px" }}>
            {tutores.map((tutor) => <Tag key={tutor.id}>{tutor.nome}</Tag>)}
        </div>
    );
};

const FormDetalhesAnimal = ({ valores: animal }) => {
  const isFemea = animal.sexo === 2;
  const naoInformado = <Text type="secondary">Não informado</Text>;

  return (
    <div>
      <Descriptions title="Identificação do Animal" bordered column={2}>
        <Descriptions.Item label="Nome">{animal.nome}</Descriptions.Item>
        <Descriptions.Item label="Data de Nascimento">{formatarData(animal.nascimento)}</Descriptions.Item>
        <Descriptions.Item label="Espécie">{formatarEspecie(animal.especie)}</Descriptions.Item>
        <Descriptions.Item label="Raça">{animal.racaNome || naoInformado}</Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Tutor(es) Responsável(is)" bordered column={1}>
        <Descriptions.Item label="Tutores">{renderTutores(animal.tutores)}</Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Características Físicas" bordered column={2}>
        <Descriptions.Item label="Sexo">{formatarSexo(animal.sexo)}</Descriptions.Item>
        <Descriptions.Item label="Peso">
          {animal.peso ? `${animal.peso.toLocaleString("pt-BR")} kg` : naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Condição Corporal (1-9)">{animal.condicaoCorporal || naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Grau de Atividade">{formatarGrauAtividade(animal.grauAtividade)}</Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Status Reprodutivo" bordered column={1}>
        <Descriptions.Item label="Castrado">{renderSimNao(animal.isCastrado)}</Descriptions.Item>
        {isFemea && (
          <>
            <Descriptions.Item label="Em Gestação?">{renderSimNao(animal.gestacao)}</Descriptions.Item>
            {animal.gestacao && (
              <Descriptions.Item label="Semanas de Gestação">{animal.semanaGestacao || naoInformado}</Descriptions.Item>
            )}
            <Descriptions.Item label="Em Lactação?">{renderSimNao(animal.lactacao > 0)}</Descriptions.Item>
            {animal.lactacao > 0 && (
              <>
                <Descriptions.Item label="Número de Filhotes">{animal.lactacao}</Descriptions.Item>
                <Descriptions.Item label="Semanas de Lactação">{animal.semanaLactacao || naoInformado}</Descriptions.Item>
              </>
            )}
          </>
        )}
      </Descriptions>
    </div>
  );
};

export default FormDetalhesAnimal;