// Use the environment variable if defined, otherwise default to local development proxy
const API_BASE = import.meta.env.VITE_API_URL || '/api';

function getToken() {
    return localStorage.getItem('token');
}

async function request(url, options = {}) {
    const token = getToken();
    const headers = { ...options.headers };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    if (!(options.body instanceof FormData)) {
        headers['Content-Type'] = 'application/json';
    }

    const response = await fetch(`${API_BASE}${url}`, { ...options, headers });

    if (response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login';
        return;
    }

    return response;
}

export async function get(url) {
    return request(url);
}

export async function post(url, body) {
    return request(url, {
        method: 'POST',
        body: JSON.stringify(body),
    });
}

export async function postFile(url, file) {
    const formData = new FormData();
    formData.append('file', file);

    return request(url, {
        method: 'POST',
        body: formData,
    });
}

export async function del(url) {
    return request(url, { method: 'DELETE' });
}

export async function put(url, body) {
    return request(url, {
        method: 'PUT',
        body: JSON.stringify(body),
    });
}

export async function login(email, password) {
    const res = await post('/auth/login', { email, password });
    if (!res.ok) return { error: 'Invalid credentials' };
    const data = await res.json();
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data));
    return { data };
}

export async function register(name, email, password, role = 'student') {
    const res = await post('/auth/register', { name, email, password, role });
    if (!res.ok) {
        const err = await res.json();
        return { error: err.errors ? Object.values(err.errors).flat().join(', ') : 'Registration failed' };
    }
    const data = await res.json();
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data));
    return { data };
}

export function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
}

export function getStoredUser() {
    const u = localStorage.getItem('user');
    return u ? JSON.parse(u) : null;
}

export function isLoggedIn() {
    return !!getToken();
}
