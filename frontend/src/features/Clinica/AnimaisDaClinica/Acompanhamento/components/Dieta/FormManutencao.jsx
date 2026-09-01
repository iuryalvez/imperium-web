import React from "react";
import { Form, InputNumber, Row, Col, Select } from "antd";
import LabelComInfo from "../../../../../../components/atoms/LabelComInfo";

const FormManutencao = ({ form, especie, perfis, loading }) => {
  // Considera 0 ou 'Gato' para gato, dependendo de como seu backend retorna o Enum de Especie
  const isGato = especie === 2 || String(especie).toLowerCase() === "gato";

  const perfilSelecionado = Form.useWatch("perfilMetabolico", form);

  // Perfis já vêm filtrados por dieta (e espécie, dependendo de como foi feito no pai), 
  // mas caso queira garantir ou aplicar outro filtro:
  // No nosso caso o pai (FormularioDieta) já passou os perfisFiltrados que são adequados
  const perfisParaExibir = perfis || [];

  return (
    <div className="bg-gray-50 p-4 rounded-md border border-gray-200 mb-4">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">
        Ajuste Específico de Manutenção
      </h4>


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
              options={perfisParaExibir}
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
