/**
 * Select com busca remota (para listas grandes, que não cabem inteiras no select).
 * Não chama a API: quem usa passa a função `buscarOpcoes(busca)`, que deve devolver
 * uma lista no formato dos endpoints /opcoes: [{ id, nome, isAtivo }].
 * Itens inativos aparecem desabilitados, com o sufixo "(inativo)".
 *
 * Props principais:
 * - value / onChange: vêm do Form.Item.
 * - buscarOpcoes(busca): função que busca as opções (chamada com debounce).
 * - opcoesAtuais: [{ id, nome }] do registro em edição, para o valor atual sempre ter rótulo
 *   mesmo que não venha na busca (ex.: item inativo ou fora dos primeiros resultados).
 * - dependencias: lista de valores que, ao mudar, recarregam as opções (ex.: [especie]).
 * - demais props (mode, placeholder, disabled, allowClear...) vão direto para o Select do antd.
 */
import React, { useEffect, useMemo, useRef, useState } from "react";
import { Select, Spin } from "antd";

const ATRASO_BUSCA_MS = 400;

const paraOpcao = (item) => ({
  value: item.id,
  label: item.isAtivo === false ? `${item.nome} (inativo)` : item.nome,
  disabled: item.isAtivo === false,
});

const SelectBuscaRemota = ({
  buscarOpcoes,
  opcoesAtuais = [],
  dependencias = [],
  placeholder = "Digite para buscar",
  ...props
}) => {
  const [opcoes, setOpcoes] = useState([]);
  const [carregando, setCarregando] = useState(false);
  const temporizador = useRef(null);
  const ultimaBusca = useRef(0);

  const carregar = async (busca) => {
    const idBusca = ++ultimaBusca.current; // ignora respostas de buscas antigas
    setCarregando(true);
    try {
      const itens = await buscarOpcoes(busca);
      if (idBusca === ultimaBusca.current) setOpcoes((itens || []).map(paraOpcao));
    } catch {
      if (idBusca === ultimaBusca.current) setOpcoes([]);
    } finally {
      if (idBusca === ultimaBusca.current) setCarregando(false);
    }
  };

  // Carrega as primeiras opções ao montar e quando as dependências mudam
  useEffect(() => {
    carregar("");
    return () => clearTimeout(temporizador.current);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, dependencias);

  const handleSearch = (texto) => {
    clearTimeout(temporizador.current);
    temporizador.current = setTimeout(() => carregar(texto), ATRASO_BUSCA_MS);
  };

  // Garante rótulo para o valor atual do registro, mesmo que ele não esteja na busca
  const opcoesComAtuais = useMemo(() => {
    const ids = new Set(opcoes.map((o) => o.value));
    const extras = (opcoesAtuais || [])
      .filter((item) => item && item.id != null && !ids.has(item.id))
      .map((item) => ({ value: item.id, label: item.nome }));
    return [...extras, ...opcoes];
  }, [opcoes, opcoesAtuais]);

  return (
    <Select
      showSearch
      filterOption={false}
      onSearch={handleSearch}
      options={opcoesComAtuais}
      loading={carregando}
      notFoundContent={carregando ? <Spin size="small" /> : "Nenhum resultado"}
      placeholder={placeholder}
      {...props}
    />
  );
};

export default SelectBuscaRemota;
