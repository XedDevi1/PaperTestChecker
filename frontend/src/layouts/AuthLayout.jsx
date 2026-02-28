import { Outlet, Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function AuthLayout() {
    const { user } = useAuth()

    if (user) return <Navigate to="/dashboard" replace />

    return (
        <div className="auth-wrapper">
            <div className="auth-card card">
                <div className="auth-brand">📝 PaperTestChecker</div>
                <Outlet />
            </div>
        </div>
    )
}
