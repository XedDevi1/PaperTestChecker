import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TeacherResultsPage() {
    const [attempts, setAttempts] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get('/teacher/results').then(async (res) => {
            if (res.ok) setAttempts(await res.json())
            setLoading(false)
        })
    }, [])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>Student Results</h1>
                <p>Test attempts by students on your generated tests</p>
            </div>

            {attempts.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">📊</div>
                    <h3>No results yet</h3>
                    <p>Students haven't taken any of your tests yet</p>
                </div>
            ) : (
                <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
                    <div className="table-wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>Test</th>
                                    <th>Student</th>
                                    <th>Score</th>
                                    <th>Date</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {attempts.map(a => {
                                    const pct = Math.round((a.score / a.maxScore) * 100)
                                    return (
                                        <tr key={a.attemptId}>
                                            <td style={{ fontWeight: 500 }}>{a.testTitle}</td>
                                            <td>{a.studentName}</td>
                                            <td>
                                                <span className={`badge ${pct >= 50 ? 'badge-success' : 'badge-error'}`}>
                                                    {a.score}/{a.maxScore} ({pct}%)
                                                </span>
                                            </td>
                                            <td style={{ color: 'var(--text-secondary)' }}>
                                                {new Date(a.completedAt).toLocaleDateString()}
                                            </td>
                                            <td>
                                                <Link to={`/teacher/results/${a.attemptId}`} className="btn btn-secondary" style={{ padding: '4px 12px', fontSize: 13 }}>
                                                    Details
                                                </Link>
                                            </td>
                                        </tr>
                                    )
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </>
    )
}
