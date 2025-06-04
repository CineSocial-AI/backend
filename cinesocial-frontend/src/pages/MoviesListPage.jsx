import React, { useState, useEffect } from 'react';
import { getMovies } from '../services/movieService';
import { Link } from 'react-router-dom';

const MoviesListPage = () => {
    const [movies, setMovies] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    // TODO: Add state for pagination, search, filters for future enhancements

    useEffect(() => {
        const fetchMovies = async () => {
            try {
                setIsLoading(true);
                setError(null);
                const response = await getMovies(1, 20); 
                if (response.data && response.data.isSuccess && response.data.value) {
                    setMovies(response.data.value.items || []);
                } else {
                    setError(response.data.errorMessage || 'Failed to fetch movies');
                }
            } catch (err) {
                setError(err.response?.data?.errorMessage || err.message || 'An error occurred');
                console.error("Fetch movies error:", err);
            } finally {
                setIsLoading(false);
            }
        };
        fetchMovies();
    }, []);

    if (isLoading) return <p className="loading-text">Loading movies...</p>;
    if (error) return <p className="error-text">Error: {error}</p>;
    // Note: The "No movies found" case could also use a specific class like "no-data-text"
    if (movies.length === 0) return <p className="no-data-text">No movies found.</p>;

    return (
        <div className="container"> {/* Optional: use a container for centered content */}
            <h1>Movies</h1>
            <div className="movies-grid">
                {movies.map(movie => (
                    <Link key={movie.id} to={`/movies/${movie.id}`} className="movie-card">
                        <img 
                            src={movie.posterUrl || `https://via.placeholder.com/200x300?text=${encodeURIComponent(movie.title)}`} 
                            alt={movie.title} 
                            // Removed inline style for img, covered by .movie-card img
                        />
                        <div className="movie-card-content">
                            <h3>{movie.title}</h3>
                            {/* Display other summary details if needed, e.g., movie.year */}
                        </div>
                    </Link>
                ))}
            </div>
            {/* TODO: Add pagination controls here */}
        </div>
    );
};

export default MoviesListPage;
