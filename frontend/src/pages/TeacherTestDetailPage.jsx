import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TeacherTestDetailPage() {
    const { id } = useParams()
    const [test, setTest] = useState(null)
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get(`/teacher/tests/${id}`).then(async (res) => {
            if (res.ok) setTest(await res.json())
            setLoading(false)
        })
    }, [id])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>
    if (!test) return <div className="alert alert-error">Test not found</div>

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/teacher/tests" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to Tests</Link>
            </div>

            <div className="page-header">
                <h1>{test.title}</h1>
                <p>For student: {test.studentName} · {test.items.length} questions · {new Date(test.createdAt).toLocaleDateString()}</p>
            </div>

            {test.items.map((item) => (
                <div key={item.questionNumber} className="question-card" style={{ borderLeft: '3px solid var(--accent)' }}>
                    <div className="question-header">
                        <h3>Question {item.questionNumber}</h3>
                    </div>
                    <p style={{ marginBottom: 16, fontSize: 15 }}>{item.questionText}</p>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
                        {item.options.map((opt, i) => (
                            <div
                                key={i}
                                style={{
                                    padding: '10px 14px',
                                    borderRadius: 'var(--radius-sm)',
                                    fontSize: 14,
                                    background: opt === item.correctAnswer ? 'rgba(74, 222, 128, 0.1)' : 'var(--bg)',
                                    border: opt === item.correctAnswer ? '1px solid rgba(74, 222, 128, 0.3)' : '1px solid var(--border)',
                                    color: opt === item.correctAnswer ? 'var(--success)' : 'var(--text)',
                                }}
                            >
                                {String.fromCharCode(65 + i)}) {opt}
                                {opt === item.correctAnswer && ' ✓'}
                            </div>
                        ))}
                    </div>
                </div>
            ))}
        </>
    )
}
