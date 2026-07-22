import React from "react";
import { Modal, Button, Space } from "antd";

const ModalPadrao = ({
  open,
  onCancel,
  title,
  icon,
  formId,
  isSubmitting,
  modo = "edicao", // Aceita "criacao", "edicao" ou "detalhes"
  width = "min(800px, 95vw)",
  children,
}) => {
  // Dynamically renders the footer based on the modal's mode
  const renderFooter = () => {
    if (modo === "detalhes") {
      return (
        <div className="modal-footer-actions">
          <Button key="close" onClick={onCancel}>
            Fechar
          </Button>
        </div>
      );
    }

    return (
      <div className="modal-footer-actions">
        <Button key="cancel" onClick={onCancel}>
          Cancelar
        </Button>
        <Button
          key="submit"
          form={formId}
          type="primary"
          htmlType="submit"
          loading={isSubmitting}
        >
          {modo === "criacao" ? "Cadastrar" : "Salvar Alterações"}
        </Button>
      </div>
    );
  };

  return (
    <Modal
      title={
        <Space>
          {icon && React.cloneElement(icon, { className: "modal-title-icon" })}
          <span className="text-verde-escuro m-0 text-lg">
            {title}
          </span>
        </Space>
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
