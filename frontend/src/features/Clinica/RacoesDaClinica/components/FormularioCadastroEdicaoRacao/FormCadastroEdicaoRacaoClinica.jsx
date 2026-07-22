import React, { useEffect } from 'react';
import { Form, Input, Radio, InputNumber, Row, Col, Divider } from 'antd';

const opcoesEspecie = [
    { label: 'Cão', value: 1 },
    { label: 'Gato', value: 2 },
];

const opcoesUnidade = [
    { label: 'Porcentagem (%)', value: true },
    { label: 'Gramas (g/kg)', value: false },
];

const FormCadastroEdicaoRacaoClinica = ({ initialValues, onFinish }) => {
    const [form] = Form.useForm();

    // 1. Observa a seleção de unidade em tempo real
    const isPorcentagem = Form.useWatch('isPorcentagem', form);

    useEffect(() => {
        if (initialValues) {
            const especieComoNumero = initialValues.especie === 'Cao' ? 1 : 2;
            form.setFieldsValue({ ...initialValues, especie: especieComoNumero });
        } else {
            // Define valores padrão para um novo formulário
            form.resetFields();
            form.setFieldsValue({ isPorcentagem: true });
        }
    }, [initialValues, form]);
    
    // 2. Define o sufixo e o valor máximo com base na seleção
    const suffix = isPorcentagem ? '%' : 'g/kg';
    const maximo = isPorcentagem ? 100 : 1000;

    return (
        <Form id="form-cadastro-edicao-racao-clinica" form={form} layout="vertical" onFinish={onFinish} autoComplete="off">
            <Row gutter={24}>
                <Col xs={24} md={12}>
                    <Form.Item name="nome" label="Nome da Ração" rules={[{ required: true }]}>
                        <Input placeholder="Ex: Ração Super Premium" />
                    </Form.Item>
                </Col>
                <Col xs={24} md={12}>
                    <Form.Item name="especie" label="Espécie" rules={[{ required: true }]}>
                        <Radio.Group options={opcoesEspecie} optionType="button" buttonStyle="solid" />
                    </Form.Item>
                </Col>
            </Row>

            {/* 3. Novo campo para selecionar a unidade */}
            <Form.Item name="isPorcentagem" label="Unidade da Composição" rules={[{ required: true }]}>
                <Radio.Group options={opcoesUnidade} optionType="button" buttonStyle="solid" />
            </Form.Item>

            {/* 4. Divider e campos de InputNumber agora são dinâmicos */}
            <Divider orientation="left" plain>Composição Nutricional ({suffix})</Divider>

            <Row gutter={24}>
                <Col xs={12} sm={8}>
                    <Form.Item name="umidade" label="Umidade" rules={[{ required: true }]}>
                        <InputNumber style={{ width: '100%' }} placeholder='0,00' min={0} max={maximo} step={0.1} decimalSeparator="," addonAfter={suffix} />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="proteinaBruta" label="Proteína Bruta" rules={[{ required: true }]}>
                        <InputNumber style={{ width: '100%' }} placeholder='0,00' min={0} max={maximo} step={0.1} decimalSeparator="," addonAfter={suffix} />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="extratoEtereo" label="Extrato Etéreo" rules={[{ required: true }]}>
                        <InputNumber style={{ width: '100%' }} placeholder='0,00' min={0} max={maximo} step={0.1} decimalSeparator="," addonAfter={suffix} />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="materiaMineral" label="Matéria Mineral" rules={[{ required: true }]}>
                        <InputNumber style={{ width: '100%' }} placeholder='0,00' min={0} max={maximo} step={0.1} decimalSeparator="," addonAfter={suffix} />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="fibraBruta" label="Fibra Bruta" rules={[{ required: true }]}>
                        <InputNumber style={{ width: '100%' }} placeholder='0,00' min={0} max={maximo} step={0.1} decimalSeparator="," addonAfter={suffix} />
                    </Form.Item>
                </Col>
            </Row>
        </Form>
    );
};

export default FormCadastroEdicaoRacaoClinica;