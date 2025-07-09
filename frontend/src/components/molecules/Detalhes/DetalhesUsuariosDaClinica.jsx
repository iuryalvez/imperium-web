import React from 'react';
import { Descriptions, Tag, Typography, Divider } from 'antd';
import dayjs from 'dayjs';

// --- Funções Auxiliares (podem ser importadas de um arquivo comum 'utils.js') ---

const formatarData = (data) => {
    if (!data) return <Typography.Text type="secondary">Nunca</Typography.Text>;
    return dayjs(data).format('DD/MM/YYYY [às] HH:mm');
};

const getCorDoPerfil = (perfil) => {
    const perfilUpper = perfil.toUpperCase();
    if (perfilUpper.includes('GESTOR')) return 'green';
    if (perfilUpper.includes('VETERINARIO')) return 'green';
    if (perfilUpper.includes('TECNICO')) return 'green';
    if (perfilUpper.includes('SECRETARIO')) return 'green';
    return 'default';
};

const renderPerfis = (perfis = []) => {
    if (!perfis || perfis.length === 0) {
        return <Typography.Text type="secondary">Nenhum perfil ativo nesta clínica</Typography.Text>;
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

const DetalhesUsuariosDaClinica = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    // Lógica para determinar o status e os perfis a serem exibidos
    const isAtivoNaClinica = !valores.perfis?.includes('INATIVO');
    const perfisVisiveis = valores.perfis?.filter(p => p.toUpperCase() !== 'INATIVO') || [];

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

            <Descriptions title="Acesso na Clínica" bordered column={1}>
                <Descriptions.Item label="Status na Clínica">
                    <Tag color={isAtivoNaClinica ? 'green' : 'red'}>
                        {isAtivoNaClinica ? 'ATIVO' : 'INATIVO'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Perfis Atribuídos">
                    {renderPerfis(perfisVisiveis)}
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