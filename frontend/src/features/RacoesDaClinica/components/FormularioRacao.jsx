import React, { useEffect } from 'react';
import { Form, Input, Button, Radio, InputNumber, Row, Col, Divider } from 'antd';

const opcoesEspecie = [
    { label: 'Cão', value: 1 },
    { label: 'Gato', value: 2 },
];

const FormularioRacao = ({ initialValues, onFinish, onCancel }) => {
    const [form] = Form.useForm();

    useEffect(() => {
        if (initialValues) {
            // Converte a espécie de string ("Cao") para número (1) para preencher o formulário
            const especieComoNumero = initialValues.especie === 'Cao' ? 1 : initialValues.especie === 'Gato' ? 2 : undefined;
            form.setFieldsValue({
                ...initialValues,
                especie: especieComoNumero,
            });
        } else {
            form.resetFields();
        }
    }, [initialValues, form]);

    return (
        <Form form={form} layout="vertical" onFinish={onFinish} autoComplete="off">
            <Row gutter={24}>
                <Col xs={24} md={12}>
                    <Form.Item name="nome" label="Nome da Ração" rules={[{ required: true, message: 'O nome é obrigatório.' }]}>
                        <Input placeholder="Ex: Ração Super Premium" />
                    </Form.Item>
                </Col>
                <Col xs={24} md={12}>
                    <Form.Item name="especie" label="Espécie" rules={[{ required: true, message: 'A espécie é obrigatória.' }]}>
                        <Radio.Group options={opcoesEspecie} optionType="button" buttonStyle="solid" />
                    </Form.Item>
                </Col>
            </Row>

            <Divider orientation="left" plain>Composição Nutricional (%)</Divider>

            <Row gutter={24}>
                <Col xs={12} sm={8}>
                    <Form.Item name="umidade" label="Umidade" rules={[{ required: true, message: 'Obrigatório' }]}>
                        <InputNumber style={{ width: '100%' }} min={0} max={100} step={0.1} decimalSeparator="," addonAfter="%" />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="proteinaBruta" label="Proteína Bruta" rules={[{ required: true, message: 'Obrigatório' }]}>
                        <InputNumber style={{ width: '100%' }} min={0} max={100} step={0.1} decimalSeparator="," addonAfter="%" />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="extratoEtereo" label="Extrato Etéreo" rules={[{ required: true, message: 'Obrigatório' }]}>
                        <InputNumber style={{ width: '100%' }} min={0} max={100} step={0.1} decimalSeparator="," addonAfter="%" />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="materiaMineral" label="Matéria Mineral" rules={[{ required: true, message: 'Obrigatório' }]}>
                        <InputNumber style={{ width: '100%' }} min={0} max={100} step={0.1} decimalSeparator="," addonAfter="%" />
                    </Form.Item>
                </Col>
                <Col xs={12} sm={8}>
                    <Form.Item name="fibraBruta" label="Fibra Bruta" rules={[{ required: true, message: 'Obrigatório' }]}>
                        <InputNumber style={{ width: '100%' }} min={0} max={100} step={0.1} decimalSeparator="," addonAfter="%" />
                    </Form.Item>
                </Col>
            </Row>

            <div className="flex justify-end gap-2 mt-6">
                <Button onClick={onCancel}>Cancelar</Button>
                <Button type="primary" htmlType="submit">Salvar</Button>
            </div>
        </Form>
    );
};

export default FormularioRacao;