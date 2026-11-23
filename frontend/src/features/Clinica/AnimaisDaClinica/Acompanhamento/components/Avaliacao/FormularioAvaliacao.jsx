import React, { useEffect } from "react";
import {
  Form,
  Input,
  InputNumber,
  Switch,
  Select,
  Row,
  Col,
} from "antd";
import { racoesDaClinicaAPI } from "../../../../../../api/clinica/racoesDaClinicaAPI";
import SelectBuscaRemota from "../../../../../../components/atoms/SelectBuscaRemota";
import { opcoesGrauAtividade, opcoesTipoDieta } from "./opcoesAvaliacao";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";
import SeletorCondicaoCorporal, {
  RotuloCondicaoCorporal,
} from "./SeletorCondicaoCorporal/SeletorCondicaoCorporal";

const FormularioAvaliacao = ({ data, initialValues, onFinish }) => {
  const [form] = Form.useForm();

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
        <Form.Item
          name="condicaoCorporal"
          label={
            <RotuloCondicaoCorporal
              especie={isGato ? "gato" : "cao"}
              onLimpar={
                isGato ? undefined : () => form.setFieldsValue({ condicaoCorporal: undefined })
              }
            />
          }
          rules={[{ required: isGato, message: "Defina o escore corporal" }]}
          className="mb-0"
        >
          <SeletorCondicaoCorporal especie={isGato ? "gato" : "cao"} />
        </Form.Item>
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
