import { Routes, Route, Navigate } from 'react-router-dom'
import AuthLayout from './layouts/AuthLayout'
import MainLayout from './layouts/MainLayout'
import AdminLayout from './layouts/AdminLayout'
import TeacherLayout from './layouts/TeacherLayout'
import ProtectedRoute from './components/ProtectedRoute'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import DashboardPage from './pages/DashboardPage'
import UploadPage from './pages/UploadPage'
import SubmissionDetailPage from './pages/SubmissionDetailPage'
import AdminUsersPage from './pages/AdminUsersPage'
import TeacherStudentsPage from './pages/TeacherStudentsPage'
import TeacherStudentQuestionsPage from './pages/TeacherStudentQuestionsPage'
import TeacherTestsPage from './pages/TeacherTestsPage'
import TeacherTestDetailPage from './pages/TeacherTestDetailPage'
import TeacherResultsPage from './pages/TeacherResultsPage'
import TeacherResultDetailPage from './pages/TeacherResultDetailPage'
import StudentTestsPage from './pages/StudentTestsPage'
import TakeTestPage from './pages/TakeTestPage'
import TestResultPage from './pages/TestResultPage'

export default function App() {
    return (
        <Routes>
            {/* Public — Auth Layout */}
            <Route element={<AuthLayout />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
            </Route>

            {/* Protected — Main Layout */}
            <Route element={<ProtectedRoute><MainLayout /></ProtectedRoute>}>
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/upload" element={<UploadPage />} />
                <Route path="/submissions/:id" element={<SubmissionDetailPage />} />
                <Route path="/tests" element={<StudentTestsPage />} />
                <Route path="/tests/:id/take" element={<TakeTestPage />} />
                <Route path="/tests/results/:id" element={<TestResultPage />} />
            </Route>

            {/* Protected — Teacher Layout */}
            <Route element={<ProtectedRoute teacherOnly><TeacherLayout /></ProtectedRoute>}>
                <Route path="/teacher/students" element={<TeacherStudentsPage />} />
                <Route path="/teacher/students/:id" element={<TeacherStudentQuestionsPage />} />
                <Route path="/teacher/tests" element={<TeacherTestsPage />} />
                <Route path="/teacher/tests/:id" element={<TeacherTestDetailPage />} />
                <Route path="/teacher/results" element={<TeacherResultsPage />} />
                <Route path="/teacher/results/:id" element={<TeacherResultDetailPage />} />
            </Route>

            {/* Protected — Admin Layout */}
            <Route element={<ProtectedRoute adminOnly><AdminLayout /></ProtectedRoute>}>
                <Route path="/admin/users" element={<AdminUsersPage />} />
            </Route>

            {/* Default redirect */}
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
    )
}
