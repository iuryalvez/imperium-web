import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import dayjs from 'dayjs';

// --- Funções Auxiliares ---

// Formata datas, tratando casos nulos
const formatarData = (data) => {
    if (!data) return <Typography.Text type="secondary">Nunca</Typography.Text>;
    return dayjs(data).format('DD/MM/YYYY [às] HH:mm');
};

// Mapeia nomes de perfis para cores de Tag para melhor visualização
const getCorDoPerfil = (perfil) => {
    const perfilUpper = perfil.toUpperCase();
    if (perfilUpper.includes('ADMIN')) return 'blue';
    if (perfilUpper.includes('GESTOR')) return 'green';
    if (perfilUpper.includes('VETERINARIO')) return 'green';
    if (perfilUpper.includes('TECNICO')) return 'green';
    if (perfilUpper.includes('SECRETARIO')) return 'green';
    if (perfilUpper.includes('INATIVO')) return 'red';
    return 'default';
};

// Renderiza a lista de perfis como Tags coloridas
const renderPerfis = (perfis = []) => {
    if (!perfis || perfis.length === 0) {
        return <Typography.Text type="secondary">Nenhum perfil atribuído</Typography.Text>;
    }
    return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
            {perfis.map(perfil => (
                <Tag key={perfil} color={getCorDoPerfil(perfil)}>
                    {perfil.toUpperCase()}
                </Tag>
            ))}
        </div>
    );
};

// --- Componente Principal ---

const DetalhesUsuariosDoSistema = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        <>
            <Descriptions title="Identificação do Usuário" bordered column={1}>
                <Descriptions.Item label="Nome Completo">{valores.nome ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="Username (Login)">{valores.userName ?? naoInformado}</Descriptions.Item>
                <Descriptions.Item label="E-mail">
                    {valores.email ? <Typography.Link href={`mailto:${valores.email}`}>{valores.email}</Typography.Link> : naoInformado}
                </Descriptions.Item>
            </Descriptions>

            <Divider />

            <Descriptions title="Status e Vínculos no Sistema" bordered column={2}>
                <Descriptions.Item label="Status Geral">
                    <Tag color={valores.isAtivo ? 'green' : 'red'}>
                        {valores.isAtivo ? 'ATIVO' : 'INATIVO'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="É Administrador?">
                    <Tag color={valores.isAdmin ? 'green' : 'default'}>
                        {valores.isAdmin ? 'Sim' : 'Não'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Clínica Vinculada" span={2}>
                    {valores.nomeClinica ?? naoInformado}
                </Descriptions.Item>
                <Descriptions.Item label="Perfis" span={2}>
                    {renderPerfis(valores.perfis)}
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

export default DetalhesUsuariosDoSistema;