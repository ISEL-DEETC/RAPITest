import React, { Component } from 'react'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';
import {  Accordion, ListGroup } from 'react-bootstrap'

export default class AccordionItemListGroup extends Component {

    render() {
        let title = this.props.title
        let eventKey = this.props.eventKey
        let listItems = this.props.listItems
        let printFunction = this.props.printFunction

        return (
            <Accordion.Item eventKey={eventKey}>
                <Accordion.Header>{title}</Accordion.Header>
                <Accordion.Body>
                    <ListGroup as="ol">
                        {listItems.map((item, index) => {
                            return printFunction(item)
                        })}
                    </ListGroup>
                </Accordion.Body>
            </Accordion.Item>
        )
    }


}