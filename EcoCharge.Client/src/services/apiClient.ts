import axios from 'axios';

/**
 * Shared Axios instance for all API calls.
 * Base URL comes from VITE_API_BASE_URL (see .env.example).
 */
export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10_000,
});

export const POLL_INTERVAL_MS = Number(import.meta.env.VITE_POLL_INTERVAL_MS ?? 5000);
