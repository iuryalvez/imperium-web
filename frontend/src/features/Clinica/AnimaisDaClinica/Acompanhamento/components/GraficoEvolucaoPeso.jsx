// // src/pages/Acompanhamento/components/GraficoEvolucaoPeso.jsx

// import React from 'react';
// import { Line } from '@ant-design/charts';
// import dayjs from 'dayjs';

// const GraficoEvolucaoPeso = ({ avaliacoes }) => {
//   // 1. Transforma os dados para o formato que o gráfico espera
//   const data = avaliacoes
//     .map(r => ({
//       data: dayjs(r.dataConsulta).format('DD/MM/YY'),
//       peso: r.peso,
//     }))
//     .reverse(); // Inverte para ter a ordem cronológica correta

//   // 2. Configurações do gráfico
//   const config = {
//     data,
//     padding: 'auto',
//     xField: 'data',
//     yField: 'peso',
//     yAxis: {
//         title: { text: 'Peso (kg)' }
//     },
//     xAxis: {
//         title: { text: 'Data da Consulta' }
//     },
//     point: {
//       size: 5,
//       shape: 'diamond',
//     },
//     tooltip: {
//       formatter: (datum) => ({ name: 'Peso', value: `${datum.peso} kg` }),
//     },
//   };

//   return <Line {...config} />;
// };

// export default GraficoEvolucaoPeso;