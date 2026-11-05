import React from "react";
import { Modal, Button, Space, Popconfirm, Grid } from "antd";
import { DeleteOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";
import { PreencherAlturaContext } from "../layout/PaginaPadrao";

// Larguras padronizadas do modal (prop "tamanho").
const LARGURAS = { sm: 520, md: 800, lg: 1000 };

/**
 * ModalPadrao
 * Modal base do sistema: título com ícone, corpo com scroll interno e rodapé sempre visível.
 * Abaixo de md (768px) ocupa a tela inteira.
 *
 * Props principais:
 * - modo: "criacao" | "edicao" (rodapé com submit do form `formId`) | "detalhes" (só leitura).
 * - tamanho: "sm" | "md" (padrão) | "lg". `width` ainda é aceito para casos especiais.
 * - rodape: substitui o rodapé padrão (ex.: ações diferentes de salvar/fechar).
 * - onEdit / onDelete / isDeleting: ações opcionais do rodapé padrão.
 */
const ModalPadrao = ({
  open,
  onCancel,
  title,
  subtitle,
  icon,
  formId,
  isSubmitting,
  isDeleting, // Optional loading state for delete actions
  onEdit, // Triggers edit mode from details view
  onDelete, // Triggers deletion
  modo = "edicao",
  tamanho = "md",
  width,
  rodape,
  children,
}) => {
  const telas = Grid.useBreakpoint();
  const telaCheia = telas.md === false;

  const renderFooter = () => {
    if (rodape !== undefined) {
      return rodape && <div className="modal-footer-actions">{rodape}</div>;
    }

    if (modo === "detalhes") {
      // Renders View-only actions with optional Edit and Delete buttons
      return (
        <div className="modal-footer-actions">
          {onDelete && (
            <Popconfirm
              title="Tem certeza que deseja excluir este item?"
              onConfirm={onDelete}
              okText="Sim"
              cancelText="Não"
            >
              <Button danger icon={<DeleteOutlined />} loading={isDeleting}>
                Excluir
              </Button>
            </Popconfirm>
          )}
          {onEdit && (
            <Button
              key="edit"
              type="primary"
              onClick={onEdit}
              icon={<EditOutlined />}
            >
              Editar
            </Button>
          )}
          <Button key="close" onClick={onCancel}>
            Fechar
          </Button>
        </div>
      );
    }

    // Renders Form actions. Delete is conditionally shown only during 'edicao'
    return (
      <div className="modal-footer-actions">
        {modo === "edicao" && onDelete && (
          <Popconfirm
            title="Tem certeza que deseja excluir este item?"
            onConfirm={onDelete}
            okText="Sim"
            cancelText="Não"
          >
            <Button danger icon={<DeleteOutlined />} loading={isDeleting}>
              Excluir
            </Button>
          </Popconfirm>
        )}
        <Button key="cancel" onClick={onCancel}>
          Cancelar
        </Button>
        <Button
          key="submit"
          form={formId}
          type="primary"
          htmlType="submit"
          loading={isSubmitting}
          icon={<FormOutlined />}
        >
          {modo === "criacao" ? "Cadastrar" : "Salvar Alterações"}
        </Button>
      </div>
    );
  };

  // Desktop: altura máxima da janela menos as margens; o corpo rola e o rodapé fica fixo.
  // Tela cheia: o conteúdo ocupa 100% da altura e o corpo estica.
  const estilos = telaCheia
    ? {
        content: {
          height: "100dvh",
          borderRadius: 0,
          display: "flex",
          flexDirection: "column",
        },
        body: { flex: 1, minHeight: 0, overflowY: "auto", overflowX: "hidden" },
      }
    : {
        content: {
          maxHeight: "calc(100dvh - 48px)",
          display: "flex",
          flexDirection: "column",
        },
        body: { flex: 1, minHeight: 0, overflowY: "auto", overflowX: "hidden", paddingRight: 8 },
      };

  return (
    <Modal
      title={
        <div>
          <Space>
            {icon && React.cloneElement(icon, { className: "modal-title-icon" })}
            <span className="text-verde-escuro m-0 text-lg">{title}</span>
          </Space>
          {subtitle && (
            <div className="text-sm font-normal text-gray-500 mt-1 pl-7">
              {subtitle}
            </div>
          )}
        </div>
      }
      open={open}
      onCancel={onCancel}
      footer={renderFooter()}
      width={telaCheia ? "100%" : width ?? `min(${LARGURAS[tamanho] ?? LARGURAS.md}px, 95vw)`}
      style={
        telaCheia
          ? { top: 0, margin: 0, padding: 0, maxWidth: "100vw" }
          : { top: 24, paddingBottom: 0 }
      }
      styles={estilos}
      destroyOnClose
    >
      {/* Tabelas dentro do modal crescem com o conteúdo (não herdam o "preencher altura" da página) */}
      <PreencherAlturaContext.Provider value={false}>{children}</PreencherAlturaContext.Provider>
    </Modal>
  );
};

export default ModalPadrao;
