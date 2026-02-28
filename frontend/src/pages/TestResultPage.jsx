import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TestResultPage() {
    const { id } = useParams()
    const [result, setResult] = useState(null)
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get(`/student-tests/attempts/${id}`).then(async (res) => {
            if (res.ok) setResult(await res.json())
            setLoading(false)
        })
    }, [id])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>
    if (!result) return <div className="alert alert-error">Result not found</div>

    const pct = Math.round((result.score / result.maxScore) * 100)

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/tests" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to My Tests</Link>
            </div>

            <div className="page-header">
                <h1>Test Result: {result.testTitle}</h1>
                <p>Completed {new Date(result.completedAt).toLocaleString()}</p>
            </div>

            <div className="card" style={{ textAlign: 'center', marginBottom: 24 }}>
                <div className="score-display" style={{ color: pct >= 50 ? 'var(--success)' : 'var(--error)' }}>
                    {result.score} / {result.maxScore}
                </div>
                <div className="score-label">{pct}% correct</div>
            </div>

            {result.answers.map((a) => (
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
                            <div className="label">Your Answer</div>
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
