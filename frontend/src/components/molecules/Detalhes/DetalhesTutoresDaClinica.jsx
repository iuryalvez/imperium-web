import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import { FaDog, FaCat } from 'react-icons/fa'; // Ícones para espécies: npm install react-icons

import {
    formatarCPF,
    formatarCEP,
    formatarTelefone,
    renderAnimais,
} from '../../../utils/formatters';

const DetalhesTutoresDaClinica = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        <>
        <Typography.Title level={4}>{`Dados do Tutor`}</Typography.Title>
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