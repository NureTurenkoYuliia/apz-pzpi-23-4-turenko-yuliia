/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#0B2545',
          light: '#163a6b',
          dark: '#071a32',
        },
        secondary: {
          DEFAULT: '#8DA9C4',
          light: '#b3c8dc',
          dark: '#6a8ba8',
        },
        background: {
          DEFAULT: '#EEF4ED',
          card: '#ffffff',
          muted: '#dce8db',
        },
        accent: {
          DEFAULT: '#96705B',
          light: '#b08878',
          dark: '#7a5a48',
        },
      },
      fontFamily: {
        display: ['"DM Serif Display"', 'Georgia', 'serif'],
        body: ['"DM Sans"', 'system-ui', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'monospace'],
      },
      boxShadow: {
        card: '0 1px 3px rgba(11,37,69,0.08), 0 4px 16px rgba(11,37,69,0.06)',
        'card-hover': '0 4px 12px rgba(11,37,69,0.14), 0 8px 32px rgba(11,37,69,0.10)',
        modal: '0 8px 40px rgba(11,37,69,0.22)',
      },
      borderRadius: {
        xl: '1rem',
        '2xl': '1.25rem',
      },
    },
  },
  plugins: [],
}
