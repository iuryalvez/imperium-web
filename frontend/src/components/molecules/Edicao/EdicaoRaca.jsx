import React, { useEffect } from 'react';
import { Form, Input, Button, Radio, InputNumber, Space, message } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { racasAPI } from '../../../api/sistema/racasAPI';

const opcoesEspecie = [
    { label: 'Cão', value: 1 },
    { label: 'Gato', value: 2 },
];

const FormularioEdicaoRaca = ({ onSuccess, onCancel, valoresIniciais }) => {
    const [form] = Form.useForm();
    const [loading, setLoading] = React.useState(false);

    // Popula o formulário com os dados da raça a ser editada
    useEffect(() => {
        if (valoresIniciais) {
            form.setFieldsValue(valoresIniciais);
        }
    }, [valoresIniciais, form]);

    const handleFinish = async (values) => {
        setLoading(true);
        try {
            await racasAPI.editar(valoresIniciais.id, values);
            message.success("Raça atualizada com sucesso!");
            if (onSuccess) onSuccess();
        } catch (error) {
            console.error("Erro ao editar raça:", error);
            message.error(error.message || "Falha ao editar raça.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Form form={form} layout="vertical" onFinish={handleFinish} autoComplete="off">
            <Form.Item name="id" hidden><Input /></Form.Item>
            <Form.Item name="nome" label="Nome da Raça" rules={[{ required: true }]}>
                <Input placeholder="Ex: Golden Retriever" />
            </Form.Item>
            <Form.Item name="especie" label="Espécie" rules={[{ required: true }]}>
                <Radio.Group options={opcoesEspecie} optionType="button" buttonStyle="solid" />
            </Form.Item>
            <Form.Item label="Peso de Referência (Opcional)">
                <Space align="baseline">
                    <Form.Item name="pesoMinimoReferencia">
                        <InputNumber min={0} step={0.1} decimalSeparator="," placeholder="Mínimo (kg)" />
                    </Form.Item>
                    <span>-</span>
                    <Form.Item name="pesoMaximoReferencia" dependencies={['pesoMinimoReferencia']} rules={[ /* ... sua regra de validação ... */ ]}>
                        <InputNumber min={0} step={0.1} decimalSeparator="," placeholder="Máximo (kg)" />
                    </Form.Item>
                </Space>
            </Form.Item>
            <div className="flex justify-end gap-2 mt-6">
                <Button onClick={onCancel}>Cancelar</Button>
                <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={loading}>
                    Salvar Alterações
                </Button>
            </div>
        </Form>
    );
};

export default FormularioEdicaoRaca;