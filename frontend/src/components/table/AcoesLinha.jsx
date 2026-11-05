import React, { useState } from "react";
import { Button, Dropdown, Popconfirm, Space, Tooltip } from "antd";
import { MoreOutlined } from "@ant-design/icons";

/**
 * AcoesLinha
 * Botões da coluna "Ações" de uma linha de tabela.
 * - Normal: botões pequenos só com ícone e Tooltip; os que pedem confirmação usam Popconfirm.
 * - compacta (celular): um botão "⋮" com um menu; a confirmação abre no próprio botão.
 *
 * Props:
 * - acoes: lista de { chave, rotulo, icone, onClick, tipo ("primary"), perigo, desabilitado,
 *   carregando, confirmar (título da pergunta) }. Itens falsy são ignorados, o que permite
 *   `podeEditar && { ... }`. Na confirmação também dá para passar `confirmarDescricao` (texto ou
 *   campo, ex.: motivo), `confirmarOk`, `confirmarCancelar` e `aoAbrirConfirmacao`.
 * - compacta: usa o menu "⋮" em vez dos botões.
 */
const AcoesLinha = ({ acoes, compacta = false }) => {
  const [pendente, setPendente] = useState(null); // ação esperando confirmação (modo compacto)
  const lista = acoes.filter(Boolean);

  if (!compacta) {
    return (
      <Space size={6}>
        {lista.map((acao) => {
          const botao = (
            <Button
              size="small"
              type={acao.tipo}
              icon={acao.icone}
              danger={acao.perigo}
              loading={acao.carregando}
              disabled={acao.desabilitado}
              aria-label={acao.rotulo}
              onClick={acao.confirmar ? undefined : acao.onClick}
            />
          );
          return (
            <Tooltip key={acao.chave} title={acao.rotulo}>
              {acao.confirmar ? (
                <Popconfirm
                  title={acao.confirmar}
                  description={acao.confirmarDescricao}
                  onConfirm={acao.onClick}
                  onOpenChange={(aberto) => aberto && acao.aoAbrirConfirmacao?.()}
                  okText={acao.confirmarOk ?? "Sim"}
                  cancelText={acao.confirmarCancelar ?? "Não"}
                  disabled={acao.desabilitado}
                >
                  {botao}
                </Popconfirm>
              ) : (
                botao
              )}
            </Tooltip>
          );
        })}
      </Space>
    );
  }

  const acaoPendente = lista.find((acao) => acao.chave === pendente);
  const carregando = lista.some((acao) => acao.carregando);

  const itens = lista.map((acao) => ({
    key: acao.chave,
    label: acao.rotulo,
    icon: acao.icone,
    danger: acao.perigo,
    disabled: acao.desabilitado,
  }));

  const aoClicar = ({ key }) => {
    const acao = lista.find((a) => a.chave === key);
    if (!acao) return;
    if (acao.confirmar) {
      acao.aoAbrirConfirmacao?.();
      setPendente(key);
    } else acao.onClick();
  };

  return (
    <Popconfirm
      open={!!acaoPendente}
      title={acaoPendente?.confirmar}
      description={acaoPendente?.confirmarDescricao}
      onConfirm={() => {
        acaoPendente?.onClick();
        setPendente(null);
      }}
      onCancel={() => setPendente(null)}
      onOpenChange={(aberto) => !aberto && setPendente(null)}
      okText={acaoPendente?.confirmarOk ?? "Sim"}
      cancelText={acaoPendente?.confirmarCancelar ?? "Não"}
      placement="left"
    >
      <Dropdown menu={{ items: itens, onClick: aoClicar }} trigger={["click"]} placement="bottomRight">
        <Button size="small" icon={<MoreOutlined />} loading={carregando} aria-label="Ações" />
      </Dropdown>
    </Popconfirm>
  );
};

export default AcoesLinha;
