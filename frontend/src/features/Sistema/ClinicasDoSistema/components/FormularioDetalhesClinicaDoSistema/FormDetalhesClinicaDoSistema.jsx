import React from "react";
import { Descriptions, Tag, Typography, Divider } from "antd";
import dayjs from "dayjs";
import { formatarCEP, formatarCNPJ, formatarTelefone } from "../../../../../utils/formatters";

const { Text, Link } = Typography;

const FormularioDetalhesClinicaDoSistema = ({ valores }) => {
  const naoInformado = <Text type="secondary">Não informado</Text>;

  return (
    <div>
      <Descriptions title="Informações Gerais" bordered column={{ xs: 1, sm: 2 }}>
        <Descriptions.Item label="Razão Social" span={2}>{valores.nomeClinica}</Descriptions.Item>
        <Descriptions.Item label="Nome Fantasia">{valores.nomeFantasia ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="CNPJ">{formatarCNPJ(valores.cnpj) ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Status">
          <Tag color={valores.isAtiva ? 'green' : 'red'}>{valores.isAtiva ? 'ATIVA' : 'INATIVA'}</Tag>
        </Descriptions.Item>
        <Descriptions.Item label="Data Cadastro">{valores.dataCadastro ? dayjs(valores.dataCadastro).format("DD/MM/YYYY") : naoInformado}</Descriptions.Item>
      </Descriptions>

      <Divider />
      <Descriptions title="Localização" bordered column={{ xs: 1, sm: 2 }}>
        <Descriptions.Item label="CEP">{formatarCEP(valores.cep) ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Bairro">{valores.bairro ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Logradouro" span={2}>{valores.logradouro ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Complemento">{valores.complemento ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Cidade/UF" span={2}>
          {valores.cidade && valores.uf ? `${valores.cidade} - ${valores.uf}` : (valores.cidade || valores.uf || naoInformado)}
        </Descriptions.Item>
        <Descriptions.Item label="Estado">{valores.estado ?? naoInformado}</Descriptions.Item>
      </Descriptions>

      <Divider />
      <Descriptions title="Contato" bordered column={1}>
        <Descriptions.Item label="E-mail">{valores.email ? <Link href={`mailto:${valores.email}`}>{valores.email}</Link> : naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Telefone">{formatarTelefone(valores.telefone) ?? naoInformado}</Descriptions.Item>
        <Descriptions.Item label="WhatsApp">{valores.whatsapp ? <Link href={`https://wa.me/55${valores.whatsapp}`} target="_blank">{formatarTelefone(valores.whatsapp)}</Link> : naoInformado}</Descriptions.Item>
        <Descriptions.Item label="Instagram">{valores.instagram ? <Link href={`https://instagram.com/${valores.instagram}`} target="_blank">@{valores.instagram}</Link> : naoInformado}</Descriptions.Item>
      </Descriptions>
    </div>
  );
};

export default FormularioDetalhesClinicaDoSistema;