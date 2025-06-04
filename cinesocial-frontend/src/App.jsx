import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import MoviesListPage from './pages/MoviesListPage';
import MovieDetailPage from './pages/MovieDetailPage';
// import ProtectedRoute from './components/ProtectedRoute'; // Uncomment if you add protected routes
// import ProfilePage from './pages/ProfilePage'; // Example for a protected route

import './App.css'; // Assuming global app styles, can be removed if not used

function App() {
    return (
        <> {/* Using fragment to avoid unnecessary div */}
            <Navbar />
            <main style={{ padding: '1rem', marginTop: '60px' }}> {/* Added marginTop to prevent content overlap with a fixed/absolute Navbar, adjust if Navbar style is different */}
                <Routes>
                    <Route path="/" element={<MoviesListPage />} />
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route path="/movies/:id" element={<MovieDetailPage />} />
                    
                    {/* 
                    Example of a protected route:
                    <Route path="/profile" element={<ProtectedRoute />}>
                        <Route index element={<ProfilePage />} />
                    </Route>
                    
                    Example of a 404 Not Found Route:
                    <Route path="*" element={
                        <div style={{ textAlign: 'center', marginTop: '50px' }}>
                            <h2>404 - Page Not Found</h2>
                            <p>Sorry, the page you are looking for does not exist.</p>
                        </div>
                    } />
                    */}
                </Routes>
            </main>
        </>
    );
}

export default App;
