import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TeacherResultDetailPage() {
    const { id } = useParams()
    const [detail, setDetail] = useState(null)
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get(`/teacher/results/${id}`).then(async (res) => {
            if (res.ok) setDetail(await res.json())
            setLoading(false)
        })
    }, [id])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>
    if (!detail) return <div className="alert alert-error">Result not found</div>

    const pct = Math.round((detail.score / detail.maxScore) * 100)

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/teacher/results" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to Results</Link>
            </div>

            <div className="page-header">
                <h1>{detail.testTitle}</h1>
                <p>Student: {detail.studentName} · {new Date(detail.completedAt).toLocaleString()}</p>
            </div>

            <div className="card" style={{ textAlign: 'center', marginBottom: 24 }}>
                <div className="score-display" style={{ color: pct >= 50 ? 'var(--success)' : 'var(--error)' }}>
                    {detail.score} / {detail.maxScore}
                </div>
                <div className="score-label">{pct}% correct</div>
            </div>

            {detail.answers.map(a => (
                <div key={a.questionNumber} className={`question-card ${a.isCorrect ? 'correct' : 'incorrect'}`}>
                    <div className="question-header">
                        <h3>Question {a.questionNumber}</h3>
                        <span className={`badge ${a.isCorrect ? 'badge-success' : 'badge-error'}`}>
                            {a.isCorrect ? '✓ Correct' : '✗ Wrong'}
                        </span>
                    </div>
                    <p style={{ marginBottom: 12, fontSize: 14 }}>{a.questionText}</p>
                    <div className="question-body">
                        <div>
                            <div className="label">Student's Answer</div>
                            <div style={{ color: a.isCorrect ? 'var(--success)' : 'var(--error)' }}>{a.selectedAnswer}</div>
                        </div>
                        {!a.isCorrect && (
                            <div>
                                <div className="label">Correct Answer</div>
                                <div style={{ color: 'var(--success)' }}>{a.correctAnswer}</div>
                            </div>
                        )}
                    </div>
                </div>
            ))}
        </>
    )
}
