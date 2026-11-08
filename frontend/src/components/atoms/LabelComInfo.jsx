import React from "react";
import { Popover, Typography, Space } from "antd";
import { InfoCircleOutlined } from "@ant-design/icons";

const { Text } = Typography;

const LabelComInfo = ({ label, tituloPopover, conteudoPopover }) => (
  <Space size={4} className="items-center">
    <span>{label}</span>
    <Popover
      title={
        <Text className="font-bold text-verde-escuro">{tituloPopover}</Text>
      }
      content={
        <div className="max-w-md max-h-80 overflow-y-auto text-xs leading-relaxed text-gray-600">
          {conteudoPopover}
        </div>
      }
      trigger="click"
      placement="right"
    >
      <InfoCircleOutlined className="text-azul-info cursor-pointer hover:text-azul-hover transition-colors" />
    </Popover>
  </Space>
);

export default LabelComInfo;
