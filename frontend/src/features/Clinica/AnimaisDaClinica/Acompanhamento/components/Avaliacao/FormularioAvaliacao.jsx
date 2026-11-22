import React, { useEffect } from "react";
import {
  Form,
  Input,
  InputNumber,
  Slider,
  Switch,
  Select,
  Row,
  Col,
  Button,
} from "antd";
import { racoesDaClinicaAPI } from "../../../../../../api/clinica/racoesDaClinicaAPI";
import SelectBuscaRemota from "../../../../../../components/atoms/SelectBuscaRemota";
import {
  opcoesCondicaoCorporal,
  opcoesGrauAtividade,
  opcoesTipoDieta,
} from "./opcoesAvaliacao";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const FormularioAvaliacao = ({ data, initialValues, onFinish }) => {
  const [form] = Form.useForm();

  console.log("[FormularioAvaliacao] initialValues", initialValues);
  console.log("[FormularioAvaliacao] data", data);

  const isGestante = Form.useWatch("isGestante", form);
  const isLactante = Form.useWatch("isLactante", form);

  const isFemea =
    data?.sexoAnimal === 2 ||
    String(data?.sexoAnimal).toLowerCase() === "femea";
  const isGato =
    data?.especieAnimal === 2 ||
    String(data?.especieAnimal).toLowerCase() === "gato";

  useEffect(() => {
    if (isGato) {
      form.setFieldsValue({ grauAtividade: undefined });
    } else {
      form.setFieldsValue({ condicaoCorporal: undefined });
    }
  }, [isGato, form]);

  // Rações do sistema + da clínica (sem as ocultas) da espécie do animal; inativas aparecem desabilitadas
  const buscarRacoes = (busca) => racoesDaClinicaAPI.opcoes(busca, data?.especieAnimal);
  const racaoAtual = initialValues?.racaoId
    ? [{ id: initialValues.racaoId, nome: initialValues.nomeRacao || `Ração #${initialValues.racaoId}` }]
    : [];

  useEffect(() => {
    if (initialValues) {
      const processedValues = { ...initialValues };
      processedValues.isGestante = initialValues.isGestante;
      processedValues.isLactante = initialValues.isLactanteNumeroFilhotes > 0;
      form.setFieldsValue(processedValues);
    } else {
      form.setFieldsValue({
        isGestante: false,
        isLactanteNumeroFilhotes: 0,
      });
    }
  }, [initialValues, form]);

  // Conteúdos literários dos Popovers
  const conteudoAtividade = (
    <ul className="list-disc pl-4 space-y-1">
      <li>
        <strong>Inativo:</strong> Cães mantidos em ambiente doméstico com pouca
        oportunidade ou estímulo à prática de exercício. As necessidades de cães
        idosos podem ser ainda menores.
      </li>
      <li>
        <strong>Moderado:</strong> Cães adultos com atividade normal, passeios
        diários regulares ou cães de canil.
      </li>
      <li>
        <strong>Ativo:</strong> Cães com ampla oportunidade e forte estímulo a
        exercícios, como grupos em ambiente rural, domicílios com amplo quintal
        ou cães de trabalho/esporte.
      </li>
    </ul>
  );

  const conteudoTipoDieta = (
    <ul className="list-disc pl-4 space-y-1">
      <li>
        <strong>Manutenção:</strong> Foco em preservar o peso corporal e a
        composição corporal atual do animal adulto saudável. Abrange ajustes
        leves para emagrecimento ou ganho de peso progressivo sem alterar o
        estágio fisiológico.
      </li>
      <li>
        <strong>Crescimento:</strong> Dieta com densidade energética e proteica
        otimizada para filhotes em desenvolvimento até a maturidade esquelética.
      </li>
      <li>
        <strong>Gestação:</strong> Dieta formulada para suprir a alta demanda
        energética no terço final da gestação de fêmeas reprodutoras.
      </li>
      <li>
        <strong>Lactação:</strong> Fase de maior exigência metabólica; visa
        garantir a produção láctea sem esgotar as reservas corporais da mãe.
      </li>
      <li>
        <strong>Emagrecimento:</strong> Dieta formulada para ajudar o animal a
        perder peso de forma saudável e sustentável (1% a 2% semanalmente).
      </li>
    </ul>
  );

  const conteudoECC = isGato ? (
    <div className="space-y-2">
      <p>
        <strong>Classificação Felina:</strong>
      </p>
      <p>
        <strong>1 (Emaciado):</strong> Ausência de gordura corporal perceptível.
        Costelas e proeminências ósseas facilmente visíveis e palpáveis. Cintura
        ou Reentrância abdominal acentuada.
      </p>
      <p>
        <strong>2 (Muito Magro):</strong> Ausência de gordura corporal palpável.
        Costelas e proeminências ósseas podem estar visíveis, facilmente
        palpáveis. Reentrância abdominal marcada.
      </p>
      <p>
        <strong>3 (Magro):</strong> Gordura corporal mínima. Costelas podem
        estar visíveis, facilmente palpáveis. Cintura ou reentrância abdominal
        evidentes.
      </p>
      <p>
        <strong>4 (Magro):</strong> Costelas palpáveis, com cobertura mínima
        de gordura. Cintura perceptível atrás das costelas. Leve reentrância
        abdominal. Bolsa de gordura abdominal mínima.
      </p>
      <p>
        <strong>5 (Ideal):</strong> Gordura corporal presente, mas não
        excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância
        abdominal presentes. Bolsa de gordura abdominal mínima.
      </p>
      <p>
        <strong>6 (Sobrepeso):</strong> Costelas palpáveis, com leve excesso
        de gordura. Cintura e bolsa de gordura abdominal perceptíveis, mas não
        evidentes. Reentrância abdominal ausente.
      </p>
      <p>
        <strong>7 (Sobrepeso - cerca de 20% acima do ideal):</strong> Gordura corporal
        importante. Costelas palpáveis com dificuldade. Cintura pouco
        perceptível. Evidente abaulamento abdominal, com bolsa de gordura
        moderada.
      </p>
      <p>
        <strong>8 (Obesidade Grave - cerca de 30% acima do ideal):</strong> Gordura corporal
        acentuada. Denso depósito de gordura sobre costelas e região lombar.
        Distensão abdominal proeminente, com significativo depósito de gordura.
      </p>
      <p>
        <strong>9 (Obesidade Grave - 40% ou mais acima do ideal):</strong> Gordura corporal
        extremamente acentuada. Depósitos de gordura maciços sobre costelas,
        proeminências ósseas, face e membros. Cintura ausente. Distensão
        abdominal acentuada, com depósitos de gordura abdominal maciços.
      </p>
    </div>
  ) : (
    <div className="space-y-2">
      <p>
        <strong>Classificação Canina:</strong>
      </p>
      <p>
        <strong>1 (Emaciado):</strong> Ausência de gordura corporal perceptível.
        Costelas e proeminências ósseas facilmente visíveis e palpáveis. Cintura
        ou reentrância abdominal acentuadas.
      </p>
      <p>
        <strong>2 (Muito Magro):</strong> Ausência de gordura corporal palpável.
        Costelas e proeminências ósseas visíveis e facilmente palpáveis. Cintura
        ou reentrância abdominal acentuadas.
      </p>
      <p>
        <strong>3 (Magro):</strong> Ausência de gordura corporal palpável.
        Costelas e proeminências ósseas podem estar visíveis, facilmente
        palpáveis. Cintura ou reentrância abdominal acentuadas.
      </p>
      <p>
        <strong>4 (Ideal):</strong> Gordura corporal mínima. Costelas não
        visíveis, mas palpáveis. Cintura e reentrância abdominal aparentes.
      </p>
      <p>
        <strong>5 (Ideal):</strong> Gordura corporal presente, mas não
        excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância
        abdominal presentes.
      </p>
      <p>
        <strong>6 (Sobrepeso):</strong> Gordura corporal
        moderada. Costelas palpáveis com alguma pressão. Cintura visível, mas
        não acentuada. Reentrância abdominal aparente.
      </p>
      <p>
        <strong>7 (Sobrepeso - cerca de 20% acima do ideal):</strong> Gordura corporal
        importante. Depósitos de gordura evidente sobre costelas, região lombar
        e base da cauda. Costelas palpáveis com dificuldade. Cintura e
        reentrância abdominal pouco perceptíveis ou ausentes.
      </p>
      <p>
        <strong>8 (Obesidade - cerca de 30% acima do ideal):</strong> Gordura corporal
        acentuada. Densos depósitos de gordura sobre costelas, região lombar e
        base da cauda. Costelas palpáveis somente com pressão acentuada. Cintura
        e reentrância abdominal ausentes, podendo existir distensão abdominal.
      </p>
      <p>
        <strong>9 (Obesidade Grave - 40% ou mais acima do ideal):</strong> Gordura corporal
        extremamente acentuada. Depósitos de gordura maciços sobre costelas,
        proeminências ósseas (não palpáveis), pescoço e membros. Cintura e
        reentrância abdominal ausentes. Distensão abdominal acentuada.
      </p>
    </div>
  );

  const getFilteredOpcoesTipoDieta = () => {
    let filtered = [...opcoesTipoDieta];
    if (data) {
      if (!isFemea) {
        filtered = filtered.filter((o) => o.value !== 3 && o.value !== 4); // Remove Lactação e Gestação
      }

      const faseVida = String(data.faseVidaAtual || "").toLowerCase();
      if (
        faseVida.includes("adulto") ||
        faseVida.includes("senior") ||
        faseVida.includes("sênior") ||
        faseVida.includes("geriatrico") ||
        faseVida.includes("geriátrico")
      ) {
        filtered = filtered.filter((o) => o.value !== 2); // Remove Crescimento
      }

      if (faseVida.includes("filhote")) {
        filtered = filtered.filter((o) => o.value !== 1 && o.value !== 5); // Remove Manutenção e Emagrecimento
      }
    }
    return filtered;
  };

  const opcoesTipoDietaFiltradas = getFilteredOpcoesTipoDieta();

  return (
    <Form
      id="form-avaliacao-consulta-clinica"
      form={form}
      layout="vertical"
      onFinish={onFinish}
    >
      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item name="peso" label="Peso (kg)" rules={[{ required: true }]}>
            <InputNumber
              placeholder="Ex: 9,87"
              style={{ width: "100%" }}
              min={0.01}
              max={300}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>

        {!isGato && (
          <Col xs={24} md={12}>
            <Form.Item
              name="grauAtividade"
              label={
                <LabelComInfo
                  label="Grau de Atividade"
                  tituloPopover="Referência de Atividade Canina (NRC, 2006)"
                  conteudoPopover={conteudoAtividade}
                />
              }
              rules={[{ required: true, message: "Selecione a atividade" }]}
            >
              <Select
                options={opcoesGrauAtividade}
                placeholder="Selecione o grau de atividade"
              />
            </Form.Item>
          </Col>
        )}
      </Row>

      {isFemea && (
        <div className="bg-pink-50 p-3 rounded-md border border-pink-100 mb-4 mt-2">
          <h4 className="text-sm font-semibold text-pink-700 mb-3">
            Fase Reprodutiva
          </h4>
          <Row gutter={16}>
            <Col xs={24} md={12}>
              <Form.Item
                name="isGestante"
                label="Em Gestação?"
                valuePropName="checked"
                className="mb-2"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isGestante && (
                <Form.Item
                  name="semanaGestacao"
                  label="Semanas de Gestação"
                  rules={[{ required: true, message: "Obrigatório" }]}
                  className="mb-0"
                >
                  <InputNumber
                    style={{ width: "100%" }}
                    min={1}
                    max={9}
                    placeholder="Ex: 5"
                  />
                </Form.Item>
              )}
            </Col>

            <Col xs={24} md={12}>
              <Form.Item
                name="isLactante"
                label="Em Lactação?"
                valuePropName="checked"
                className="mb-2"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isLactante && (
                <Row gutter={8}>
                  <Col span={12}>
                    <Form.Item
                      name="isLactanteNumeroFilhotes"
                      label="Nº Filhotes"
                      rules={[
                        { required: true, message: "Obrigatório" },
                        {
                          validator: (_, value) =>
                            value > 0
                              ? Promise.resolve()
                              : Promise.reject(new Error("Maior que 0")),
                        },
                      ]}
                      className="mb-0"
                    >
                      <InputNumber
                        style={{ width: "100%" }}
                        min={1}
                        max={30}
                        placeholder="Ex: 4"
                      />
                    </Form.Item>
                  </Col>
                  <Col span={12}>
                    <Form.Item 
                      name="semanaLactacao" 
                      label="Semanas"
                      rules={[{ required: true, message: "Obrigatório" }]}
                      className="mb-0"
                    >
                      <InputNumber
                        style={{ width: "100%" }}
                        min={1}
                        max={20}
                        placeholder="Ex: 3"
                      />
                    </Form.Item>
                  </Col>
                </Row>
              )}
            </Col>
          </Row>
        </div>
      )}

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            name="tipoDieta"
            label={
              <LabelComInfo
                label="Tipo da Dieta Atual"
                tituloPopover="Diretrizes do Tipo de Dieta"
                conteudoPopover={conteudoTipoDieta}
              />
            }
            rules={[{ required: true, message: "O tipo da dieta é obrigatório." }]}
          >
            <Select
              options={opcoesTipoDietaFiltradas}
              placeholder="Selecione o tipo da dieta"
            />
          </Form.Item>
        </Col>

        <Col xs={24} md={12}>
          <Form.Item
            name="racaoId"
            label="Ração Utilizada"
            rules={[{ required: true, message: "Selecione a ração" }]}
          >
            <SelectBuscaRemota
              placeholder="Selecione a ração"
              buscarOpcoes={buscarRacoes}
              opcoesAtuais={racaoAtual}
              dependencias={[data?.especieAnimal]}
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4 mt-2">
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="condicaoCorporal"
              label={
                <div className="flex items-center justify-between w-full">
                  <LabelComInfo
                    label="Condição Corporal (ECC - 1 a 9)"
                    tituloPopover="Escore de Condição Corporal"
                    conteudoPopover={conteudoECC}
                  />
                  {!isGato && (
                    <Button 
                      type="link" 
                      size="small" 
                      className="text-xs p-0 m-0"
                      onClick={() => form.setFieldsValue({ condicaoCorporal: undefined })}
                    >
                      Limpar
                    </Button>
                  )}
                </div>
              }
              rules={[{ required: isGato, message: "Defina o escore corporal" }]}
              className="mb-0 px-2"
            >
              <Slider min={1} max={9} marks={opcoesCondicaoCorporal} />
            </Form.Item>
          </Col>
        </Row>
      </div>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item name="descricaoDieta" label="Descrição Adicional da Dieta (Opcional)">
            <Input.TextArea
              rows={3}
              placeholder="Ex: Adicionar 50g de alimento úmido pela manhã ou dividir em 3 porções."
            />
          </Form.Item>
        </Col>
      </Row>
    </Form>
  );
};

export default FormularioAvaliacao;
