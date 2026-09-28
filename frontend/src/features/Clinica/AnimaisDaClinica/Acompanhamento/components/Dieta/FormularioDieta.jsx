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
} from "antd";
import { useWatch } from "antd/es/form/Form";
import { InfoCircleOutlined } from "@ant-design/icons";
import { message } from "antd";

import { acompanhamentoAPI } from "../../../../../../api/clinica/acompanhamentoAPI";
import FormManutencao from "./components/FormManutencao";
import FormCrescimento from "./components/FormCrescimento";
import FormLactacao from "./components/FormLactacao";
import FormGestacao from "./components/FormGestacao";

const opcoesTipoDietaBase = [
  { value: 1, label: "Manutenção" },
  { value: 2, label: "Crescimento" },
  { value: 3, label: "Lactação" },
  { value: 4, label: "Gestação" },
  { value: 5, label: "Emagrecimento" },
];

const FormularioDieta = ({
  onFinish,
  initialValues,
  racoes,
  dadosRaca,
  dadosAnimal,
}) => {
  const [form] = Form.useForm();
  const tipoDietaSelecionado = useWatch("tipoDieta", form);

  const isGato =
    initialValues?.especieAnimal === 2 ||
    String(initialValues?.especieAnimal).toLowerCase() === "gato";

  const isCao =
    initialValues?.especieAnimal === 1 ||
    String(initialValues?.especieAnimal).toLowerCase() === "cao";

  const isEditing = !!initialValues?.id;

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

  const perfilSelecionadoId = useWatch("perfilMetabolico", form);
  const multiplicadorEnergetico = useWatch("multiplicadorEnergetico", form);
  const potencia = useWatch("potencia", form);

  useEffect(() => {
    if (perfilSelecionadoId && perfisMetabolicos.length > 0) {
      const perfil = perfisMetabolicos.find((p) => p.value === perfilSelecionadoId);
      if (perfil && perfil.k !== null && perfil.p !== null && perfil.k !== undefined && perfil.p !== undefined) {
        form.setFieldsValue({
          multiplicadorEnergetico: perfil.k,
          potencia: perfil.p,
        });
      }
    }
  }, [perfilSelecionadoId, perfisMetabolicos, form]);

  useEffect(() => {
    if (perfilSelecionadoId && perfisMetabolicos.length > 0) {
      const perfil = perfisMetabolicos.find((p) => p.value === perfilSelecionadoId);
      
      // Se é um perfil que tem K e P fixos no backend
      if (perfil && perfil.k !== null && perfil.p !== null && perfil.k !== undefined && perfil.p !== undefined) {
        // Se o usuário alterou K ou P para valores diferentes do perfil base
        if (
          (multiplicadorEnergetico !== undefined && Math.abs(multiplicadorEnergetico - perfil.k) > 0.001) ||
          (potencia !== undefined && Math.abs(potencia - perfil.p) > 0.001)
        ) {
          const tipo = form.getFieldValue("tipoDieta");
          const isGat = initialValues?.especieAnimal === 2 || String(initialValues?.especieAnimal).toLowerCase() === "gato";
          const termoBusca = isGat ? "gato" : "cão";
          
          const perfilGenerico = perfisMetabolicos.find(p => 
            p.tiposDieta?.includes(tipo) && 
            p.k === null && 
            p.p === null && 
            p.label.toLowerCase().includes(termoBusca)
          );

          let novoPerfilId = perfilGenerico ? perfilGenerico.value : null;

          if (novoPerfilId && novoPerfilId !== perfilSelecionadoId) {
            form.setFieldValue("perfilMetabolico", novoPerfilId);
          }
        }
      }
    }
  }, [multiplicadorEnergetico, potencia, perfilSelecionadoId, perfisMetabolicos, form, initialValues?.especieAnimal]);

  useEffect(() => {
    if (initialValues) {
      const tipoDietaMap = {
        Manutencao: 1,
        Crescimento: 2,
        Lactacao: 3,
        Gestacao: 4,
        Emagrecimento: 5,
      };

      let parsedTipoDieta = initialValues.tipoDieta;
      if (typeof parsedTipoDieta === "string") {
        parsedTipoDieta = tipoDietaMap[parsedTipoDieta] || parsedTipoDieta;
      }

      form.setFieldsValue({
        racaoId: initialValues.racaoId,
        tipoDieta: parsedTipoDieta,
        pesoAtual: initialValues.pesoAtual || initialValues.peso,
        pesoIdeal: initialValues.pesoIdeal,
        numeroFilhotes:
          initialValues.numeroFilhotes ||
          initialValues.isLactanteNumeroFilhotes,
        semanaLactacao: initialValues.semanaLactacao,
        semanaGestacao: initialValues.semanaGestacao,
        hasAdicionalGestacaoCao:
          initialValues.hasAdicionalGestacaoCao,
        perfilMetabolico: initialValues.perfilMetabolico,
        multiplicadorEnergetico:
          initialValues.multiplicadorEnergetico,
        potencia: initialValues.potencia,
        isCustomizado: !!(
          initialValues.multiplicadorEnergetico ||
          initialValues.potencia
        ),
      });
    }
  }, [initialValues, form]);

  const racoesOptions =
    racoes?.map((r) => ({ label: r.nome, value: r.id })) || [];

  const getFilteredOpcoesTipoDieta = () => {
    let filtered = [...opcoesTipoDietaBase];

    if (dadosAnimal) {
      const sexo = dadosAnimal.sexo?.toLowerCase();
      // Em C# a enum FaseVidaAnimal: Desconhecida(0), Filhote(1), Jovem(2), Adulto(3), Senior(4), Geriatrico(5)
      // O DTO stringifica para: "Adulto", "Senior", "Filhote", etc.
      const faseVida = dadosAnimal.faseVidaAtual?.toLowerCase() || "";

      if (sexo === "macho") {
        filtered = filtered.filter((o) => o.value !== 3 && o.value !== 4); // Remove Lactação e Gestação
      }

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

  const opcoesTipoDieta = getFilteredOpcoesTipoDieta();

  const renderSpecificForm = () => {
    const perfisFiltrados = perfisMetabolicos.filter((p) => {
      const currentPerfil =
        form.getFieldValue("perfilMetabolico") ||
        initialValues?.perfilMetabolico;
      const isSelected = currentPerfil && p.value === currentPerfil;

      const isPerfilGato = p.label.toLowerCase().includes("gato");
      const especieMatch = isGato ? isPerfilGato : !isPerfilGato;
      let dietaMatch = p.tiposDieta?.includes(tipoDietaSelecionado);
      
      // Para Lactação (3) de cães, também disponibilizamos Manutenção (1)
      if (tipoDietaSelecionado === 3 && isCao) {
        dietaMatch = dietaMatch || p.tiposDieta?.includes(1);
      }
      // Para Gestação (4), disponibiliza-se os mesmos perfis da Manutenção (1) além dos próprios perfis de gestação
      if (tipoDietaSelecionado === 4) {
        dietaMatch = dietaMatch || p.tiposDieta?.includes(1);
      }

      return (especieMatch && dietaMatch) || isSelected;
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
            tipoDietaSelecionado={tipoDietaSelecionado}
          />
        );
      case 2:
        return (
          <FormCrescimento
            form={form}
            perfis={perfisFiltrados}
            loading={loadingPerfis}
            especie={initialValues?.especieAnimal}
          />
        );
      case 3:
        return (
          <FormLactacao
            form={form}
            especie={initialValues?.especieAnimal}
            perfis={perfisFiltrados}
            loading={loadingPerfis}
          />
        );
      case 4:
        return (
          <FormGestacao
            form={form}
            perfis={perfisFiltrados}
            loading={loadingPerfis}
            especie={initialValues?.especieAnimal}
          />
        );
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
      {!isEditing && (
        <Alert
          className="mb-4"
          type="info"
          showIcon
          message="Dados vinculados da Avaliação Clínica"
          description="Os dados foram preenchidos automaticamente com base na avaliação registrada nesta consulta."
        />
      )}
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
        <Col span={24}>
          <Form.Item
            name="pesoAtual"
            label="Peso Atual (kg)"
            rules={[{ required: true }]}
          >
            <InputNumber
              style={{ width: "100%" }}
              placeholder="Ex: 15,0"
              min={0.01}
              max={300}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <div className="mt-2">{renderSpecificForm()}</div>

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
