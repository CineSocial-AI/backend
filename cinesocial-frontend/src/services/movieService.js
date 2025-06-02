// cinesocial-frontend/src/services/movieService.js
import axios from 'axios';

const API_URL = '/api/Movies'; // Using proxy

export const getMovies = async (page = 1, pageSize = 20, search = null, genreIds = null, sortBy = null) => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) {
        params.append('search', search);
    }
    if (genreIds && genreIds.length > 0) {
        genreIds.forEach(id => params.append('genreIds', id.toString()));
    }
    if (sortBy) {
        params.append('sortBy', sortBy);
    }
    return axios.get(`${API_URL}?${params.toString()}`);
};

export const getMovieById = async (id) => {
    return axios.get(`${API_URL}/${id}`);
};

// Add other movie-related API functions here later as needed
