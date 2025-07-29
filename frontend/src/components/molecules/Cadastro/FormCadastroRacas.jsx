import React from 'react';
import { Form, Input, Button, Radio, InputNumber, Space, message, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { racasAPI } from '../../../api/sistema/racasAPI'; 

const opcoesEspecie = [
    { label: 'Cão', value: 1 },
    { label: 'Gato', value: 2 },
];

const FormularioCadastroRaca = ({ onSuccess, onCancel }) => {
    const [form] = Form.useForm();
    const [loading, setLoading] = React.useState(false);

    const handleFinish = async (values) => {
        setLoading(true);
        try {
            await racasAPI.cadastrar(values);
            message.success("Raça cadastrada com sucesso!");
            form.resetFields();
            if (onSuccess) onSuccess(); // Avisa o componente pai sobre o sucesso
        } catch (error) {
            console.error("Erro ao cadastrar raça:", error);
            message.error(error.message || "Falha ao cadastrar raça.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Form
            form={form}
            layout="vertical"
            onFinish={handleFinish}
            autoComplete="off"
        >
            <Form.Item
                name="nome"
                label="Nome da Raça"
                rules={[{ required: true, message: 'O nome da raça é obrigatório.' }]}
            >
                <Input placeholder="Ex: Golden Retriever" />
            </Form.Item>

            <Form.Item
                name="especie"
                label="Espécie"
                rules={[{ required: true, message: 'A espécie é obrigatória.' }]}
            >
                <Radio.Group options={opcoesEspecie} optionType="button" buttonStyle="solid" />
            </Form.Item>
            
            <Form.Item label="Peso de Referência (Opcional)">
                <Space align="baseline">
                    <Form.Item name="pesoMinimoReferencia">
                        <InputNumber min={0} step={0.1} decimalSeparator="," placeholder="Mínimo (kg)" />
                    </Form.Item>
                    <span>-</span>
                    <Form.Item
                        name="pesoMaximoReferencia"
                        dependencies={['pesoMinimoReferencia']} // Depende do campo de peso mínimo
                        rules={[
                            ({ getFieldValue }) => ({
                                validator(_, value) {
                                    const min = getFieldValue('pesoMinimoReferencia');
                                    if (!value || !min || value >= min) {
                                        return Promise.resolve();
                                    }
                                    return Promise.reject(new Error('O peso máximo deve ser maior ou igual ao mínimo.'));
                                },
                            }),
                        ]}
                    >
                        <InputNumber min={0} step={0.1} decimalSeparator="," placeholder="Máximo (kg)" />
                    </Form.Item>
                </Space>
            </Form.Item>

            <div className="flex justify-end gap-2 mt-6">
                <Button onClick={onCancel}>
                    Cancelar
                </Button>
                <Button type="primary" htmlType="submit" icon={<PlusOutlined />} loading={loading}>
                    Cadastrar
                </Button>
            </div>
        </Form>
    );
};

export default FormularioCadastroRaca;