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
        let formCount = document.getElementById("formCount").value
        let formThreads = document.getElementById("formThreads").value
        let formDelay = document.getElementById("formDelay").value

        if (!Number.isInteger(formCount) || formCount <= 0) {
            this.setState({ showWarning: true, warningMessage: "Count must be integer and greater then 0" })
            return;
        }

        if (!Number.isInteger(formThreads) || formThreads <= 0) {
            this.setState({ showWarning: true, warningMessage: "Threads must be integer and greater then 0" })
            return
        }

        if (!Number.isInteger(formDelay) || formDelay < 0) {
            this.setState({ showWarning: true, warningMessage: "Delay must be integer and atleast 0" })
            return
        }

        this.props.okButtonFunc(formCount, formThreads, formDelay)
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
                            <Form.Group className="mb-3" controlId="formCount">
                                <Form.Label>Count</Form.Label>
                                <Form.Control placeholder="1 or Greater" />
                                <Form.Text className="text-muted">
                                    The number of times this workflow will be executed
                                </Form.Text>
                            </Form.Group>
                        </Form>
                        <Form>
                            <Form.Group className="mb-3" controlId="formThreads">
                                <Form.Label>Threads</Form.Label>
                                <Form.Control placeholder="1 or Greater" />
                                <Form.Text className="text-muted">
                                    The number of threads in which the count will be divided
                                </Form.Text>
                            </Form.Group>
                        </Form>
                        <Form>
                            <Form.Group className="mb-3" controlId="formDelay">
                                <Form.Label>Delay (ms)</Form.Label>
                                <Form.Control placeholder="0 or Greater" />
                                <Form.Text className="text-muted">
                                    The delay, in milliseconds, between each workflow execution
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