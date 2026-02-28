import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function RegisterPage() {
    const { register } = useAuth()
    const [form, setForm] = useState({ name: '', email: '', password: '', role: 'student' })
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)

    const set = (key) => (e) => setForm(f => ({ ...f, [key]: e.target.value }))

    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')
        setLoading(true)
        const result = await register(form.name, form.email, form.password, form.role)
        setLoading(false)
        if (result.error) setError(result.error)
    }

    return (
        <>
            <h2 style={{ fontSize: 20, fontWeight: 600, marginBottom: 20 }}>Create Account</h2>

            {error && <div className="alert alert-error">{error}</div>}

            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="name">Name</label>
                    <input id="name" value={form.name} onChange={set('name')} required placeholder="Your name" />
                </div>

                <div className="form-group">
                    <label htmlFor="reg-email">Email</label>
                    <input id="reg-email" type="email" value={form.email} onChange={set('email')} required placeholder="you@example.com" />
                </div>

                <div className="form-group">
                    <label htmlFor="reg-password">Password</label>
                    <input id="reg-password" type="password" value={form.password} onChange={set('password')} required placeholder="Min 6 characters" minLength={6} />
                </div>

                <div className="form-group">
                    <label htmlFor="role">Role</label>
                    <select id="role" value={form.role} onChange={set('role')}>
                        <option value="student">Student</option>
                        <option value="teacher">Teacher</option>
                        <option value="admin">Admin</option>
                    </select>
                </div>

                <button type="submit" className="btn btn-primary" disabled={loading} style={{ width: '100%', marginTop: 4 }}>
                    {loading ? 'Creating account...' : 'Create Account'}
                </button>
            </form>

            <p style={{ textAlign: 'center', marginTop: 20, fontSize: 14, color: 'var(--text-secondary)' }}>
                Already have an account? <Link to="/login">Sign In</Link>
            </p>
        </>
    )
}
