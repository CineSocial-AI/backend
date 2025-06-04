import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getMovieById } from '../services/movieService';

const MovieDetailPage = () => {
    const { id } = useParams();
    const [movie, setMovie] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchMovieDetail = async () => {
            if (!id) {
                setIsLoading(false);
                setError("No movie ID provided.");
                return;
            }
            try {
                setIsLoading(true);
                setError(null);
                const response = await getMovieById(id);
                if (response.data && response.data.isSuccess && response.data.value) {
                    setMovie(response.data.value);
                } else {
                    setError(response.data.errorMessage || `Movie with ID ${id} not found.`);
                }
            } catch (err) {
                setError(err.response?.data?.errorMessage || err.message || 'An error occurred while fetching movie details');
                console.error(`Fetch movie detail error for ID ${id}:`, err);
            } finally {
                setIsLoading(false);
            }
        };
        fetchMovieDetail();
    }, [id]);

    if (isLoading) return <p className="loading-text">Loading movie details...</p>;
    if (error) return <p className="error-text">Error: {error}</p>;
    if (!movie) return <p className="no-data-text">Movie not found.</p>;

    return (
        <div className="movie-detail-layout container"> {/* Added container for consistency */}
            <div className="movie-detail-poster-container">
                <img 
                    src={movie.posterUrl || `https://via.placeholder.com/300x450?text=${encodeURIComponent(movie.title)}`} 
                    alt={movie.title} 
                    className="movie-detail-poster"
                />
            </div>
            <div className="movie-detail-info">
                <h1>{movie.title}</h1>
                <p><strong>Release Date:</strong> {movie.releaseDate ? new Date(movie.releaseDate).toLocaleDateString() : 'N/A'}</p>
                
                <section> {/* Using section for semantic grouping */}
                    <h3>Overview</h3>
                    <p>{movie.overview || 'No overview available.'}</p>
                </section>

                {movie.genres && movie.genres.length > 0 && (
                    <section>
                        <h3>Genres:</h3>
                        <ul className="genres-list">
                            {movie.genres.map(genre => (
                                <li key={genre.id || genre.name}>{genre.name}</li>
                            ))}
                        </ul>
                    </section>
                )}
                
                {/* 
                Placeholder for other details like Cast, Director, Rating, Runtime etc.
                Example:
                {movie.cast && movie.cast.length > 0 && (
                    <section>
                        <h3>Cast:</h3>
                        <ul className="cast-list"> // Assuming a .cast-list class
                            {movie.cast.map(actor => (
                                <li key={actor.id || actor.name}>{actor.name} (as {actor.character})</li>
                            ))}
                        </ul>
                    </section>
                )}
                <p><strong>Rating:</strong> {movie.rating ? movie.rating : 'N/A'}</p>
                <p><strong>Runtime:</strong> {movie.runtime ? `${movie.runtime} minutes` : 'N/A'}</p>
                */}
            </div>
        </div>
    );
};

export default MovieDetailPage;
