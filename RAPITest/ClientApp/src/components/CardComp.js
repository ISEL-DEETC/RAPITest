import React, { Component } from 'react'
import {  Card } from 'react-bootstrap'

export default class AccordionComp extends Component {
    render() {
        let title = this.props.title
        let body = this.props.body
        let icon = this.props.icon

        if (title === 'Recently Completed Tests') {
            return (
                <Card border="info" style={{ minHeight: "313px" }}>
                    <Card.Header>
                        <h3><img style={{ marginRight: "15px" }} width="50" height="50" src={icon} alt="Logo" />{title}</h3>
                    </Card.Header>
                    <Card.Body>
                        {body}
                    </Card.Body>
                </Card>
            )
        }
        return (
            <Card border="info">
                <Card.Header>
                    <h3><img style={{ marginRight: "15px" }} width="50" height="50" src={icon} alt="Logo" />{title}</h3>
                </Card.Header>
                <Card.Body>
                    <Card.Text>{body}</Card.Text>
                </Card.Body>
            </Card>
        )
    }
}