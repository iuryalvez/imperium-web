/** @type {import('tailwindcss').Config} */
const { colors } = require('./src/theme/colors.js');

module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      // Agora o Tailwind conhece suas cores e pode gerar classes como 'bg-verde-medio'
      colors: {
        ...colors,
      },
    },
  },
  plugins: [],
};