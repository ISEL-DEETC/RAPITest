import React, { Component } from 'react';
import Form from 'react-bootstrap/Form';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { warningMessage } from '../../components/AlertComp'
import {Row, Col, Figure } from 'react-bootstrap'
import thinkingIcon from '../../assets/thinking.png'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";

export class TestName extends Component {

    constructor() {

        super()

        this.state = {
            name: "",
            showWarning: false,
            warningMessage: ""
        }

        this.finalizeCallback = this.finalizeCallback.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
    }

    finalizeCallback() {
        let formString = document.getElementById("formBasicEmail").value
        if (formString.length === 0) {
            this.setState({ showWarning: true, warningMessage: "Please fill out the required form" })
            return <div></div>
        }
        if (formString.length > 40) {
            this.setState({ showWarning: true, warningMessage: "Limit of 40 characters exceeded" })
            return <div></div>
        }
        var format = /[ `!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?~]/;
        if (format.test(formString)) {
            this.setState({ showWarning: true, warningMessage: "Invalid characters, no special characters allowed" })
        } else {
            this.props.handlerName(formString)
        }
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                <Row>
                    <Col sm={4}>
                        <Form>
                              <Form.Group className="mb-3" controlId="formBasicEmail">
                                <Form.Label>API Test Name</Form.Label>
                                <Form.Control placeholder="Enter Name" />
                                <Form.Text className="text-muted">
                                  The name of the API you want to test.
                                </Form.Text>
                              </Form.Group>
                        </Form>
                        <div style={{ textAlign: "center" }}>
                            <AwesomeButton type="primary" onPress={() => this.finalizeCallback()}>Continue</AwesomeButton>
                        </div>
                    </Col>
                    <Col sm={8}>
                        <Figure style={{padding: "100px 0px 0px 250px"}}>
                            <Figure.Image
                                width={400}
                                height={400}
                                alt="400x400"
                                src={thinkingIcon}
                            />
                            <Figure.Caption>
                            </Figure.Caption>
                        </Figure>
                    </Col>
                </Row>
            </div>
        )
    }
}