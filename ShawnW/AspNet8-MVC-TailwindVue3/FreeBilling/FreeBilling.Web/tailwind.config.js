/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Pages/**/*.{html,cshtml}",
        "./Areas/**/*.{html,cshtml}",
        "./wwwroot/**/*.{html,htm}",
        "../freebilling.app/src/**/*.{html,htm,js,vue}"
    ],
  theme: {
    extend: {},
  },
  plugins: [],
}

