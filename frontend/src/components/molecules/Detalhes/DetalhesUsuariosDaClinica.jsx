import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import {
    formatarData,
    renderPerfis,
    formatarCPF,      // <-- Importar
    formatarTelefone  // <-- Importar
} from '../../../utils/formatters';

const DetalhesUsuariosDaClinica = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        <>
        <Typography.Title level={4}>{`Dados do Usuário`}</Typography.Title>
            <Descriptions title="Informações Pessoais" bordered column={2}>
                <Descriptions.Item label="Nome Completo" span={2}>{valores.nome ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="CPF">{formatarCPF(valores.cpf) ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="Formação">{valores.formacao ?? naoInformado}</Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Contato" bordered column={1}>
                <Descriptions.Item label="E-mail">
                    {valores.email ? <Typography.Link href={`mailto:${valores.email}`}>{valores.email}</Typography.Link> : naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Telefone Principal">{formatarTelefone(valores.telefone1) ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="WhatsApp">
                    {valores.whatsapp ? <Typography.Link href={`https://wa.me/55${valores.whatsapp}`} target="_blank">{formatarTelefone(valores.whatsapp)}</Typography.Link> : naoInformado}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Acesso na Clínica" bordered column={1}>
                <Descriptions.Item label="Status na Clínica">
                    <Tag color={valores.isAtivoNaClinica ? 'green' : 'red'}>
                        {valores.isAtivoNaClinica ? 'ATIVO' : 'INATIVO'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Perfis Atribuídos">
                    {renderPerfis(valores.perfis?.filter(p => p.toUpperCase() !== 'INATIVO') || [])}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Histórico" bordered column={1}>
                <Descriptions.Item label="Data de Cadastro">{formatarData(valores.dataCadastro)}</Descriptions.Item>
                <Descriptions.Item label="Último Acesso">{formatarData(valores.dataUltimoAcesso)}</Descriptions.Item>
            </Descriptions>
        </>
    );
};

export default DetalhesUsuariosDaClinica;