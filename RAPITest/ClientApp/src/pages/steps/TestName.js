import React, { Component } from 'react';
import Form from 'react-bootstrap/Form';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { warningMessage } from '../../components/AlertComp'


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
            return
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
                <Form>
                      <Form.Group className="mb-3" controlId="formBasicEmail">
                        <Form.Label>API Test Name</Form.Label>
                        <Form.Control placeholder="Enter Name" />
                        <Form.Text className="text-muted">
                          The name of the API you want to test.
                        </Form.Text>
                      </Form.Group>
    
                    <button type="button" className="btn btn-outline-primary" onClick={this.finalizeCallback}>Continue</button>
                </Form>
            </div>
        )
    }
}
