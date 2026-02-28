import { createContext, useContext, useState } from 'react'
import * as api from '../api/client'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
    const [user, setUser] = useState(api.getStoredUser)

    const handleLogin = async (email, password) => {
        const result = await api.login(email, password)
        if (result.data) setUser(result.data)
        return result
    }

    const handleRegister = async (name, email, password, role) => {
        const result = await api.register(name, email, password, role)
        if (result.data) setUser(result.data)
        return result
    }

    const handleLogout = () => {
        api.logout()
        setUser(null)
    }

    return (
        <AuthContext.Provider value={{ user, login: handleLogin, register: handleRegister, logout: handleLogout }}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    const ctx = useContext(AuthContext)
    if (!ctx) throw new Error('useAuth must be within AuthProvider')
    return ctx
}
