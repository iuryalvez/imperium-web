import React from 'react';
import { Form, Input, Checkbox, Typography } from 'antd';

const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const FormularioCadastroUsuarioDaClinica = () => {
    return (
        <Form>
            <Typography.Title level={4}>{`Cadastro de Usuário`}</Typography.Title>
            <Form.Item label="Nome Completo" name="nome" rules={[{ required: true, message: "O nome é obrigatório" }]}>
                <Input placeholder="Digite o nome completo" />
            </Form.Item>
            <Form.Item label="E-mail" name="email" rules={[{ required: true, message: "O e-mail é obrigatório" }, { type: 'email', message: "Formato de e-mail inválido" }]}>
                <Input placeholder="exemplo@email.com" />
            </Form.Item>
            <Form.Item label="Senha" name="senha" rules={[{ required: true, message: "A senha é obrigatória" }]}>
                <Input.Password placeholder="Crie uma senha forte" />
            </Form.Item>
            <Form.Item
                label="Confirmar Senha"
                name="confirmarSenha"
                dependencies={['senha']}
                rules={[
                    { required: true, message: "Confirme sua senha" },
                    ({ getFieldValue }) => ({
                        validator(_, value) {
                            if (!value || getFieldValue('senha') === value) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('As senhas não coincidem!'));
                        },
                    }),
                ]}
            >
                <Input.Password placeholder="Digite a senha novamente" />
            </Form.Item>
            <Form.Item label="Perfis na Clínica" name="perfis" rules={[{ required: true, message: "Selecione ao menos um perfil" }]}>
                <Checkbox.Group options={perfisClinica} style={{ display: 'flex', flexDirection: 'column', gap: 8 }} />
            </Form.Item>
        </Form>
    );
};

export default FormularioCadastroUsuarioDaClinica;