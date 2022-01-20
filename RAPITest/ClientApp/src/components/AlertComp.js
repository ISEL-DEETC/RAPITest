import React from 'react'

export const successMessage = (message, closeCallback) => (
    <div class="alert alert-success alert-dismissible ">
        {message}
        <button type="button" class="close"  onClick={() => closeCallback()}>
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
)
export const warningMessage = (message, closeCallback) => (
    <div className="alert alert-warning alert-dismissible ">
        {message}
        <button type="button" className="close" onClick={() => closeCallback()}>
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
)
export const infoMessage = (message, closeCallback) => (
    <div class="alert alert-info alert-dismissible ">
        {message}
        <button type="button" class="close" onClick={() => closeCallback()}>
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
)
export const dangerMessage = (message, closeCallback) => (
    <div class="alert alert-danger alert-dismissible ">
        {message}
        <button type="button" class="close" onClick={() => closeCallback()}>
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
)