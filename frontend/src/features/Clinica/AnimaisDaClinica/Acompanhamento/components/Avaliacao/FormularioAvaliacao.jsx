import React, { useState, useEffect } from "react";
import {
  Form,
  Input,
  InputNumber,
  Slider,
  Switch,
  Select,
  Row,
  Col,
  Divider,
  message,
  Typography,
  Popover,
  Space,
} from "antd";
import { InfoCircleOutlined } from "@ant-design/icons";
import { racoesDaClinicaAPI } from "../../../../../../api/clinica/racoesDaClinicaAPI";
import {
  opcoesCondicaoCorporal,
  opcoesGrauAtividade,
  opcoesTipoDieta,
} from "./opcoesAvaliacao";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const { Text } = Typography;

const FormularioAvaliacao = ({ data, initialValues, onFinish }) => {
  const [form] = Form.useForm();
  const [racoes, setRacoes] = useState([]);
  const [loadingRacoes, setLoadingRacoes] = useState(true);

  console.log('[FormularioAvaliacao] initialValues', initialValues);
  console.log('[FormularioAvaliacao] data', data);

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

  useEffect(() => {
    if (data?.especieAnimal) {
      racoesDaClinicaAPI
        .listar(data.especieAnimal)
        .then((data) =>
          setRacoes(data.map((r) => ({ label: r.nome, value: r.id }))),
        )
        .catch(() => message.error("Erro ao carregar lista de rações."))
        .finally(() => setLoadingRacoes(false));
    }
  }, [data]);

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
        <strong>4 (Magro):</strong> Gordura corporal mínima. Costelas não
        visíveis. Cintura e reentrância abdominal aparentes.
      </p>
      <p>
        <strong>5 (Ideal):</strong> Gordura corporal presente, mas não
        excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância
        abdominal presentes. Bolsa de gordura abdominal mínima.
      </p>
      <p>
        <strong>6 (Sobrepeso - até 15% acima):</strong> Gordura corporal
        moderada. Costelas palpáveis com alguma pressão. Cintura visível, mas
        não acentuada. Reentrância e gordura abdominal aparentes, mas não
        acentuadas.
      </p>
      <p>
        <strong>7 (Sobrepeso - 16% a 30% acima):</strong> Gordura corporal
        importante. Costelas palpáveis com dificuldade. Cintura pouco
        perceptível. Evidente abaulamento abdominal, com bolsa de gordura
        moderada.
      </p>
      <p>
        <strong>8 (Obesidade Grave - 31% a 45% acima):</strong> Gordura
        corporal acentuada. Denso depósito de gordura sobre costelas e região
        lombar. Distensão abdominal proeminente, com significativo depósito de
        gordura.
      </p>
      <p>
        <strong>9 (Obesidade Grave - acima de 46%):</strong> Gordura corporal
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
        visíveis, mas palpáveis. Cintura e reentrância abdominal
        aparentes.
      </p>
      <p>
        <strong>5 (Ideal):</strong> Gordura corporal presente, mas não
        excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância
        abdominal presentes.
      </p>
      <p>
        <strong>6 (Sobrepeso - até 15% acima):</strong> Gordura corporal
        moderada. Costelas palpáveis com alguma pressão. Cintura visível, mas
        não acentuada. Reentrância abdominal aparente.
      </p>
      <p>
        <strong>7 (Sobrepeso - 16% a 30% acima):</strong> Gordura corporal
        importante. Depósitos de gordura evidente sobre costelas, região lombar
        e base da cauda. Costelas palpáveis com dificuldade. Cintura e
        reentrância abdominal pouco perceptíveis ou ausentes.
      </p>
      <p>
        <strong>8 (Obesidade - 31% a 45% acima):</strong> Gordura corporal
        acentuada. Densos depósitos de gordura sobre costelas, região lombar e
        base da cauda. Costelas palpáveis somente com pressão acentuada. Cintura
        e reentrância abdominal ausentes, podendo existir distensão
        abdominal.
      </p>
      <p>
        <strong>9 (Obesidade Grave - acima de 46%):</strong> Gordura corporal
        extremamente acentuada. Depósitos de gordura maciços sobre costelas,
        proeminências ósseas (não palpáveis), pescoço e membros. Cintura e
        reentrância abdominal ausentes. Distensão abdominal acentuada.
      </p>
    </div>
  );

  return (
    <Form
      id="form-avaliacao-consulta-clinica"
      form={form}
      layout="vertical"
      onFinish={onFinish}
    >
      {/* Grid Autoajustável: Evita espaços em branco */}
      <Row gutter={16}>
        <Col xs={24} sm={12}>
          <Form.Item name="peso" label="Peso (kg)" rules={[{ required: true }]}>
            <InputNumber
              placeholder="Ex: 9,87"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>

        {!isGato && (
          <Col xs={24} sm={12}>
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

        <Col xs={24} sm={12}>
          <Form.Item
            name="racaoId"
            label="Ração Utilizada"
            rules={[{ required: true, message: "Selecione a ração" }]}
          >
            <Select
              showSearch
              placeholder="Selecione a ração"
              options={racoes}
              loading={loadingRacoes}
              optionFilterProp="label"
            />
          </Form.Item>
        </Col>

        {/* Se for Gato, expande para 24 colunas para fechar o retângulo do grid com perfeição */}
        <Col xs={24} sm={isGato ? 24 : 12}>
          <Form.Item
            name="tipoDieta"
            label={
              <LabelComInfo
                label="Tipo da Dieta"
                tituloPopover="Diretrizes do Tipo de Dieta"
                conteudoPopover={conteudoTipoDieta}
              />
            }
            rules={[
              { required: true, message: "O tipo da dieta é obrigatório." },
            ]}
          >
            <Select
              options={opcoesTipoDieta}
              placeholder="Selecione o tipo da dieta"
            />
          </Form.Item>
        </Col>
      </Row>

      {isGato && (
          <Form.Item
            name="condicaoCorporal"
            label={
              <LabelComInfo
                label="Condição Corporal (1-9)"
                tituloPopover="Escore de Condição Corporal Felino (Laflamme, 1997)"
                conteudoPopover={conteudoECC}
              />
            }
            rules={[{ required: true, message: "Defina o escore corporal" }]}
          >
            <Slider min={1} max={9} marks={opcoesCondicaoCorporal} />
          </Form.Item>
      )}

      <Form.Item name="descricaoDieta" label="Descrição Adicional da Dieta">
        <Input.TextArea
          rows={3}
          placeholder="Ex: Adicionar 50g de alimento úmido pela manhã ou dividir em 3 porções."
        />
      </Form.Item>

      {isFemea && (
        <>
          <Divider orientation="left" plain>
            Status Reprodutivo
          </Divider>
          <Row gutter={16}>
            <Col xs={24} sm={12}>
              <Form.Item
                name="isGestante"
                label="Em Gestação?"
                valuePropName="checked"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isGestante && (
                <Form.Item
                  name="semanaGestacao"
                  label="Semanas de Gestação"
                  rules={[{ required: true, message: "Obrigatório" }]}
                >
                  <InputNumber
                    style={{ width: "100%" }}
                    min={1}
                    placeholder="Nº de semanas"
                  />
                </Form.Item>
              )}
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item
                name="isLactante"
                label="Em Lactação?"
                valuePropName="checked"
              >
                <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
              </Form.Item>
              {isLactante && (
                <>
                  <Form.Item
                    name="isLactanteNumeroFilhotes"
                    label="Nº de Filhotes"
                    rules={[
                      { required: true, message: "Obrigatório" },
                      {
                        validator: (_, value) =>
                          value > 0
                            ? Promise.resolve()
                            : Promise.reject(new Error("Deve ser maior que 0")),
                      },
                    ]}
                  >
                    <InputNumber
                      style={{ width: "100%" }}
                      min={1}
                      placeholder="Quantos filhotes?"
                    />
                  </Form.Item>
                  <Form.Item name="semanaLactacao" label="Semanas de Lactação">
                    <InputNumber
                      style={{ width: "100%" }}
                      min={1}
                      placeholder="Nº de semanas"
                    />
                  </Form.Item>
                </>
              )}
            </Col>
          </Row>
        </>
      )}
    </Form>
  );
};

export default FormularioAvaliacao;
