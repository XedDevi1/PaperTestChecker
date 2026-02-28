import { useState } from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Navbar() {
    const { user, logout } = useAuth()
    const navigate = useNavigate()
    const location = useLocation()
    const [menuOpen, setMenuOpen] = useState(false)

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    const isActive = (path) => location.pathname === path
    const startsWith = (prefix) => location.pathname.startsWith(prefix)

    const navLink = (to, label, activeCheck) => (
        <Link
            to={to}
            className={`navbar-link ${activeCheck ? 'active' : ''}`}
            onClick={() => setMenuOpen(false)}
        >
            {label}
        </Link>
    )

    return (
        <nav className="navbar">
            <div className="navbar-inner">
                <Link to="/dashboard" className="navbar-brand">📝 PaperTestChecker</Link>

                <button className="navbar-toggle" onClick={() => setMenuOpen(!menuOpen)}>
                    {menuOpen ? '✕' : '☰'}
                </button>

                <div className={`navbar-links ${menuOpen ? 'open' : ''}`}>
                    {navLink('/dashboard', 'Dashboard', isActive('/dashboard'))}
                    {navLink('/upload', 'Upload Test', isActive('/upload'))}
                    {navLink('/tests', 'My Tests', startsWith('/tests'))}

                    {(user?.role === 'teacher' || user?.role === 'admin') && (
                        <>
                            {navLink('/teacher/students', 'Students', startsWith('/teacher/students'))}
                            {navLink('/teacher/tests', 'Tests', startsWith('/teacher/tests'))}
                            {navLink('/teacher/results', 'Results', startsWith('/teacher/results'))}
                        </>
                    )}

                    {user?.role === 'admin' &&
                        navLink('/admin/users', 'Admin', isActive('/admin/users'))
                    }
                </div>

                <div className="navbar-right">
                    <span className="navbar-user">
                        <span className="user-name">{user?.name}</span>
                        <span className="badge badge-info" style={{ marginLeft: 8 }}>{user?.role}</span>
                    </span>
                    <button onClick={handleLogout} className="btn btn-secondary" style={{ padding: '6px 14px', fontSize: 13 }}>
                        Logout
                    </button>
                </div>
            </div>
        </nav>
    )
}
