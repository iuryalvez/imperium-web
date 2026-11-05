import React from "react";
import { Button, Checkbox, Dropdown } from "antd";
import { SettingOutlined } from "@ant-design/icons";

/**
 * SeletorColunas
 * Botão "Colunas" com a lista de colunas da tabela para mostrar ou esconder.
 * Usado pela TabelaPadrao quando recebe `selecionarColunas`.
 *
 * Props:
 * - colunas: colunas da tabela ({ key, title }).
 * - visiveis: keys das colunas visíveis.
 * - onAlterar(novasVisiveis): chamado ao marcar ou desmarcar. Sempre fica ao menos uma coluna.
 */
const SeletorColunas = ({ colunas, visiveis, onAlterar }) => {
  const alternar = (key) => {
    if (visiveis.includes(key)) {
      if (visiveis.length > 1) onAlterar(visiveis.filter((k) => k !== key));
    } else {
      onAlterar([...visiveis, key]);
    }
  };

  const menu = (
    <div className="flex flex-col gap-1 p-3 bg-white rounded-lg shadow-lg">
      {colunas.map((col) => (
        <Checkbox
          key={col.key}
          checked={visiveis.includes(col.key)}
          onChange={() => alternar(col.key)}
        >
          {col.title}
        </Checkbox>
      ))}
    </div>
  );

  return (
    <Dropdown dropdownRender={() => menu} trigger={["click"]} placement="bottomRight">
      <Button icon={<SettingOutlined />}>Colunas</Button>
    </Dropdown>
  );
};

export default SeletorColunas;
