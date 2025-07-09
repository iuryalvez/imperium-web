import React from 'react';
import { Form, Input, Checkbox, Typography, Tag } from 'antd';

const perfisSistema = [
    { label: "Administrador", value: "ADMIN" },
    { label: "Gestor", value: "GESTOR" },
    { label: "Técnico", value: "TECNICO" },
    { label: "Secretário", value: "SECRETARIO" },
    { label: "Veterinário", value: "VETERINARIO" },
    { label: "Inativo", value: "INATIVO" },
];

const EdicaoUsuarioDoSistema = ({ form, valoresIniciais }) => {
    // Usamos `valoresIniciais` para determinar o comportamento inicial e as restrições
    const isAdmin = valoresIniciais?.isAdmin || false;

    // Filtra as opções de perfis com base se o usuário é admin
    const opcoesDePerfil = perfisSistema.filter(perfil => {
        if (isAdmin) {
            return ['ADMIN', 'INATIVO'].includes(perfil.value);
        }
        return !['ADMIN'].includes(perfil.value);
    }).map(perfil => ({
        ...perfil,
        // Desabilita a opção 'ADMIN' para sempre, pois um admin não pode ser editado aqui
        // e um usuário normal não pode virar admin por esta tela.
        disabled: perfil.value === 'ADMIN'
    }));

    return (
        <>
            {/* Campos ocultos para garantir que os IDs sejam enviados na submissão */}
            <Form.Item name="id" hidden><Input /></Form.Item>
            <Form.Item name="upcId" hidden><Input /></Form.Item>
            <Form.Item name="isAdmin" hidden><Input /></Form.Item>

            <Form.Item label="Nome Completo" name="nome" rules={[{ required: true, message: "O nome é obrigatório" }]}>
                <Input placeholder="Digite o nome completo" />
            </Form.Item>
            <Form.Item label="E-mail" name="email" rules={[{ required: true, message: "O e-mail é obrigatório" }, { type: 'email' }]}>
                <Input placeholder="exemplo@email.com" />
            </Form.Item>
            
            {/* Lógica de perfis e clínica */}
            {isAdmin ? (
                // Se for admin, exibe o perfil e o status de forma informativa
                <>
                    <Typography.Text strong>Perfil: </Typography.Text>
                    <Tag color="gold">ADMINISTRADOR</Tag>
                    <Form.Item name="perfis" label="Status" valuePropName="checked" style={{marginTop: '16px'}}>
                        <Checkbox.Group>
                            <Checkbox value="INATIVO">Marcar como Inativo</Checkbox>
                        </Checkbox.Group>
                    </Form.Item>
                </>
            ) : (
                // Se não for admin, permite a edição dos perfis e mostra a clínica
                <>
                    <Form.Item label="Perfis na Clínica" name="perfis" rules={[{ required: true, message: "Selecione ao menos um perfil" }]}>
                        <Checkbox.Group
                            options={opcoesDePerfil}
                            style={{ display: 'flex', flexDirection: 'column', gap: 8 }}
                        />
                    </Form.Item>
                    <Form.Item label="Clínica">
                        <Input value={valoresIniciais?.nomeClinica || 'Não informado'} disabled />
                    </Form.Item>
                </>
            )}
        </>
    );
};

export default EdicaoUsuarioDoSistema;