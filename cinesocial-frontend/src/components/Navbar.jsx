import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import LogoutButton from './LogoutButton'; // Assuming LogoutButton is in the same directory

const Navbar = () => {
    const { isAuthenticated, user } = useAuth();

    // Global 'nav' tag styles from index.css will apply.
    // Classes like 'nav a', 'nav .user-info', 'nav .logout-button button' will also apply.

    return (
        <nav> {/* The <nav> tag itself is styled by index.css */}
            <div className="nav-brand"> {/* Optional: class for branding/logo link */}
                <Link to="/">CineSocial</Link>
            </div>
            <div className="nav-links"> {/* Grouping navigation links */}
                {isAuthenticated ? (
                    <>
                        {user && <span className="user-info">Welcome, {user.userName}!</span>}
                        <Link to="/">Home</Link>
                        {/* <Link to="/profile">Profile</Link> */}
                        <div className="logout-button"> {/* Wrapper div for specific button styling */}
                            <LogoutButton />
                        </div>
                    </>
                ) : (
                    <>
                        <Link to="/">Home</Link>
                        <Link to="/login">Login</Link>
                        <Link to="/register">Register</Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default Navbar;
