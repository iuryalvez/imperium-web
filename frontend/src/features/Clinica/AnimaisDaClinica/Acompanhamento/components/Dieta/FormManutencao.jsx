import React, { useState, useEffect } from "react";
import { Form, InputNumber, Row, Col, Select, message } from "antd";
import { acompanhamentoAPI } from "../../../../../../api/clinica/acompanhamentoAPI";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const FormManutencao = ({ form, especie }) => {
  const [perfis, setPerfis] = useState([]);
  const [loading, setLoading] = useState(false);

  // Considera 0 ou 'Gato' para gato, dependendo de como seu backend retorna o Enum de Especie
  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";

  const perfilSelecionado = Form.useWatch("perfilMetabolico", form);

  useEffect(() => {
    const fetchPerfis = async () => {
      setLoading(true);
      try {
        const data = await acompanhamentoAPI.getPerfisMetabolicos();
        setPerfis(data);
      } catch (error) {
        message.error("Erro ao carregar perfis metabólicos.");
      } finally {
        setLoading(false);
      }
    };
    fetchPerfis();
  }, []);

  // Filtra os perfis baseados na espécie
  const perfisFiltrados = perfis.filter((p) => {
    const isPerfilGato = p.label.toLowerCase().includes("gato");
    return isGato ? isPerfilGato : !isPerfilGato;
  });

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Manutenção
      </h4>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="pesoObjetivo"
            label={
              <LabelComInfo
                label="Peso Alvo do Ciclo (kg)"
                tituloPopover="Peso Alvo do Ciclo"
                conteudoPopover="A meta de peso apenas para esta prescrição específica (ex: emagrecimento gradual). Se deixado em branco, o sistema usará o Peso Ideal cadastrado."
              />
            }
          >
            <InputNumber
              placeholder="Ex: 17,5"
              style={{ width: "100%" }}
              min={0}
              step={0.01}
              decimalSeparator=","
            />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="perfilMetabolico"
            label={
              <LabelComInfo
                label="Perfil Metabólico (Fórmula NRC)"
                tituloPopover="Perfil Metabólico"
                conteudoPopover="Opcional. Se deixado em branco, o sistema calculará a fórmula automaticamente com base na espécie e condição corporal."
              />
            }
          >
            <Select
              allowClear
              loading={loading}
              options={perfisFiltrados}
              placeholder="Selecione um perfil para forçar uma fórmula específica..."
            />
          </Form.Item>
        </Col>
      </Row>

      {isGato && perfilSelecionado === 1 && (
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="multiplicadorEnergeticoCustom"
              label={
                <LabelComInfo
                  label="Multiplicador Energético (k) para Gato Exótico"
                  tituloPopover="Multiplicador Energético"
                  conteudoPopover="Para gatos exóticos, o valor K pode variar de 55 a 260. Defina aqui o valor exato a ser utilizado no cálculo."
                />
              }
              rules={[
                { required: true, message: "O multiplicador é obrigatório para este perfil." },
                { type: 'number', min: 55, max: 260, message: "O valor deve estar entre 55 e 260" }
              ]}
            >
              <InputNumber
                placeholder="Ex: 157.5"
                style={{ width: "100%" }}
                min={55}
                max={260}
                step={0.5}
                decimalSeparator="."
              />
            </Form.Item>
          </Col>
        </Row>
      )}
    </div>
  );
};

export default FormManutencao;
