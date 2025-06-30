import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { MovieSummary } from '../../services/api';
import apiService from '../../services/api';
import Loading from '../../components/UI/Loading';
import { PlayIcon, StarIcon } from '@heroicons/react/24/solid';
import { FilmIcon } from '@heroicons/react/24/outline';

const Home: React.FC = () => {
  const { isAuthenticated, user } = useAuth();
  const [popularMovies, setPopularMovies] = useState<MovieSummary[]>([]);
  const [topRatedMovies, setTopRatedMovies] = useState<MovieSummary[]>([]);
  const [recentMovies, setRecentMovies] = useState<MovieSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchMovies = async () => {
      try {
        setIsLoading(true);
        
        // Note: These endpoints might not be fully implemented in backend yet
        // For demo purposes, we'll use the main movies endpoint
        const moviesResponse = await apiService.getMovies(1, 6);
        const movies = moviesResponse.items || [];
        
        // Simulate different categories by splitting the results
        setPopularMovies(movies.slice(0, 2));
        setTopRatedMovies(movies.slice(2, 4));
        setRecentMovies(movies.slice(4, 6));
        
      } catch (err: any) {
        console.error('Failed to fetch movies:', err);
        setError('Failed to load movies. Please try again later.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchMovies();
  }, []);

  const MovieCard: React.FC<{ movie: MovieSummary }> = ({ movie }) => (
    <Link to={`/movies/${movie.id}`} className="group block">
      <div className="bg-white rounded-lg shadow-md overflow-hidden card-hover">
        <div className="aspect-w-3 aspect-h-4 bg-secondary-200 relative">
          {movie.posterUrl ? (
            <img
              src={movie.posterUrl}
              alt={movie.title}
              className="w-full h-48 object-cover"
            />
          ) : (
            <div className="w-full h-48 flex items-center justify-center bg-secondary-100">
              <FilmIcon className="h-12 w-12 text-secondary-400" />
            </div>
          )}
          <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-20 transition-all duration-200 flex items-center justify-center">
            <PlayIcon className="h-12 w-12 text-white opacity-0 group-hover:opacity-100 transition-opacity duration-200" />
          </div>
        </div>
        <div className="p-4">
          <h3 className="text-lg font-semibold text-secondary-900 mb-2 line-clamp-2">
            {movie.title}
          </h3>
          <div className="flex items-center justify-between">
            <span className="text-sm text-secondary-600">
              {new Date(movie.releaseDate).getFullYear()}
            </span>
            {movie.rating && (
              <div className="flex items-center">
                <StarIcon className="h-4 w-4 text-yellow-400" />
                <span className="ml-1 text-sm text-secondary-600">
                  {movie.rating.toFixed(1)}
                </span>
              </div>
            )}
          </div>
        </div>
      </div>
    </Link>
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loading size="lg" text="Loading movies..." />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-secondary-50">
      {/* Hero Section */}
      <div className="bg-primary-600 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24">
          <div className="text-center">
            <h1 className="text-4xl md:text-6xl font-bold mb-6">
              Welcome to CineSocial
            </h1>
            <p className="text-xl md:text-2xl mb-8 text-primary-100 max-w-3xl mx-auto">
              Discover, review, and share your favorite movies with fellow film enthusiasts.
              {isAuthenticated && user && (
                <span className="block mt-2">Welcome back, {user.firstName}!</span>
              )}
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Link
                to="/movies"
                className="bg-white text-primary-600 px-8 py-3 rounded-lg font-semibold hover:bg-primary-50 transition-colors duration-200"
              >
                Browse Movies
              </Link>
              {!isAuthenticated && (
                <Link
                  to="/register"
                  className="border-2 border-white text-white px-8 py-3 rounded-lg font-semibold hover:bg-white hover:text-primary-600 transition-colors duration-200"
                >
                  Join Now
                </Link>
              )}
              {isAuthenticated && (
                <Link
                  to="/movies/create"
                  className="border-2 border-white text-white px-8 py-3 rounded-lg font-semibold hover:bg-white hover:text-primary-600 transition-colors duration-200"
                >
                  Add Movie
                </Link>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Content Sections */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        {error ? (
          <div className="bg-red-50 border border-red-200 rounded-md p-4 mb-8">
            <p className="text-red-600">{error}</p>
          </div>
        ) : (
          <div className="space-y-16">
            {/* Popular Movies */}
            {popularMovies.length > 0 && (
              <section>
                <div className="flex items-center justify-between mb-8">
                  <h2 className="text-3xl font-bold text-secondary-900">Popular Movies</h2>
                  <Link
                    to="/movies?sort=popular"
                    className="text-primary-600 hover:text-primary-700 font-medium"
                  >
                    View all →
                  </Link>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-6">
                  {popularMovies.map((movie) => (
                    <MovieCard key={movie.id} movie={movie} />
                  ))}
                </div>
              </section>
            )}

            {/* Top Rated Movies */}
            {topRatedMovies.length > 0 && (
              <section>
                <div className="flex items-center justify-between mb-8">
                  <h2 className="text-3xl font-bold text-secondary-900">Top Rated</h2>
                  <Link
                    to="/movies?sort=rating"
                    className="text-primary-600 hover:text-primary-700 font-medium"
                  >
                    View all →
                  </Link>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-6">
                  {topRatedMovies.map((movie) => (
                    <MovieCard key={movie.id} movie={movie} />
                  ))}
                </div>
              </section>
            )}

            {/* Recent Movies */}
            {recentMovies.length > 0 && (
              <section>
                <div className="flex items-center justify-between mb-8">
                  <h2 className="text-3xl font-bold text-secondary-900">Recently Added</h2>
                  <Link
                    to="/movies?sort=date"
                    className="text-primary-600 hover:text-primary-700 font-medium"
                  >
                    View all →
                  </Link>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-6">
                  {recentMovies.map((movie) => (
                    <MovieCard key={movie.id} movie={movie} />
                  ))}
                </div>
              </section>
            )}

            {/* Empty state */}
            {popularMovies.length === 0 && topRatedMovies.length === 0 && recentMovies.length === 0 && (
              <div className="text-center py-16">
                <FilmIcon className="h-24 w-24 text-secondary-400 mx-auto mb-6" />
                <h3 className="text-2xl font-bold text-secondary-900 mb-4">No movies available</h3>
                <p className="text-secondary-600 mb-8">
                  Be the first to add a movie to our collection!
                </p>
                {isAuthenticated && (
                  <Link
                    to="/movies/create"
                    className="bg-primary-600 text-white px-6 py-3 rounded-lg font-semibold hover:bg-primary-700 transition-colors duration-200"
                  >
                    Add First Movie
                  </Link>
                )}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;