import { Outlet, Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function AuthLayout() {
    const { user } = useAuth()

    if (user) return <Navigate to="/dashboard" replace />

    return (
        <div style={styles.wrapper}>
            <div style={styles.card}>
                <div style={styles.brand}>📝 PaperTestChecker</div>
                <Outlet />
            </div>
        </div>
    )
}

const styles = {
    wrapper: {
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: 24,
    },
    card: {
        background: 'var(--surface)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius)',
        padding: 36,
        width: '100%',
        maxWidth: 400,
    },
    brand: {
        fontSize: 20,
        fontWeight: 700,
        textAlign: 'center',
        marginBottom: 28,
    },
}
