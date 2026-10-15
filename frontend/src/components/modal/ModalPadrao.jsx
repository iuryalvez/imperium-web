import React from "react";
import { Modal, Button, Space, Popconfirm } from "antd";
import { DeleteOutlined, EditOutlined, FormOutlined } from "@ant-design/icons";

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
  width = "min(800px, 95vw)",
  children,
}) => {
  const renderFooter = () => {
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
      style={{ top: 40 }}
      width={width}
      destroyOnClose
    >
      <div className="modal-scroll-container">{children}</div>
    </Modal>
  );
};

export default ModalPadrao;
