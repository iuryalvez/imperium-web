import React, { useState } from "react";
import {
  Statistic,
  Row,
  Col,
  Descriptions,
  Typography,
  Divider,
  Popover,
  Tag,
  Button,
  Form,
  InputNumber,
  Input,
  Card,
  Popconfirm,
  Tooltip,
  message,
  Progress,
  Alert
} from "antd";
import {
  ExperimentOutlined,
  CalculatorOutlined,
  InfoCircleOutlined,
  PlusOutlined,
  DeleteOutlined,
  LineChartOutlined
} from "@ant-design/icons";
import ModalPadrao from "../../../../../../components/modal/ModalPadrao";
import { colors } from "../../../../../../theme/colors";
import FormulaPreview from "./components/FormulaPreview";
import GraficoPesoSemana from "./components/GraficoPesoSemana";

const { Text } = Typography;

const conteudoTodasFormulas = (
  <div className="max-w-md max-h-96 overflow-y-auto space-y-3 text-xs leading-relaxed text-gray-700 p-1">
    <div>
      <p className="font-bold text-[var(--verde-escuro)] mb-1">
        CÃES (NRC, 2006)
      </p>
      <ul className="list-disc pl-4 space-y-1">
        <li>
          <strong>Inativos:</strong> 95 × (PC em kg)<sup>0,75</sup>
        </li>
        <li>
          <strong>Ativos ou de canil (Média):</strong> 130 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Adultos jovens e ativos:</strong> 140 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Terriers adultos e ativos:</strong> 180 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Dogue Alemão adulto e ativo:</strong> 200 × (PC em kg)
          <sup>0,75</sup>
        </li>
        <li>
          <strong>Idosos ativos / Newfoundlands:</strong> 105 × (PC em kg)
          <sup>0,75</sup>
        </li>
      </ul>
    </div>

    <div className="border-t pt-2">
      <p className="font-bold text-[var(--verde-escuro)] mb-1">
        GATOS (NRC, 2006)
      </p>
      <ul className="list-disc pl-4 space-y-1">
        <li>
          <strong>Domésticos magros ou ideais (ECC ≤ 5):</strong> 100 × (PC em
          kg)<sup>0,67</sup>
        </li>
        <li>
          <strong>Domésticos obesos (ECC &gt; 5):</strong> 130 × (PC em kg)
          <sup>0,40</sup>
        </li>
        <li>
          <strong>Gatos exóticos (Média):</strong> 157,5 × (PC em kg)
          <sup>0,75</sup>
        </li>
      </ul>
    </div>
  </div>
);

const DetalhesDieta = ({ dieta, isGato, onRegistrarPeso, onRemoverPeso }) => {
  const [isPesoModalVisible, setIsPesoModalVisible] = useState(false);
  const [salvandoPeso, setSalvandoPeso] = useState(false);
  const [formPeso] = Form.useForm();

  if (!dieta) return null;

  const kcal =
    dieta.necessidadeEnergeticaKcal ?? dieta.energiaMetabolizavel ?? 0;
  const gramas = dieta.quantidadeAlimentoGramas ?? dieta.qtdAlimento ?? 0;
  const racao = dieta.racaoUtilizada ?? dieta.nomeRacao ?? "Não informada";

  const formatadorDuasCasas = new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  const formatadorUmaCasa = new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 1, maximumFractionDigits: 1 });

  // const getStatusColor = (status) => {
  //   switch (status) {
  //     case 1: return "default"; // Aberta
  //     case 2: return "blue"; // EmAndamento
  //     case 3: return "green"; // Concluida
  //     default: return "default";
  //   }
  // };

  const handleOpenPesoModal = () => setIsPesoModalVisible(true);
  const handleClosePesoModal = () => {
    setIsPesoModalVisible(false);
    formPeso.resetFields();
  };

  const previsoes = dieta.previsoesJson ? JSON.parse(dieta.previsoesJson) : [];
  const isEmagrecimento = dieta.tipoDieta === 5;
  // Semana atual: no emagrecimento avança conforme os pesos são registrados
  // (próxima semana a registrar = maior semana registrada + 1), nunca menor que a calculada por data.
  const maiorSemanaRegistrada = Math.max(0, ...(dieta.registrosPeso || []).map(r => r.semana));
  const semanaPorData = dieta.semanaAtual || 1;
  const semanaAtual = isEmagrecimento
    ? Math.min(Math.max(semanaPorData, maiorSemanaRegistrada + 1), 15)
    : semanaPorData;

  // O peso alvo do emagrecimento é salvo em pesoObjetivo (pesoIdeal pode vir nulo)
  const pesoAlvo = dieta.pesoObjetivo ?? dieta.pesoIdeal ?? 0;
  // % por semana acima do qual a perda é considerada prejudicial: 2% em cães (Guimarães e Tudury, 2006)
  // e 1% em gatos (Carciofi, 2013; Gomes, 2016)
  const LIMITE_PERDA_SEVERA = isGato ? 1 : 2;

  // Semanas já registradas e primeira semana livre (sugerida no formulário)
  const semanasRegistradas = (dieta.registrosPeso || []).map(r => r.semana);
  const todasSemanasRegistradas = semanasRegistradas.length >= 15;
  const semanaInicialBusca = Math.min(Math.max(semanaAtual, 1), 15);
  const semanaSugerida = [
    ...Array.from({ length: 15 - semanaInicialBusca + 1 }, (_, i) => semanaInicialBusca + i),
    ...Array.from({ length: semanaInicialBusca - 1 }, (_, i) => i + 1),
  ].find(s => !semanasRegistradas.includes(s)) ?? semanaInicialBusca;

  // Meta de perda semanal (%) definida pelo nível de restrição (backend já envia em %, ex.: 1.5)
  const percentualEsperado = typeof dieta.percentualPerdaSemanal === "number" ? dieta.percentualPerdaSemanal : null;

  /**
   * Perda real por semana de um registro: compara com o último peso registrado antes dele
   * (ou com o peso inicial da dieta) e divide pelo nº de semanas decorridas entre os dois.
   */
  const calcularVariacao = (registro) => {
    const anteriores = (dieta.registrosPeso || [])
      .filter(r => r.semana < registro.semana)
      .sort((a, b) => b.semana - a.semana);
    const base = anteriores[0];
    const pesoBase = base ? base.pesoReal : dieta.pesoAtual;
    const semanaBase = base ? base.semana : 0;
    if (!pesoBase) return null;

    const semanas = Math.max(registro.semana - semanaBase, 1);
    const perdaKg = (pesoBase - registro.pesoReal) / semanas;
    const referencia = base
      ? `o peso da Semana ${base.semana} (${formatadorDuasCasas.format(base.pesoReal)} kg)`
      : `o peso inicial da dieta (${formatadorDuasCasas.format(dieta.pesoAtual)} kg)`;
    return { kg: perdaKg, percentual: (perdaKg / pesoBase) * 100, referencia };
  };

  // Cálculo do progresso total até o peso alvo e previsão de conclusão com base nos registros reais
  const registrosOrdenados = [...(dieta.registrosPeso || [])].sort((a, b) => a.semana - b.semana);
  const ultimoRegistro = registrosOrdenados[registrosOrdenados.length - 1];
  const pesoMaisRecente = ultimoRegistro ? ultimoRegistro.pesoReal : dieta.pesoAtual;
  const totalParaPerder = (dieta.pesoAtual || 0) - pesoAlvo;
  const totalPerdido = (dieta.pesoAtual || 0) - (pesoMaisRecente || 0);
  const percentualProgresso = totalParaPerder > 0
    ? Math.min(Math.max(Math.round((totalPerdido / totalParaPerder) * 100), 0), 100)
    : 0;

  let estimativaAlvoTexto = null;
  if (ultimoRegistro && pesoAlvo > 0) {
    if (pesoMaisRecente <= pesoAlvo) {
      estimativaAlvoTexto = "Peso alvo atingido!";
    } else if (totalPerdido > 0 && ultimoRegistro.semana > 0) {
      const mediaPerdaPorSemana = totalPerdido / ultimoRegistro.semana;
      const semanasRestantes = Math.ceil((pesoMaisRecente - pesoAlvo) / mediaPerdaPorSemana);
      if (dieta.dataInicio) {
        const dataRef = new Date(`${ultimoRegistro.dataRegistro || dieta.dataInicio}T00:00:00`);
        dataRef.setDate(dataRef.getDate() + semanasRestantes * 7);
        estimativaAlvoTexto = `Previsão no ritmo atual (≈ ${formatadorDuasCasas.format(mediaPerdaPorSemana)} kg/sem): mais ~${semanasRestantes} semana(s), por volta de ${dataRef.toLocaleDateString("pt-BR")}.`;
      } else {
        estimativaAlvoTexto = `Previsão no ritmo atual (≈ ${formatadorDuasCasas.format(mediaPerdaPorSemana)} kg/sem): mais ~${semanasRestantes} semana(s) para atingir o alvo.`;
      }
    } else {
      estimativaAlvoTexto = "Sem perda acumulada até o momento para estimar a conclusão.";
    }
  }

  const thClass = "px-2 py-1 text-left text-xs font-medium text-gray-500 uppercase tracking-wider";
  const tdClass = "px-2 py-1 whitespace-nowrap text-sm text-gray-700";

  const submitPeso = async (values) => {
    if (semanasRegistradas.includes(values.semana)) {
      message.warning(`Já existe um registro para a semana ${values.semana}.`);
      return;
    }
    if (onRegistrarPeso) {
      const dto = {
        semana: values.semana,
        dataRegistro: new Date().toISOString().split('T')[0],
        pesoReal: values.pesoReal,
        observacao: values.observacao
      };
      setSalvandoPeso(true);
      try {
        const ok = await onRegistrarPeso(dieta.id, dto);
        if (ok) handleClosePesoModal();
      } finally {
        setSalvandoPeso(false);
      }
    }
  };

  return (
    <div>

      {/* 1. Resultados Principais em Destaque */}
      <Row gutter={[16, 16]}>
        <Col xs={24} md={12}>
          <div className="flex items-center gap-1.5 mb-1">
            <span className="text-gray-500 text-xs uppercase tracking-wider font-semibold">
              Necessidade Energética
            </span>
            <Popover
              title={
                <Text className="font-bold text-[var(--verde-escuro)]">
                  Fórmulas de Necessidade Energética (NRC)
                </Text>
              }
              content={conteudoTodasFormulas}
              trigger="click"
              placement="right"
            >
              <InfoCircleOutlined className="text-azul-info cursor-pointer hover:text-azul-hover transition-colors text-sm" />
            </Popover>
          </div>
          <Statistic
            value={kcal}
            formatter={(val) => formatadorDuasCasas.format(val)}
            suffix="kcal/dia"
            valueStyle={{ color: "var(--verde-escuro)", fontWeight: 600 }}
          />
        </Col>
        <Col xs={24} md={12}>
          <Statistic
            title="Quantidade de Alimento"
            value={gramas}
            formatter={(val) => formatadorDuasCasas.format(val)}
            prefix={
              <ExperimentOutlined className="text-[var(--verde-medio)] mr-1" />
            }
            suffix="g/dia"
            valueStyle={{ color: "var(--verde-escuro)", fontWeight: 600 }}
          />
        </Col>
      </Row>

      {/* 2. Transparência do Cálculo (Explainable Algorithm) */}
      {dieta.perfilNutricional && (
        <div className="my-4 p-3 bg-gray-50 border border-gray-200 rounded-md">
          <div className="flex items-center gap-1 text-[var(--verde-escuro)] font-semibold mb-1 text-xs uppercase tracking-wider">
            <CalculatorOutlined />
            <span>Perfil Metabólico</span>
          </div>

          <div className="flex items-center gap-2 mb-3">
            <Text className="block text-sm text-gray-700 font-medium">
              {dieta.perfilNutricional || "Cálculo padrão baseado na literatura"}
            </Text>
          </div>

          <div className="pt-2 border-t border-gray-200 mt-2">
            <FormulaPreview
              tipoDieta={dieta.tipoDieta}
              isGato={isGato}
              isCustomizado={true} // Força true para usar os valores exatos calculados no backend
              multiplicadorCustom={dieta.fatorK}
              potencia={dieta.expoente}
              pesoAtual={dieta.pesoAtual}
              pesoObjetivo={dieta.pesoObjetivo}
              perfilMetabolicoLabel={dieta.perfilNutricional}
              hasAdicionalGestacaoCao={dieta.hasAdicionalGestacaoCao}
              numeroFilhotes={dieta.numeroFilhotes}
              semanaLactacao={dieta.semanaLactacao}
              fatorRestricao={dieta.fatorRestricao}
            />
          </div>
        </div>
      )}

      {/* Emagrecimento: Resumo do Plano */}
      {isEmagrecimento && (
        <Card size="small" className="mb-3 bg-orange-50 border-orange-200" styles={{ body: { padding: 10 } }}>
          <div className="flex flex-col gap-0.5">
            <span className="font-semibold text-orange-800">Plano de Emagrecimento (Duração: 15 semanas)</span>
            <span className="text-sm text-orange-700">Restrição: <strong>{dieta.nivelRestricaoLabel} ({(dieta.fatorRestricao * 100).toFixed(0)}% da EM Base)</strong></span>
            <span className="text-sm text-orange-700">
              Perda estimada: <strong>{formatadorUmaCasa.format(dieta.percentualPerdaSemanal)}% / semana</strong> (≈ {formatadorDuasCasas.format(dieta.perdaSemanalKg)} kg)
            </span>
            <span className="text-sm text-orange-700">Peso Atual: {formatadorDuasCasas.format(dieta.pesoAtual)} kg | Peso Alvo: {formatadorDuasCasas.format(pesoAlvo)} kg</span>
          </div>
        </Card>
      )}

      {/* Previsões Json e Acompanhamento de Peso */}
      {previsoes.length > 0 && (
        <div className="mt-4 mb-2">
          <Divider orientation="left" plain className="mb-2 mt-0">
            <span className="text-xs font-semibold uppercase text-gray-500 tracking-wider">
              {isEmagrecimento ? "Acompanhamento Semanal" : "Projeções Energéticas (Próximas Semanas)"}
            </span>
          </Divider>
          
          {/* Sem onRegistrarPeso (perfil sem permissão para editar dietas), o registro de peso fica oculto */}
          {isEmagrecimento && dieta.status === 2 && onRegistrarPeso && (
            <div className="mb-2 flex justify-between items-center">
              <span className="text-sm text-gray-600">
                Semana Atual: <strong>{semanaAtual}</strong> / 15
              </span>
              <Button 
                type="primary" 
                size="small" 
                icon={<PlusOutlined />} 
                onClick={handleOpenPesoModal}
                disabled={todasSemanasRegistradas}
              >
                Registrar Peso Real
              </Button>
            </div>
          )}

          <div className="bg-blue-50 border border-blue-100 rounded-md p-2 text-sm overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th className={thClass}>Semana</th>
                  {isEmagrecimento && <th className={thClass}>Peso Projetado (kg)</th>}
                  <th className={thClass}>Demanda (Kcal)</th>
                  <th className={thClass}>Ração (g/dia)</th>
                  {isEmagrecimento && <th className={thClass}>Peso Real (kg)</th>}
                  {isEmagrecimento && (
                    <th className={thClass}>
                      <Tooltip title="Variação semanal real calculada em relação ao último peso real registrado (ou, na primeira pesagem, em relação ao peso inicial da dieta).">
                        <span className="cursor-help">
                          Variação Real {percentualEsperado !== null && `(meta ${formatadorUmaCasa.format(percentualEsperado)}%)`}
                        </span>
                      </Tooltip>
                    </th>
                  )}
                  {isEmagrecimento && onRemoverPeso && <th className={`${thClass} w-12 text-center`}></th>}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {previsoes.map((prev, idx) => {
                  const isCurrent = dieta.status === 2 && prev.semana === semanaAtual;
                  const registro = dieta.registrosPeso?.find(r => r.semana === prev.semana);
                  const variacao = registro ? calcularVariacao(registro) : null;
                  const atingiuMeta = variacao && percentualEsperado !== null && variacao.percentual >= percentualEsperado;

                  return (
                    <tr key={idx} className={isCurrent ? "bg-blue-100 font-semibold" : ""}>
                      <td className={tdClass}>Semana {prev.semana}</td>
                      {isEmagrecimento && (
                        <td className={tdClass}>
                          {prev.alvoAtingido ? <span className="text-green-600">Alvo Atingido</span> : formatadorDuasCasas.format(prev.pesoProjetado)}
                        </td>
                      )}
                      <td className={tdClass}>{formatadorDuasCasas.format(prev.kcal)}</td>
                      <td className={tdClass}>
                        {dieta.racaoUtilizada === "Ração não informada" ? "-" : formatadorUmaCasa.format(prev.kcal / (dieta.necessidadeEnergeticaKcal / gramas))}
                      </td>
                      {isEmagrecimento && (
                        <td className={tdClass}>
                          {registro ? (
                            <Tag color="blue" className="m-0 inline-block min-w-[64px] text-center">
                              {formatadorDuasCasas.format(registro.pesoReal)}
                            </Tag>
                          ) : "-"}
                        </td>
                      )}
                      {isEmagrecimento && (
                        <td className={tdClass}>
                          {variacao ? (
                            <Tooltip
                              title={
                                variacao.percentual > LIMITE_PERDA_SEVERA
                                  ? `Perda muito severa (> ${LIMITE_PERDA_SEVERA}% por semana) – pode ser prejudicial ao animal. Perda de ${formatadorDuasCasas.format(variacao.kg)} kg/semana em relação a ${variacao.referencia}.`
                                  : `${variacao.kg >= 0 ? "Perda" : "Ganho"} de ${formatadorDuasCasas.format(Math.abs(variacao.kg))} kg/semana em relação a ${variacao.referencia}.`
                              }
                            >
                              <Tag
                                color={
                                  variacao.percentual > LIMITE_PERDA_SEVERA ? "purple"
                                    : variacao.percentual < 0 ? "red"
                                    : atingiuMeta ? "green" : "orange"
                                }
                                className="m-0 inline-block min-w-[78px] text-center"
                              >
                                {variacao.percentual > LIMITE_PERDA_SEVERA ? "⚠ " : variacao.percentual >= 0 ? "↓ " : "↑ "}
                                {formatadorDuasCasas.format(Math.abs(variacao.percentual))}%
                              </Tag>
                            </Tooltip>
                          ) : "-"}
                        </td>
                      )}
                      {isEmagrecimento && onRemoverPeso && (
                        <td className={`${tdClass} text-center`}>
                          {registro && (
                            <Popconfirm
                              title="Remover este registro de peso?"
                              okText="Remover"
                              cancelText="Cancelar"
                              onConfirm={() => onRemoverPeso(dieta.id, registro.id)}
                            >
                              <Tooltip title="Remover registro">
                                <Button type="text" size="small" danger icon={<DeleteOutlined />} />
                              </Tooltip>
                            </Popconfirm>
                          )}
                        </td>
                      )}
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>

          {isEmagrecimento && isGato && percentualEsperado !== null && percentualEsperado > LIMITE_PERDA_SEVERA && (
            <Alert
              type="warning"
              showIcon
              className="mt-3"
              message={`Meta de ${formatadorUmaCasa.format(percentualEsperado)}% por semana: acima do recomendado para gatos`}
              description="Perdas acima de 1% do peso por semana podem ser prejudiciais ao gato (risco de lipidose hepática). O recomendado é 1% por semana (Carciofi, 2013; Gomes, 2016)."
            />
          )}

          {isEmagrecimento && (
            <Alert
              type="info"
              showIcon
              className="mt-3"
              message="Ao atingir o peso alvo"
              description="As semanas depois do alvo mostram a energia de manutenção do peso alvo. Aumente a quantidade de ração aos poucos até esse valor e reavalie o animal (Guimarães e Tudury, 2006; Carciofi, 2013)."
            />
          )}

          {isEmagrecimento && (
            <>
              <Divider orientation="left" plain className="mb-2 mt-4">
                <span className="text-xs font-semibold uppercase text-gray-500 tracking-wider">
                  Evolução do Peso: Estimado x Real
                </span>
              </Divider>
              <div className="bg-white border border-gray-100 rounded-md p-2">
                <GraficoPesoSemana
                  previsoes={previsoes}
                  registros={dieta.registrosPeso || []}
                  pesoInicial={dieta.pesoAtual}
                  pesoAlvo={pesoAlvo}
                />
              </div>

              {totalParaPerder > 0 && (
                <div className="mt-3 p-3 bg-gray-50 border border-gray-200 rounded-md text-sm">
                  <div className="flex justify-between items-center mb-1">
                    <span className="font-medium text-gray-700">Progresso até o Peso Alvo</span>
                    <span className="text-xs text-gray-600">
                      <strong>{formatadorDuasCasas.format(Math.max(totalPerdido, 0))} kg</strong> de{" "}
                      <strong>{formatadorDuasCasas.format(totalParaPerder)} kg</strong> ({percentualProgresso}%)
                    </span>
                  </div>
                  <Progress
                    percent={percentualProgresso}
                    size="small"
                    strokeColor={colors.verde.medio}
                    trailColor={colors.cinza.borda}
                  />
                  {estimativaAlvoTexto && (
                    <div className="text-xs text-gray-500 mt-1">{estimativaAlvoTexto}</div>
                  )}
                </div>
              )}
            </>
          )}
        </div>
      )}

      <Divider dashed className="my-3" />

      {/* 3. Dados Gerais da Prescrição */}
      <Descriptions
        column={1}
        size="small"
        bordered
        labelStyle={{
          backgroundColor: "var(--verde-fraco)",
          color: "var(--verde-escuro)",
          fontWeight: 500,
          width: "140px",
        }}
        contentStyle={{ backgroundColor: colors.branco }}
      >
        <Descriptions.Item label="Ração Base">{racao}</Descriptions.Item>
        <Descriptions.Item label="Tipo de Dieta">
          {dieta.tipoDietaLabel}
        </Descriptions.Item>
        {pesoAlvo > 0 && (
          <Descriptions.Item label={isEmagrecimento ? "Peso Alvo" : "Peso Ideal / Alvo"}>
            {formatadorDuasCasas.format(pesoAlvo)} kg
          </Descriptions.Item>
        )}
        <Descriptions.Item label="Observações">
          {dieta.observacoes || "Nenhuma"}
        </Descriptions.Item>
      </Descriptions>

      {/* 4. Sugestão Prática de Refeições (não se aplica ao emagrecimento: a ração varia por semana) */}
      {!isEmagrecimento && (
        <>
          <Divider orientation="left" plain className="mb-2 mt-4">
            <span className="text-xs font-semibold uppercase text-gray-500 tracking-wider">
              Sugestão de Refeições
            </span>
          </Divider>

          <div className="bg-white p-3 border border-gray-100 rounded shadow-sm text-sm flex flex-col gap-1.5 text-gray-700">
            <Text>
              • <strong>2x ao dia:</strong> {formatadorUmaCasa.format(gramas / 2)}g por refeição
            </Text>
            <Text>
              • <strong>3x ao dia:</strong> {formatadorUmaCasa.format(gramas / 3)}g por refeição
            </Text>
            {gramas > 150 && (
              <Text>
                • <strong>4x ao dia:</strong> {formatadorUmaCasa.format(gramas / 4)}g por refeição
              </Text>
            )}
          </div>
        </>
      )}

      {/* Modal para Registrar Peso */}
      <ModalPadrao
        modo="criacao"
        open={isPesoModalVisible}
        onCancel={handleClosePesoModal}
        title="Registrar Peso Semanal"
        icon={<LineChartOutlined />}
        formId="form-registrar-peso"
        isSubmitting={salvandoPeso}
        tamanho="sm"
      >
        <Form id="form-registrar-peso" form={formPeso} layout="vertical" onFinish={submitPeso} initialValues={{ semana: semanaSugerida }}>
          <Form.Item
            name="semana"
            label="Semana do plano"
            rules={[{ required: true, message: "Obrigatório" }]}
          >
            <InputNumber className="w-full" min={1} max={15} precision={0} placeholder="Ex: 1" />
          </Form.Item>
          <Form.Item
            name="pesoReal"
            label="Peso Real do Animal (kg)"
            rules={[{ required: true, message: "Obrigatório" }]}
          >
            <InputNumber className="w-full" min={0.1} step={0.1} decimalSeparator="," placeholder="Ex: 4,8" addonAfter="kg" />
          </Form.Item>
          <Form.Item name="observacao" label="Observação (Opcional)" className="mb-0">
            <Input.TextArea rows={2} placeholder="Ex: Animal apresentou letargia" />
          </Form.Item>
        </Form>
      </ModalPadrao>
    </div>
  );
};

export default DetalhesDieta;
