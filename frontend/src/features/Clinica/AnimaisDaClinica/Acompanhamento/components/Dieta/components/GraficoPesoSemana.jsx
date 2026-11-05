import React, { useMemo } from "react";
import {
  ResponsiveContainer,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ReferenceLine,
} from "recharts";
import { colors } from "../../../../../../../theme/colors";

const formatarPeso = (valor) =>
  typeof valor === "number"
    ? `${valor.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 })} kg`
    : "-";

/**
 * Gráfico comparativo Peso x Semana (estimado vs. real) para dietas de emagrecimento.
 * A semana 0 representa o peso inicial do animal (ponto de partida comum às duas linhas).
 */
const GraficoPesoSemana = ({ previsoes = [], registros = [], pesoInicial, pesoAlvo }) => {
  const dados = useMemo(() => {
    const pontos = [];

    if (typeof pesoInicial === "number") {
      pontos.push({ semana: 0, estimado: pesoInicial, real: pesoInicial });
    }

    previsoes.forEach((prev) => {
      const registro = registros.find((r) => r.semana === prev.semana);
      pontos.push({
        semana: prev.semana,
        estimado: prev.pesoProjetado,
        real: registro ? registro.pesoReal : null,
      });
    });

    return pontos;
  }, [previsoes, registros, pesoInicial]);

  if (dados.length === 0) return null;

  return (
    <div className="w-full h-[260px]">
      <ResponsiveContainer>
        <LineChart data={dados} margin={{ top: 10, right: 16, left: 0, bottom: 16 }}>
          <CartesianGrid strokeDasharray="3 3" stroke={colors.cinza.borda} />
          <XAxis
            dataKey="semana"
            tickFormatter={(v) => (v === 0 ? "Início" : `S${v}`)}
            tick={{ fontSize: 12 }}
            label={{ value: "Semana", position: "insideBottom", offset: -8, fontSize: 12 }}
          />
          <YAxis
            domain={["dataMin - 0.3", "dataMax + 0.3"]}
            tickFormatter={(v) => v.toFixed(1)}
            tick={{ fontSize: 12 }}
            width={40}
            label={{ value: "kg", angle: -90, position: "insideLeft", fontSize: 12 }}
          />
          <Tooltip
            formatter={(valor, nome) => [formatarPeso(valor), nome]}
            labelFormatter={(v) => (v === 0 ? "Peso inicial" : `Semana ${v}`)}
          />
          <Legend verticalAlign="top" height={28} />
          {typeof pesoAlvo === "number" && pesoAlvo > 0 && (
            <ReferenceLine
              y={pesoAlvo}
              stroke={colors.grafico.alvo}
              strokeDasharray="6 4"
              label={{ value: `Alvo ${pesoAlvo.toLocaleString("pt-BR")} kg`, position: "insideBottomRight", fontSize: 11, fill: colors.grafico.alvo }}
            />
          )}
          <Line
            type="monotone"
            dataKey="estimado"
            name="Peso estimado"
            stroke={colors.verde.claro}
            strokeWidth={2}
            strokeDasharray="5 3"
            dot={{ r: 3 }}
          />
          <Line
            type="monotone"
            dataKey="real"
            name="Peso real"
            stroke={colors.grafico.real}
            strokeWidth={2.5}
            dot={{ r: 4 }}
            connectNulls
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};

export default GraficoPesoSemana;
