import React from "react";
import { Descriptions, Card, Typography, Divider } from "antd";

const { Text } = Typography;

const ModalDetalhesCalculoRacao = ({ racao }) => {
  if (!racao) return null;

  // Normalização para %
  const umidade = racao.isPorcentagem ? racao.umidade : racao.umidade / 10;
  const pb = racao.isPorcentagem ? racao.proteinaBruta : racao.proteinaBruta / 10;
  const eea = racao.isPorcentagem ? racao.extratoEtereo : racao.extratoEtereo / 10;
  const fb = racao.isPorcentagem ? racao.fibraBruta : racao.fibraBruta / 10;
  const mm = racao.isPorcentagem ? racao.materiaMineral : racao.materiaMineral / 10;
  const ms = 100 - umidade;
  const fbMs = ms > 0 ? (fb / ms) * 100 : 0;

  const format2 = (val) => new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 0, maximumFractionDigits: 2 }).format(val);
  const format4 = (val) => new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 4, maximumFractionDigits: 4 }).format(val);

  // Variáveis em g/g
  const pbG = format4(pb / 100);
  const eeaG = format4(eea / 100);
  const fbG = format4(fb / 100);
  const ennG = format4(racao.extrativoNaoNitrogenado / 100);

  // Constantes da espécie
  const isCao = racao.especie === "Cao";
  const constanteCde = isCao ? 91.2 : 87.9;
  const multiplicadorFbCde = isCao ? 1.43 : 0.88;
  const multiplicadorPbEm = isCao ? 1.04 : 0.77;

  return (
    <div>
      <Descriptions size="small" bordered column={2}>
        <Descriptions.Item label="Nome">{racao.nome}</Descriptions.Item>
        <Descriptions.Item label="Espécie">{isCao ? "Cão" : "Gato"}</Descriptions.Item>
        <Descriptions.Item label="Umidade">{format2(umidade)}%</Descriptions.Item>
        <Descriptions.Item label="Proteína Bruta (PB)">{format2(pb)}%</Descriptions.Item>
        <Descriptions.Item label="Extrato Etéreo (EEA)">{format2(eea)}%</Descriptions.Item>
        <Descriptions.Item label="Fibra Bruta (FB)">{format2(fb)}%</Descriptions.Item>
        <Descriptions.Item label="Matéria Mineral (MM)">{format2(mm)}%</Descriptions.Item>
        <Descriptions.Item label="Matéria Seca (MS)">{format2(ms)}%</Descriptions.Item>
      </Descriptions>

      <Divider orientation="left">Passo a Passo (NRC 2006)</Divider>

      <Card size="small" style={{ marginBottom: 12 }}>
        <Text strong>1. Extrativos Não Nitrogenados (ENN)</Text>
        <br />
        <Text type="secondary">Fórmula: 100 - (Umidade + PB + EEA + FB + MM)</Text>
        <br />
        <Text>
          100 - ({format2(umidade)} + {format2(pb)} + {format2(eea)} + {format2(fb)} + {format2(mm)}) = <strong>{format2(racao.extrativoNaoNitrogenado)}%</strong>
        </Text>
      </Card>

      <Card size="small" style={{ marginBottom: 12 }}>
        <Text strong>2. Energia Bruta (EB)</Text>
        <br />
        <Text type="secondary">Fórmula: (5,7 × g PB) + (9,4 × g EEA) + [4,1 × (g ENN + g FB)]</Text>
        <br />
        <Text>
          (5,7 × {pbG}) + (9,4 × {eeaG}) + [4,1 × ({ennG} + {fbG})] = <strong>{format2(racao.energiaBruta)} kcal/g</strong>
        </Text>
      </Card>

      <Card size="small" style={{ marginBottom: 12 }}>
        <Text strong>3. Coeficiente de Digestibilidade da Energia (CDE)</Text>
        <br />
        <Text type="secondary">
          Fórmula ({isCao ? "Cães" : "Gatos"}): {format2(constanteCde)} - ({format2(multiplicadorFbCde)} × porcentagem de FB na MS)
        </Text>
        <br />
        <Text>
          {format2(constanteCde)} - ({format2(multiplicadorFbCde)} × {format2(fbMs)}) = <strong>{format2(racao.coeficienteDigestibilidadeEnergia)}%</strong>
        </Text>
      </Card>

      <Card size="small" style={{ marginBottom: 12 }}>
        <Text strong>4. Energia Digestível (ED)</Text>
        <br />
        <Text type="secondary">Fórmula: EB × (CDE / 100)</Text>
        <br />
        <Text>
          {format2(racao.energiaBruta)} × ({format2(racao.coeficienteDigestibilidadeEnergia)} / 100) = <strong>{format2(racao.energiaDigestivel)} kcal/g</strong>
        </Text>
      </Card>

      <Card size="small">
        <Text strong>5. Energia Metabolizável (EM)</Text>
        <br />
        <Text type="secondary">
          Fórmula ({isCao ? "Cães" : "Gatos"}): ED - ({format2(multiplicadorPbEm)} × g PB)
        </Text>
        <br />
        <Text>
          {format2(racao.energiaDigestivel)} - ({format2(multiplicadorPbEm)} × {pbG}) = <strong>{format2(racao.energiaMetabolizavel)} kcal/g</strong>
        </Text>
      </Card>
    </div>
  );
};

export default ModalDetalhesCalculoRacao;