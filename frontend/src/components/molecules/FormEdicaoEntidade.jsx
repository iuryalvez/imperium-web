// src/components/formularios/FormEdicaoEntidade.jsx
import React, { useState, useEffect } from 'react';
import { Button, Form, Typography, message } from 'antd';
import { SaveOutlined } from '@ant-design/icons';

import EdicaoUsuarioDoSistema from './Edicao/EdicaoUsuariosDoSistema';

const componentesDeEdicao = {
    usuariosDoSistema: EdicaoUsuarioDoSistema,
    // ... outros componentes
};

const FormEdicaoEntidade = ({ entidade, valoresIniciais, onSubmit, onSuccess, onCancel, tituloModal }) => {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);

    // Efeito para popular o formulário quando os dados de edição são carregados
    useEffect(() => {
        if (valoresIniciais) {
            form.setFieldsValue(valoresIniciais);
        }
    }, [valoresIniciais, form]);

    const handleFinish = async (values) => {
        setLoading(true);
        try {
            // Inclui o ID nos valores a serem submetidos, caso não esteja visível
            const finalValues = { ...valoresIniciais, ...values };
            await onSubmit(finalValues);
            message.success(`${tituloModal.replace('Edição de', 'Editado(a)')} com sucesso.`);
            if (onSuccess) onSuccess();
        } catch (error) {
            console.error("Erro ao editar:", error);
            message.error(error.response?.data?.message || "Falha ao salvar as alterações.");
        } finally {
            setLoading(false);
        }
    };

    const EdicaoEntidade = componentesDeEdicao[entidade];

    return (
        <Form form={form} layout="vertical" onFinish={handleFinish} autoComplete="off">
            <Typography.Title level={4}>{tituloModal}</Typography.Title>
            
            {EdicaoEntidade ? (
                // Passamos `form` e `valoresIniciais` para o componente filho
                <EdicaoEntidade form={form} valoresIniciais={valoresIniciais} />
            ) : (
                <Typography.Text type="danger">
                    Formulário de edição para '{entidade}' não foi implementado.
                </Typography.Text>
            )}

            <div className="flex gap-4 mt-6">
                <Button onClick={onCancel} className="w-1/2">
                    Cancelar
                </Button>
                <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={loading} className="w-1/2">
                    Salvar Alterações
                </Button>
            </div>
        </Form>
    );
};

export default FormEdicaoEntidade;