import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function ProtectedRoute({ children, adminOnly = false, teacherOnly = false }) {
    const { user } = useAuth()

    if (!user) return <Navigate to="/login" replace />
    if (adminOnly && user.role !== 'admin') return <Navigate to="/dashboard" replace />
    if (teacherOnly && user.role !== 'teacher' && user.role !== 'admin') return <Navigate to="/dashboard" replace />

    return children
}
