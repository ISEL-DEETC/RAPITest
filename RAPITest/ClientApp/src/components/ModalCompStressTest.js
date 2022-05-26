import React from 'react'
import { Modal, Form } from 'react-bootstrap'
import { AwesomeButton, AwesomeButtonProgress } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import { warningMessage } from './AlertComp'

export default class ModalCompStressTest extends React.Component {

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
        let formString = document.getElementById("formWorkflowId").value
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

            let currentWorkflows = this.props.currentWorkflows

            let found = false

            currentWorkflows.forEach((item, index) => {
                if (item.WorkflowID === formString) {
                    this.setState({ showWarning: true, warningMessage: "ID already used, must be unique" })
                    found = true
                }
            })

            if (!found) {
                this.props.okButtonFunc(formString)
            }
        }
    }

    render() {
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add Stress Test</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                        <Form>
                            <Form.Group className="mb-3" controlId="formWorkflowId">
                                <Form.Label>Workflow ID</Form.Label>
                                <Form.Control placeholder="Enter ID" />
                                <Form.Text className="text-muted">
                                    The ID of the added workflow, must be unique
                                </Form.Text>
                            </Form.Group>
                        </Form>
                    </Modal.Body>
                    <Modal.Footer>
                        <AwesomeButton type="primary" onPress={this.finalizeCallback}>Add</AwesomeButton>
                        <AwesomeButton type="secondary" onPress={cancelButtonFunc}>Cancel</AwesomeButton>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}