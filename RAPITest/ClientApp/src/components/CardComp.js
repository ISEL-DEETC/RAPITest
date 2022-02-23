import React, { Component } from 'react'
import {  Card } from 'react-bootstrap'

export default class AccordionComp extends Component {
    render() {
        let title = this.props.title
        let body = this.props.body

        if (title === 'Recently Completed Tests') {
            return (
                <Card border="info" style={{ minHeight: "280px" }}>
                    <Card.Header>
                        <h3>{title}</h3>
                    </Card.Header>
                    <Card.Body>
                        <Card.Text>{body}</Card.Text>
                    </Card.Body>
                </Card>
            )
        }
        return (
            <Card border="info">
                <Card.Header>
                    <h3>{title}</h3>
                </Card.Header>
                <Card.Body>
                    <Card.Text>{body}</Card.Text>
                </Card.Body>
            </Card>
        )
    }
}