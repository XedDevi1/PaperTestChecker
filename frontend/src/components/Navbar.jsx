import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Navbar() {
    const { user, logout } = useAuth()
    const navigate = useNavigate()
    const location = useLocation()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    const isActive = (path) => location.pathname === path

    return (
        <nav style={styles.nav}>
            <div style={styles.inner}>
                <Link to="/dashboard" style={styles.brand}>
                    📝 PaperTestChecker
                </Link>

                <div style={styles.links}>
                    <Link
                        to="/dashboard"
                        style={{ ...styles.link, ...(isActive('/dashboard') ? styles.active : {}) }}
                    >
                        Dashboard
                    </Link>
                    <Link
                        to="/upload"
                        style={{ ...styles.link, ...(isActive('/upload') ? styles.active : {}) }}
                    >
                        Upload Test
                    </Link>
                    {user?.role === 'admin' && (
                        <Link
                            to="/admin/users"
                            style={{ ...styles.link, ...(isActive('/admin/users') ? styles.active : {}) }}
                        >
                            Admin
                        </Link>
                    )}
                </div>

                <div style={styles.right}>
                    <span style={styles.userInfo}>
                        {user?.name}
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

const styles = {
    nav: {
        background: 'var(--surface)',
        borderBottom: '1px solid var(--border)',
        position: 'sticky',
        top: 0,
        zIndex: 100,
    },
    inner: {
        maxWidth: 1000,
        margin: '0 auto',
        padding: '0 24px',
        height: 56,
        display: 'flex',
        alignItems: 'center',
        gap: 32,
    },
    brand: {
        fontSize: 16,
        fontWeight: 700,
        color: 'var(--text)',
        textDecoration: 'none',
    },
    links: {
        display: 'flex',
        gap: 4,
        flex: 1,
    },
    link: {
        padding: '6px 12px',
        borderRadius: 6,
        fontSize: 14,
        color: 'var(--text-secondary)',
        textDecoration: 'none',
        transition: 'all 0.15s',
    },
    active: {
        color: 'var(--accent)',
        background: 'rgba(79, 142, 247, 0.1)',
    },
    right: {
        display: 'flex',
        alignItems: 'center',
        gap: 12,
    },
    userInfo: {
        fontSize: 13,
        color: 'var(--text-secondary)',
        display: 'flex',
        alignItems: 'center',
    },
}
