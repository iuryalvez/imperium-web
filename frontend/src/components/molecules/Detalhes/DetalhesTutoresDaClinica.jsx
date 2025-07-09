import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import { FaDog, FaCat } from 'react-icons/fa'; // Ícones para espécies: npm install react-icons

// --- Funções Auxiliares (Formatters) ---

// Formata um CPF (ex: 12345678900 -> 123.456.789-00)
const formatarCPF = (cpf) => {
    if (!cpf || cpf.length !== 11) return cpf;
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
};

// Formata um Telefone (ex: 62999999999 -> (62) 99999-9999)
const formatarTelefone = (tel) => {
    if (!tel || tel.length < 10) return tel;
    if (tel.length === 11) {
        return tel.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
    return tel.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
};

// Formata um CEP (ex: 74000000 -> 74000-000)
const formatarCEP = (cep) => {
    if (!cep || cep.length !== 8) return cep;
    return cep.replace(/(\d{5})(\d{3})/, '$1-$2');
};

// Renderiza a lista de animais associados com ícones
const renderAnimais = (animais = []) => {
    if (!animais || animais.length === 0) {
        return <Typography.Text type="secondary">Nenhum animal associado</Typography.Text>;
    }
    return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
            {animais.map(animal => (
                <Tag
                    key={animal.id}
                    icon={animal.especie === 1 ? <FaDog /> : <FaCat />}
                    style={{ display: 'flex', alignItems: 'center', gap: '4px', fontSize: '14px' }}
                >
                    {animal.nome}
                </Tag>
            ))}
        </div>
    );
};


// --- Componente Principal de Detalhes ---

const DetalhesTutoresDaClinica = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        <>
            <Descriptions title="Informações Pessoais" bordered column={1}>
                <Descriptions.Item label="Nome Completo">{valores.nome ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="CPF">{formatarCPF(valores.cpf) ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="E-mail">
                    {valores.email ? <Typography.Link href={`mailto:${valores.email}`}>{valores.email}</Typography.Link> : naoInformado}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Contato" bordered column={1}>
                <Descriptions.Item label="Telefone Principal">{formatarTelefone(valores.telefone1) ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="WhatsApp">
                    {valores.whatsapp ? <Typography.Link href={`https://wa.me/55${valores.whatsapp}`} target="_blank">{formatarTelefone(valores.whatsapp)}</Typography.Link> : naoInformado}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Endereço" bordered column={2}>
                <Descriptions.Item label="CEP" span={2}>{formatarCEP(valores.cep) ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="Endereço" span={2}>{valores.endereco ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="Número">{valores.endNumero || naoInformado}</Descriptions.Item>
            </Descriptions>
            
            <Divider />

            <Descriptions title="Animais Associados" bordered column={1}>
                {/* Assumindo que `valores.animais` é um array de objetos animal */}
                <Descriptions.Item label="Animais">{renderAnimais(valores.animais)}</Descriptions.Item>
            </Descriptions>
        </>
    );
};

export default DetalhesTutoresDaClinica;