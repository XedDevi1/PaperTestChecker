import { useState, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { postFile } from '../api/client'

export default function UploadPage() {
    const [file, setFile] = useState(null)
    const [preview, setPreview] = useState(null)
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState('')
    const [result, setResult] = useState(null)
    const [dragover, setDragover] = useState(false)
    const inputRef = useRef()
    const navigate = useNavigate()

    const handleFile = (f) => {
        if (!f) return
        if (!['image/jpeg', 'image/png', 'image/webp'].includes(f.type)) {
            setError('Only JPEG, PNG, and WebP images are supported')
            return
        }
        setFile(f)
        setPreview(URL.createObjectURL(f))
        setError('')
        setResult(null)
    }

    const handleDrop = (e) => {
        e.preventDefault()
        setDragover(false)
        handleFile(e.dataTransfer.files[0])
    }

    const handleSubmit = async () => {
        if (!file) return
        setError('')
        setLoading(true)
        try {
            const res = await postFile('/submissions/upload', file)
            if (!res.ok) {
                const data = await res.json()
                setError(data.error || 'Upload failed')
            } else {
                const data = await res.json()
                setResult(data)
            }
        } catch {
            setError('Network error. Please try again.')
        }
        setLoading(false)
    }

    const reset = () => {
        setFile(null)
        setPreview(null)
        setResult(null)
        setError('')
    }

    if (result) {
        return (
            <>
                <div className="page-header">
                    <h1>Analysis Results</h1>
                </div>

                <div className="card" style={{ textAlign: 'center', marginBottom: 24 }}>
                    <div className="score-display" style={{ color: result.totalScore >= result.maxScore / 2 ? 'var(--success)' : 'var(--error)' }}>
                        {result.totalScore} / {result.maxScore}
                    </div>
                    <div className="score-label">Total Score</div>
                </div>

                {result.questions.map((q) => (
                    <div key={q.questionNumber} className={`question-card ${q.isCorrect ? 'correct' : 'incorrect'}`}>
                        <div className="question-header">
                            <h3>Question {q.questionNumber}</h3>
                            <span className={`badge ${q.isCorrect ? 'badge-success' : 'badge-error'}`}>
                                {q.isCorrect ? '✓ Correct' : '✗ Wrong'}
                            </span>
                        </div>

                        <p style={{ marginBottom: 12, fontSize: 14 }}>{q.questionText}</p>

                        <div className="question-body">
                            <div>
                                <div className="label">Student's Answer</div>
                                <div>{q.studentAnswer}</div>
                            </div>
                            <div>
                                <div className="label">Correct Answer</div>
                                <div>{q.correctAnswer}</div>
                            </div>
                        </div>

                        {!q.isCorrect && q.feedback && (
                            <div className="feedback-section">
                                <strong>Feedback:</strong> {q.feedback}
                                {q.recommendedReadings?.length > 0 && (
                                    <ul className="readings-list">
                                        {q.recommendedReadings.map((r, i) => <li key={i}>{r}</li>)}
                                    </ul>
                                )}
                            </div>
                        )}
                    </div>
                ))}

                <div style={{ display: 'flex', gap: 12, marginTop: 24 }}>
                    <button onClick={reset} className="btn btn-secondary">Upload Another</button>
                    <button onClick={() => navigate('/dashboard')} className="btn btn-primary">Go to Dashboard</button>
                </div>
            </>
        )
    }

    return (
        <>
            <div className="page-header">
                <h1>Upload Test Photo</h1>
                <p>Take a photo of your marked paper test and upload it for AI analysis</p>
            </div>

            {error && <div className="alert alert-error">{error}</div>}

            <div
                className={`upload-area ${dragover ? 'dragover' : ''}`}
                onClick={() => inputRef.current?.click()}
                onDragOver={(e) => { e.preventDefault(); setDragover(true) }}
                onDragLeave={() => setDragover(false)}
                onDrop={handleDrop}
            >
                {preview ? (
                    <img src={preview} alt="Preview" style={{ maxHeight: 300, maxWidth: '100%', borderRadius: 8 }} />
                ) : (
                    <>
                        <div className="icon">📸</div>
                        <p>Click or drag & drop your test photo here</p>
                        <div className="formats">JPEG, PNG, WebP — max 10MB</div>
                    </>
                )}
                <input
                    ref={inputRef}
                    type="file"
                    accept="image/jpeg,image/png,image/webp"
                    style={{ display: 'none' }}
                    onChange={(e) => handleFile(e.target.files[0])}
                />
            </div>

            {file && (
                <div style={{ display: 'flex', gap: 12, marginTop: 20, justifyContent: 'center' }}>
                    <button onClick={reset} className="btn btn-secondary">Clear</button>
                    <button onClick={handleSubmit} className="btn btn-primary" disabled={loading}>
                        {loading ? (
                            <><div className="spinner" style={{ width: 16, height: 16, borderWidth: 2 }}></div> Analyzing...</>
                        ) : (
                            '🔍 Analyze Test'
                        )}
                    </button>
                </div>
            )}
        </>
    )
}
