import React, { useEffect, useState } from "react";
import {
  Form,
  Input,
  Select,
  Row,
  Col,
  InputNumber,
  Alert,
  Divider,
  Tag,
  Checkbox,
} from "antd";
import { useWatch } from "antd/es/form/Form";
import { InfoCircleOutlined } from "@ant-design/icons";
import { message } from "antd";

import { colors } from "../../../../../../theme/colors";
import { acompanhamentoAPI } from "../../../../../../api/clinica/acompanhamentoAPI";
import FormManutencao from "./FormManutencao";
import FormCrescimento from "./FormCrescimento";
import FormLactacao from "./FormLactacao";
import FormGestacao from "./FormGestacao";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const opcoesTipoDieta = [
  { value: 1, label: "Manutenção" },
  { value: 2, label: "Crescimento" },
  { value: 3, label: "Lactação" },
  { value: 4, label: "Gestação" },
  { value: 5, label: "Emagrecimento" },
];

const FormularioDieta = ({ onFinish, initialValues, racoes, dadosRaca }) => {
  const [form] = Form.useForm();
  const tipoDietaSelecionado = useWatch("tipoDieta", form);
  
  const isCustomizado = useWatch("isCustomizado", form);
  const multiplicadorCustom = useWatch("multiplicadorEnergeticoCustom", form);
  const potenciaCustom = useWatch("potenciaCustom", form);
  const pesoAtualWatch = useWatch("pesoAtual", form);

  const isGato =
    initialValues?.especieAnimal === 2 ||
    String(initialValues?.especieAnimal).toLowerCase() === "gato";

  const [perfisMetabolicos, setPerfisMetabolicos] = useState([]);
  const [loadingPerfis, setLoadingPerfis] = useState(false);

  console.log("[FormularioDieta] initialValues: ", initialValues);

  useEffect(() => {
    const fetchPerfis = async () => {
      setLoadingPerfis(true);
      try {
        const data = await acompanhamentoAPI.getPerfisMetabolicos();
        setPerfisMetabolicos(data);
      } catch (error) {
        message.error("Erro ao carregar perfis metabólicos.");
      } finally {
        setLoadingPerfis(false);
      }
    };
    fetchPerfis();
  }, []);

  useEffect(() => {
    if (isCustomizado) {
      form.setFieldValue("perfilMetabolico", undefined);
    }
  }, [isCustomizado, form]);

  useEffect(() => {
    if (initialValues) {
      const tipoDietaMap = {
        Manutencao: 1,
        Crescimento: 2,
        Lactacao: 3,
        Gestacao: 4,
      };

      let parsedTipoDieta = initialValues.tipoDieta;
      if (typeof parsedTipoDieta === "string") {
        parsedTipoDieta = tipoDietaMap[parsedTipoDieta] || parsedTipoDieta;
      }

      form.setFieldsValue({
        racaoId: initialValues.racaoId,
        tipoDieta: parsedTipoDieta,
        pesoAtual: initialValues.peso,
        pesoIdeal: initialValues.pesoIdeal,
        numeroFilhotes: initialValues.isLactanteNumeroFilhotes,
        semanaLactacao: initialValues.semanaLactacao,
        perfilMetabolico: initialValues.perfilMetabolico,
        multiplicadorEnergeticoCustom: initialValues.multiplicadorEnergeticoCustom,
        potenciaCustom: initialValues.potenciaCustom,
        isCustomizado: !!(initialValues.multiplicadorEnergeticoCustom || initialValues.potenciaCustom),
      });
    }
  }, [initialValues, form]);

  const racoesOptions =
    racoes?.map((r) => ({ label: r.nome, value: r.id })) || [];

  const renderSpecificForm = () => {
    const perfisFiltrados = perfisMetabolicos.filter((p) => {
      const isPerfilGato = p.label.toLowerCase().includes("gato");
      const especieMatch = isGato ? isPerfilGato : !isPerfilGato;
      const dietaMatch = p.tiposDieta?.includes(tipoDietaSelecionado);
      return especieMatch && dietaMatch;
    });

    switch (tipoDietaSelecionado) {
      case 1:
      case 5:
        return (
          <FormManutencao
            form={form}
            especie={initialValues?.especieAnimal} // 0/1 ou 'Cao'/'Gato' dependendo do seu DTO
            perfis={perfisFiltrados}
            loading={loadingPerfis}
          />
        );
      case 2:
        return <FormCrescimento />;
      case 3:
        return <FormLactacao />;
      case 4:
        return <FormGestacao />;
      default:
        return null;
    }
  };

  return (
    <Form
      id="form-dieta-animal"
      form={form}
      layout="vertical"
      onFinish={onFinish}
    >
      <Alert
        className="mb-4"
        type="info"
        showIcon
        message="Dados vinculados da Avaliação Clínica"
        description="Os dados foram preenchidos automaticamente com base na avaliação registrada nesta consulta."
      />
      <Row gutter={16}>
        <Col span={12}>
          <Form.Item
            name="racaoId"
            label="Ração Base"
            rules={[{ required: true }]}
          >
            <Select
              placeholder="Selecione a ração"
              showSearch
              optionFilterProp="label"
              options={racoesOptions}
            />
          </Form.Item>
        </Col>
        <Col span={12}>
          <Form.Item
            name="tipoDieta"
            label="Tipo da Dieta"
            rules={[{ required: true }]}
          >
            <Select
              options={opcoesTipoDieta}
              placeholder="Selecione o tipo da dieta"
            />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" className="mt-0 mb-4 text-gray-500 text-sm">
        Parâmetros de Peso e Condição
      </Divider>

      {/* Exibição do ECC registrado no relatório */}
      {isGato && (
        <div className="mb-4 flex items-center bg-gray-50 border border-gray-200 p-3 rounded-md">
          <span className="text-gray-600 mr-2 text-sm">
            Condição Corporal (ECC) Registrada:
          </span>
          <Tag
            color={
              initialValues?.condicaoCorporal === "Ideal" ||
              initialValues?.condicaoCorporal === 5 ||
              initialValues?.condicaoCorporal
                ?.toString()
                ?.toLowerCase()
                ?.includes("ideal")
                ? "green"
                : "orange"
            }
            className="m-0 font-bold"
          >
            {initialValues?.condicaoCorporal
              ? `${initialValues.condicaoCorporal}`
              : "N/A"}
          </Tag>
          <span className="text-xs text-gray-400 ml-2">
            (Definido na avaliação clínica)
          </span>
        </div>
      )}

      {dadosRaca?.nome && (dadosRaca.pesoMin || dadosRaca.pesoMax) && (
        <Alert
          className="mb-4"
          type="info"
          showIcon
          icon={<InfoCircleOutlined />}
          message={`Referência Literária: ${dadosRaca.nome}`}
          description={`O peso ideal desta raça, ajustado para o sexo do paciente, varia entre ${dadosRaca.pesoMin || "?"}kg e ${dadosRaca.pesoMax || "?"}kg.`}
        />
      )}

      <Row gutter={16}>
        <Col span={12}>
          <Form.Item
            name="pesoAtual"
            label="Peso Atual (kg)"
            rules={[{ required: true }]}
          >
            <InputNumber
              style={{ width: "100%" }}
              placeholder="Ex: 15,0"
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>

        <Col span={12}>
          <Form.Item
            name="pesoIdeal"
            label={
              <LabelComInfo
                label="Peso Referência / Ideal (kg)"
                tituloPopover="Peso Referência / Ideal"
                conteudoPopover="Ficará salvo no cadastro do paciente e será a nova base para os cálculos energéticos, sobrescrevendo a raça."
              />
            }
          >
            <InputNumber
              placeholder="Ex: 15,0"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="mt-2">{renderSpecificForm()}</div>

      <Divider orientation="left" className="mt-4 mb-4 text-gray-500 text-sm">
        Customização do Perfil Metabólico (Opcional)
      </Divider>

      <Form.Item name="isCustomizado" valuePropName="checked" className="mb-2">
        <Checkbox>
          Desejo personalizar o Multiplicador (K) e a Potência (P) da fórmula NRC
        </Checkbox>
      </Form.Item>

      {isCustomizado && (
        <div
          className="p-3 rounded-md mb-4 text-center font-mono"
          style={{
            backgroundColor: colors.verde.fraco,
            borderColor: colors.verde.claro,
            borderWidth: 1,
            borderStyle: "solid",
            color: colors.verde.escuro,
          }}
        >
          <strong>Fórmula que será utilizada: </strong>
          {multiplicadorCustom || "K"} × ({pesoAtualWatch || "Peso"}kg)<sup>{potenciaCustom || "P"}</sup>
        </div>
      )}

      {isCustomizado && (
        <Row gutter={16}>
          <Col xs={24} md={12}>
            <Form.Item
              name="multiplicadorEnergeticoCustom"
              label={
                <LabelComInfo
                  label="Multiplicador Customizado (K)"
                  tituloPopover="Fator Customizado"
                  conteudoPopover="Personaliza o multiplicador K na fórmula. Ex: Para gatos exóticos pode variar de 55 a 260."
                />
              }
            >
              <InputNumber
                placeholder="Ex: 130"
                style={{ width: "100%" }}
                step={0.5}
                decimalSeparator=","
              />
            </Form.Item>
          </Col>
          <Col xs={24} md={12}>
            <Form.Item
              name="potenciaCustom"
              label={
                <LabelComInfo
                  label="Potência Customizada (P)"
                  tituloPopover="Potência Customizada"
                  conteudoPopover="Sobrescreve a potência P da fórmula selecionada (Geralmente 0,75 para cães ou 0,67 para gatos)."
                />
              }
            >
              <InputNumber
                placeholder="Ex: 0,75"
                style={{ width: "100%" }}
                step={0.01}
                decimalSeparator=","
              />
            </Form.Item>
          </Col>
        </Row>
      )}

      <Form.Item name="observacoes" label="Observações da Dieta">
        <Input.TextArea
          rows={3}
          placeholder="Instruções para o tutor (Ex: Adicionar água morna)"
        />
      </Form.Item>
    </Form>
  );
};

export default FormularioDieta;
