import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import dayjs from 'dayjs';

// As funções de formatação continuam úteis
const formatarData = (data) => {
    if (!data) return <Typography.Text type="secondary">Não informada</Typography.Text>;
    return dayjs(data).format('DD/MM/YYYY [às] HH:mm');
};

const DetalhesClinica = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        // Usamos um Fragment <> pois o Descriptions já provê a estrutura.
        <>
            <Typography.Title level={4}>{`Detalhes da Clínica`}</Typography.Title>
            <Descriptions title="Informações Gerais" bordered column={2}>
                <Descriptions.Item label="Nome da Clínica" span={2}>
                    {valores.nomeClinica}
                </Descriptions.Item>
                <Descriptions.Item label="Nome Fantasia">
                    {valores.nomeFantasia ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="CNPJ">
                    {valores.cnpj ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Status">
                    {/* Usar Tag é a forma ideal de exibir status */}
                    <Tag color={valores.isAtiva ? 'green' : 'red'}>
                        {valores.isAtiva ? 'ATIVA' : 'INATIVA'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Data de Cadastro">
                    {formatarData(valores.dataCad)}
                </Descriptions.Item>
            </Descriptions>

            <Divider /> 

            <Descriptions title="Endereço" bordered column={2}>
                <Descriptions.Item label="Logradouro" span={2}>
                    {valores.endereco ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Número">
                    {valores.endNumero || naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="CEP">
                    {valores.cep ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Cidade">
                    {valores.endCidade ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Estado">
                    {valores.endEstado ?? naoInformado}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Contato" bordered column={1}>
                <Descriptions.Item label="E-mail">
                    {/* Podemos adicionar um link funcional para o e-mail */}
                    {valores.email 
                        ? <Typography.Link href={`mailto:${valores.email}`}>{valores.email}</Typography.Link> 
                        : naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Telefone Principal">
                    {valores.telefone1 ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="WhatsApp">
                    {/* E também para o WhatsApp */}
                    {valores.whatsapp 
                        ? <Typography.Link href={`https://wa.me/55${valores.whatsapp}`} target="_blank">{valores.whatsapp}</Typography.Link> 
                        : naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Instagram">
                    {valores.instagram 
                        ? <Typography.Link href={`https://instagram.com/${valores.instagram}`} target="_blank">@{valores.instagram}</Typography.Link> 
                        : naoInformado}
                </Descriptions.Item>
            </Descriptions>
        </>
    );
};

export default DetalhesClinica;