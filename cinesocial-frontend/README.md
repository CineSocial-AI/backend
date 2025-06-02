# CineSocial Frontend

This is the React-based frontend for the CineSocial application, built using Vite. It provides users with features to browse movies, manage watchlists, interact with social groups, and more (features to be expanded).

## Prerequisites

*   **Node.js:** Version 20.0.0 or higher is recommended (due to dependencies like `react-router-dom`). You can download it from [nodejs.org](https://nodejs.org/).
*   **npm:** Version 8.x or higher (usually comes with Node.js). You can also use `yarn` if preferred, though these instructions assume `npm`.

## Getting Started

### 1. Clone the Repository (if you haven't already)

```bash
# If you are in the root of the main project:
cd cinesocial-frontend
# Or clone the specific frontend if it were a separate repository:
# git clone <repository-url>
# cd cinesocial-frontend
```

### 2. Install Dependencies

Navigate to the `cinesocial-frontend` directory and run the following command to install the necessary project dependencies:

```bash
npm install
```

### 3. Configure Environment (if needed)

Currently, the application connects to a backend API expected to be running at `http://localhost:5108/api`. This is configured via a proxy in `vite.config.js`. Ensure your backend is running at this address or update the proxy configuration accordingly.

### 4. Running the Development Server

To start the frontend application in development mode with hot reloading, run:

```bash
npm run dev
```

This will typically start the server on `http://localhost:5173` (Vite's default). Check your terminal output for the exact URL. The application will automatically reload if you change any of the source files.

### 5. Building for Production

To create an optimized static build of the application for production, run:

```bash
npm run build
```

The production-ready files will be located in the `dist` directory. These files can then be deployed to any static file hosting service.

## Project Structure Overview

A brief overview of the key directories:

*   `public/`: Contains static assets that are directly copied to the build output (e.g., `vite.svg`).
*   `src/`: Contains all the React application source code.
    *   `assets/`: For static assets like images (e.g., `react.svg`) that are imported into components.
    *   `components/`: Reusable UI components (e.g., Navbar, LogoutButton, ProtectedRoute).
    *   `contexts/`: React context providers (e.g., `AuthContext` for authentication).
    *   `pages/`: Top-level page components that correspond to different routes (e.g., `LoginPage`, `MoviesListPage`, `MovieDetailPage`).
    *   `services/`: Modules for making API calls (e.g., `authService.js`, `movieService.js`).
    *   `App.jsx`: The main application component where routing is defined.
    *   `main.jsx`: The entry point of the application, where React is initialized.
    *   `index.css`: Global styles for the application.
*   `vite.config.js`: Configuration file for Vite, including the development server proxy.
*   `package.json`: Lists project dependencies and scripts.
*   `eslint.config.js`: ESLint configuration file (if present, created by Vite).

## Available Scripts

In the `package.json`, you will find other scripts, but the primary ones are:

*   `npm run dev`: Starts the development server.
*   `npm run build`: Creates a production build.
*   `npm run lint`: (If configured by Vite, e.g., `eslint . --ext js,jsx --report-unused-disable-directives --max-warnings 0`) Lints the codebase for potential errors.
*   `npm run preview`: Serves the production build locally for preview after running `npm run build`.

```
