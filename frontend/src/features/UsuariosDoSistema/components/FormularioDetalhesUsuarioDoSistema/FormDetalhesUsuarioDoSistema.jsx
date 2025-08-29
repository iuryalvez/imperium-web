import React from "react";
import { Descriptions, Tag, Typography, Divider } from "antd";
import {
  formatarCPF,
  formatarTelefone,
  formatarData,
} from "../../../../utils/formatters"; // Ajuste o caminho para seus formatters
import { renderPerfis } from "../../../../utils/renderers";

const { Text, Link } = Typography;

const FormDetalhesUsuarioDoSistema = ({ valores }) => {
  const naoInformado = <Text type="secondary">Não informado</Text>;

  return (
    // Não precisamos de um <Form> aqui, pois é apenas para exibição
    <div>
      <Descriptions title="Identificação do Usuário" bordered column={2}>
        <Descriptions.Item label="Nome Completo" span={2}>
          {valores.nome || naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="CPF">
          {valores.cpf ? formatarCPF(valores.cpf) : naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Formação">
          {valores.formacao || naoInformado}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Contato" bordered column={1}>
        <Descriptions.Item label="E-mail">
          {valores.email ? (
            <Link href={`mailto:${valores.email}`}>{valores.email}</Link>
          ) : (
            naoInformado
          )}
        </Descriptions.Item>
        <Descriptions.Item label="Telefone Principal">
          {valores.telefone ? formatarTelefone(valores.telefone) : naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="WhatsApp">
          {valores.whatsapp ? (
            <Link href={`https://wa.me/55${valores.whatsapp}`} target="_blank">
              {formatarTelefone(valores.whatsapp)}
            </Link>
          ) : (
            naoInformado
          )}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Status e Vínculos no Sistema" bordered column={2}>
        <Descriptions.Item label="Status Geral">
          <Tag color={valores.isAtivo ? "green" : "red"}>
            {valores.isAtivo ? "ATIVO" : "INATIVO"}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="É Administrador?">
          <Tag color={valores.isAdmin ? "gold" : "default"}>
            {valores.isAdmin ? "Sim" : "Não"}
          </Tag>
        </Descriptions.Item>
        {!valores.isAdmin && (
          <>
            <Descriptions.Item label="Clínica Vinculada" span={2}>
              {valores.nomeClinica || naoInformado}
            </Descriptions.Item>
            <Descriptions.Item label="Perfis" span={2}>
              {renderPerfis(valores.perfis)}
            </Descriptions.Item>
          </>
        )}
      </Descriptions>

      <Divider />

      <Descriptions title="Histórico" bordered column={1}>
        <Descriptions.Item label="Data de Cadastro">
          {formatarData(valores.dataCadastro)}
        </Descriptions.Item>
        <Descriptions.Item label="Último Acesso">
          {formatarData(valores.dataUltimoAcesso)}
        </Descriptions.Item>
      </Descriptions>
    </div>
  );
};

export default FormDetalhesUsuarioDoSistema;
