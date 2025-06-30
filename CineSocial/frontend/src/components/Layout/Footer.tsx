import React from 'react';
import { Link } from 'react-router-dom';
import { FilmIcon, HeartIcon } from '@heroicons/react/24/outline';

const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white border-t border-secondary-200">
      <div className="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Brand */}
          <div className="col-span-1 md:col-span-2">
            <div className="flex items-center space-x-2 mb-4">
              <FilmIcon className="h-8 w-8 text-primary-600" />
              <span className="text-xl font-bold text-secondary-900">CineSocial</span>
            </div>
            <p className="text-secondary-600 text-sm max-w-md">
              Your ultimate destination for movie discovery, reviews, and social interaction. 
              Connect with fellow movie enthusiasts and discover your next favorite film.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="text-sm font-semibold text-secondary-900 uppercase tracking-wider mb-4">
              Quick Links
            </h3>
            <ul className="space-y-2">
              <li>
                <Link to="/movies" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Browse Movies
                </Link>
              </li>
              <li>
                <Link to="/search" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Search
                </Link>
              </li>
              <li>
                <Link to="/movies/popular" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Popular Movies
                </Link>
              </li>
              <li>
                <Link to="/movies/top-rated" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Top Rated
                </Link>
              </li>
            </ul>
          </div>

          {/* Support */}
          <div>
            <h3 className="text-sm font-semibold text-secondary-900 uppercase tracking-wider mb-4">
              Support
            </h3>
            <ul className="space-y-2">
              <li>
                <Link to="/help" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Help Center
                </Link>
              </li>
              <li>
                <Link to="/contact" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Contact Us
                </Link>
              </li>
              <li>
                <Link to="/privacy" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Privacy Policy
                </Link>
              </li>
              <li>
                <Link to="/terms" className="text-secondary-600 hover:text-primary-600 text-sm">
                  Terms of Service
                </Link>
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-8 pt-8 border-t border-secondary-200">
          <div className="flex flex-col md:flex-row justify-between items-center">
            <p className="text-secondary-500 text-sm">
              © {currentYear} CineSocial. All rights reserved.
            </p>
            <div className="flex items-center space-x-1 mt-2 md:mt-0">
              <span className="text-secondary-500 text-sm">Made with</span>
              <HeartIcon className="h-4 w-4 text-red-500" />
              <span className="text-secondary-500 text-sm">for movie lovers</span>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;