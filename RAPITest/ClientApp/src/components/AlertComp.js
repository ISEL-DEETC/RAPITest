import React from 'react'
import { Alert } from 'react-bootstrap'



export const successMessage = (message, closeCallback) => (
    <Alert variant="success" onClose={() => closeCallback()} dismissible>
        <p>
            {message}
        </p>
    </Alert>
)

export const warningMessage = (message, closeCallback) => (
    <Alert variant="warning" onClose={() => closeCallback()} dismissible>
        <p>
            {message}
        </p>
    </Alert>
)

export const infoMessage = (message, closeCallback) => (
    <Alert variant="info" onClose={() => closeCallback()} dismissible>
        <p>
            {message}
        </p>
    </Alert>
)

export const dangerMessage = (message, closeCallback) => (
    <Alert variant="danger" onClose={() => closeCallback()} dismissible>
        <p>
            {message}
        </p>
    </Alert>
)