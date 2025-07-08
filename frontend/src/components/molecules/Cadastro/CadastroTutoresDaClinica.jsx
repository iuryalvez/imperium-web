import React, { useState, useEffect } from 'react';
import { Form, Input, Select, Row, Col, Divider } from 'antd';
import { IMaskInput } from 'react-imask';
import { animaisDaClinicaAPI } from '../../../api/clinica/animaisDaClinicaAPI'; 

// Estilo para fazer o IMaskInput se parecer com um Input do Ant Design
const inputStyle = {
  width: '100%',
  padding: '4px 11px',
  border: '1px solid #d9d9d9',
  borderRadius: '6px',
  fontSize: '14px',
  lineHeight: '1.5715',
  transition: 'all 0.2s',
  backgroundColor: '#fff',
};

const FormularioCadastroTutoresDaClinica = ({ form }) => {
    const [animaisDaClinica, setAnimaisDaClinica] = useState([]);
    const [loadingAnimais, setLoadingAnimais] = useState(false);

    useEffect(() => {
        setLoadingAnimais(true);
        animaisDaClinicaAPI.listar()
            .then(data => {
                const animaisFormatados = data.map(animal => ({
                    label: `${animal.nome} (${animal.especie === 1 ? 'Cão' : 'Gato'})`,
                    value: animal.id,
                }));
                setAnimaisDaClinica(animaisFormatados);
            })
            .catch(err => console.error("Erro ao carregar animais da clínica:", err))
            .finally(() => setLoadingAnimais(false));
    }, []);

    return (
        <Row gutter={24}>
            {/* --- Coluna 1: Informações Pessoais e Contato --- */}
            <Col xs={24} lg={12}>
                <Divider orientation="left" plain>Informações Pessoais</Divider>
                <Form.Item label="Nome Completo" name="nome" rules={[{ required: true, message: "O nome é obrigatório" }]}>
                    <Input placeholder="Digite o nome do tutor" />
                </Form.Item>
                <Form.Item label="CPF" name="cpf" rules={[{ required: true, message: "O CPF é obrigatório" }]}>
                    <IMaskInput
                        mask="000.000.000-00"
                        unmask={true}
                        placeholder="000.000.000-00"
                        style={inputStyle}
                    />
                </Form.Item>
                <Form.Item label="E-mail" name="email" rules={[{ required: true, message: "O e-mail é obrigatório" }, { type: 'email' }]}>
                    <Input placeholder="exemplo@email.com" />
                </Form.Item>

                <Divider orientation="left" plain>Contato</Divider>
                <Form.Item label="Telefone Principal" name="telefone1">
                    <IMaskInput
                        mask="(00) 00000-0000"
                        unmask={true}
                        placeholder="(00) 99999-9999"
                        style={inputStyle}
                    />
                </Form.Item>
                <Form.Item label="WhatsApp" name="whatsapp">
                    <IMaskInput
                        mask="(00) 00000-0000"
                        unmask={true}
                        placeholder="(00) 99999-9999"
                        style={inputStyle}
                    />
                </Form.Item>
            </Col>

            {/* --- Coluna 2: Endereço e Associação de Animais --- */}
            <Col xs={24} lg={12}>
                <Divider orientation="left" plain>Endereço (Opcional)</Divider>
                <Form.Item label="CEP" name="cep">
                    <IMaskInput
                        mask="00000-000"
                        unmask={true}
                        placeholder="00000-000"
                        style={inputStyle}
                    />
                </Form.Item>
                <Form.Item label="Endereço" name="endereco">
                    <Input placeholder="Rua, Avenida, etc." />
                </Form.Item>
                <Form.Item label="Número" name="endNumero">
                    <Input placeholder="Nº" />
                </Form.Item>
                
                <Divider orientation="left" plain>Associar Animais (Opcional)</Divider>
                <Form.Item label="Animais já existentes" name="animalId">
                    <Select
                        mode="multiple"
                        showSearch
                        placeholder="Busque por nome"
                        loading={loadingAnimais}
                        options={animaisDaClinica}
                        optionFilterProp="label"
                    />
                </Form.Item>
            </Col>
        </Row>
    );
};

export default FormularioCadastroTutoresDaClinica;