import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './assets/global.css'; // Import our global CSS
import CssBaseline from '@mui/material/CssBaseline';
// Optional: ThemeProvider and createTheme if a custom theme is needed later
// import { ThemeProvider, createTheme } from '@mui/material/styles';

// Optional: Define a custom theme
// const theme = createTheme({
//   palette: {
//     primary: {
//       main: '#1976d2', // Example primary color
//     },
//     secondary: {
//       main: '#dc004e', // Example secondary color
//     },
//   },
// });

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    {/* <ThemeProvider theme={theme}> // Uncomment if you want to use a custom theme */}
      <CssBaseline />
      <App />
    {/* </ThemeProvider> */}
  </React.StrictMode>,
);
