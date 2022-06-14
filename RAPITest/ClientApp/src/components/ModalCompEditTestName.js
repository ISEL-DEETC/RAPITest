import React from 'react'
import { Modal, Form } from 'react-bootstrap'
import { AwesomeButton, AwesomeButtonProgress } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import { warningMessage } from './AlertComp'

export default class ModalCompEditTestName extends React.Component {

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

    closeWarning() {
        this.setState({ showWarning: false })
    }

    finalizeCallback() {
        let formString = document.getElementById("formBasicEmail").value
        if (this.props.currentName === formString) {
            this.setState({ showWarning: true, warningMessage: "Name cannot be the same" })
            return
        }
        if (formString.length === 0) {
            this.setState({ showWarning: true, warningMessage: "Please fill out the required form" })
            return
        }
        if (formString.length > 40) {
            this.setState({ showWarning: true, warningMessage: "Limit of 40 characters exceeded" })
            return
        }
        var format = /[ `!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?~]/;
        if (format.test(formString)) {
            this.setState({ showWarning: true, warningMessage: "Invalid characters, no special characters allowed" })
        } else {
            this.props.okButtonFunc(formString)
        }
    }

    render() {
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit API Test Name</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                        <Form>
                            <Form.Group className="mb-3" controlId="formBasicEmail">
                                <Form.Label>API Test Name</Form.Label>
                                <Form.Control defaultValue={this.props.currentName} />
                                <Form.Text className="text-muted">
                                    The name of the API you want to test.
                                </Form.Text>
                            </Form.Group>
                        </Form>
                    </Modal.Body>
                    <Modal.Footer>
                        <AwesomeButton type="primary" onPress={this.finalizeCallback}>Edit</AwesomeButton>
                        <AwesomeButton type="secondary" onPress={cancelButtonFunc}>Cancel</AwesomeButton>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}