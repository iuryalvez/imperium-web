import React from 'react';
import { Typography } from 'antd';
import dayjs from 'dayjs';

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
 * Formata uma data para o padrão DD/MM/YYYY às HH:mm.
 * Retorna um componente 'Nunca' se a data for nula.
 * @param {string | Date} data - A data a ser formatada.
 * @returns {React.Component | string}
 */
export const formatarDataCadastroPerfil = (data) => {
    if (!data) return <Typography.Text type="secondary">Nunca</Typography.Text>;
    return dayjs(data).format('DD/MM/YYYY');
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
 * Formata um CNPJ (12345678000199 -> 12.345.678/0001-99).
 * @param {string} cnpj - String contendo 14 dígitos.
 * @returns {string} O CNPJ formatado.
 */
export const formatarCNPJ = (cnpj) => {
    if (!cnpj || cnpj.length !== 14) return cnpj;
    return cnpj.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
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
        case 1: return 'Inativo';
        case 2: return 'Moderado';
        case 3: return 'Alto';
        default: return <Typography.Text type="secondary">Sem dados de consultas</Typography.Text>;
    }
};
/**
 * Idade a partir da data de nascimento, para ajudar no cadastro do animal.
 * Ex.: "2 anos e 3 meses", "1 ano", "5 meses", "menos de 1 mês".
 * @param {dayjs.Dayjs | string | Date} nascimento
 * @param {dayjs.Dayjs} [hoje] - referência (para testes).
 * @returns {string | null} null sem data ou com data futura.
 */
export const formatarIdade = (nascimento, hoje = dayjs()) => {
    if (!nascimento) return null;
    const data = dayjs(nascimento);
    if (!data.isValid() || data.isAfter(hoje, 'day')) return null;

    const totalMeses = hoje.diff(data, 'month');
    const anos = Math.floor(totalMeses / 12);
    const meses = totalMeses % 12;
    const texto = (n, singular, plural) => `${n} ${n === 1 ? singular : plural}`;

    if (anos === 0 && meses === 0) return 'menos de 1 mês';
    if (anos === 0) return texto(meses, 'mês', 'meses');
    if (meses === 0) return texto(anos, 'ano', 'anos');
    return `${texto(anos, 'ano', 'anos')} e ${texto(meses, 'mês', 'meses')}`;
};
