import { Descriptions, Tag, Typography, Divider } from 'antd';

import {
    formatarCPF,
    formatarTelefone,
    renderPerfis
} from '../../../utils/formatters';

const DetalhesUsuariosDoSistema = ({ valores }) => {
    const naoInformado = <Typography.Text type="secondary">Não informado</Typography.Text>;

    return (
        <>
            <Descriptions title="Identificação do Usuário" bordered column={2}>
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

            <Descriptions title="Status e Vínculos no Sistema" bordered column={2}>
                <Descriptions.Item label="Status Geral">
                    <Tag color={valores.isAtivo ? 'green' : 'red'}>
                        {valores.isAtivo ? 'ATIVO' : 'INATIVO'}
                    </Tag>
                </Descriptions.Item>
                <Descriptions.Item label="É Administrador?">
                    <Tag color={valores.isAdmin ? 'gold' : 'default'}>
                        {valores.isAdmin ? 'Sim' : 'Não'}
                    </Tag>
                </Descriptions.Item>
                {!valores.isAdmin && (
                    <>
                        <Descriptions.Item label="Clínica Vinculada" span={2}>
                            {valores.nomeClinica ?? naoInformado}
                        </Descriptions.Item>
                        <Descriptions.Item label="Perfis" span={2}>
                            {renderPerfis(valores.perfis)}
                        </Descriptions.Item>
                    </>
                )}
            </Descriptions>
        </>
    );
};

export default DetalhesUsuariosDoSistema;