import React from 'react';
import { Typography, Tag } from 'antd';
import dayjs from 'dayjs';
import { FaCat, FaDog } from 'react-icons/fa';

/**
 * Formata uma data para o padrão DD/MM/YYYY às HH:mm.
 * Retorna um componente 'Nunca' se a data for nula.
 * @param {string | Date} data - A data a ser formatada.
 * @returns {React.Component | string}
 */
export const formatarData = (data) => {
    if (!data) return <Typography.Text type="secondary">Nunca</Typography.Text>;
    return dayjs(data).format('DD/MM/YYYY [às] HH:mm');
};

/**
 * Retorna uma cor de Tag do Ant Design com base no nome do perfil.
 * @param {string} perfil - O nome do perfil (ex: 'GESTOR').
 * @returns {string} O nome da cor para a Tag.
 */
export const getCorDoPerfil = (perfil) => {
    if (!perfil) return 'default';
    const perfilUpper = perfil.toUpperCase();
    if (perfilUpper.includes('ADMIN')) return 'gold';
    if (perfilUpper.includes('GESTOR')) return 'blue';
    if (perfilUpper.includes('VETERINARIO')) return 'purple';
    if (perfilUpper.includes('TECNICO')) return 'cyan';
    if (perfilUpper.includes('SECRETARIO')) return 'geekblue';
    if (perfilUpper.includes('INATIVO')) return 'red';
    return 'default';
};

/**
 * Formata um CPF (12345678900 -> 123.456.789-00).
 * @param {string} cpf - String contendo 11 dígitos.
 * @returns {string} O CPF formatado.
 */
export const formatarCPF = (cpf) => {
    if (!cpf || cpf.length !== 11) return cpf;
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
};

/**
 * Formata um telefone (ex: 62999999999 -> (62) 99999-9999).
 * @param {string} tel - String contendo 10 ou 11 dígitos.
 * @returns {string} O telefone formatado.
 */
export const formatarTelefone = (tel) => {
    if (!tel || tel.length < 10) return tel;
    if (tel.length === 11) {
        return tel.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
    return tel.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
};

/**
 * Formata um CEP (ex: 74000000 -> 74000-000).
 * @param {string} cep - String contendo 8 dígitos.
 * @returns {string} O CEP formatado.
 */
export const formatarCEP = (cep) => {
    if (!cep || cep.length !== 8) return cep;
    return cep.replace(/(\d{5})(\d{3})/, '$1-$2');
};

/**
 * Converte o ID da espécie para o nome correspondente.
 * @param {number} value - 1 para Cão, 2 para Gato.
 * @returns {string}
 */
export const formatarEspecie = (value) => (value === 1 ? 'Cão' : 'Gato');

/**
 * Converte o ID do sexo para o nome correspondente.
 * @param {number} value - 1 para Macho, 2 para Fêmea.
 * @returns {string}
 */
export const formatarSexo = (value) => (value === 1 ? 'Macho' : 'Fêmea');

/**
 * Converte o ID do grau de atividade para o texto correspondente.
 * @param {number} value - 1, 2 ou 3.
 * @returns {React.Component | string}
 */
export const formatarGrauAtividade = (value) => {
    switch (value) {
        case 1: return 'Baixo / Inativo';
        case 2: return 'Moderado / Normal';
        case 3: return 'Alto / Atleta';
        default: return <Typography.Text type="secondary">Não informado</Typography.Text>;
    }
};

/**
 * Renderiza um conjunto de perfis como Tags coloridas.
 * @param {string[]} perfis - Uma lista de strings de perfil.
 * @returns {React.Component}
 */
export const renderPerfis = (perfis = []) => {
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

export const renderAnimais = (animais = []) => {
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
