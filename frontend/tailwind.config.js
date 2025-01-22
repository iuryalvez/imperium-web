/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        green: {
          DEFAULT: '#21BA45', // Verde de realce
          //DEFAULT: '#4CAF50', // Verde secundario
          weak: '#D9FFEC',
          light: '#278F54',  // Verde claro
          dark: '#064420',   // Verde escuro
        },
        white: {
          DEFAULT: '#FFFFFF',
        },
        gray: {
          light: '#F7F7F7',
          dark: '#9E9E9E',
        },
        'background-custom': '#278F54',
      },
    },
  },
  plugins: [],
};