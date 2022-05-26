import React from 'react'
import { Modal, Form } from 'react-bootstrap'
import { AwesomeButton, AwesomeButtonProgress } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import { warningMessage } from './AlertComp'
import "react-widgets/styles.css";
import Combobox from "react-widgets/Combobox";
import KeyValue  from "./KeyValueComp";
import './ModalCompTest.css';
import addIcon from '../assets/add.png'

export default class ModalCompTest extends React.Component {

    constructor() {

        super()

        this.state = {
            name: "",
            showWarning: false,
            warningMessage: "",
            selectedPath: "",
            headers: []
        }

        this.finalizeCallback = this.finalizeCallback.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
        this.pathValue = this.pathValue.bind(this)
        this.updateHeaders = this.updateHeaders.bind(this)
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

    pathValue(selectedPaths) {
        this.setState({ selectedPath: selectedPaths })
    }

    updateHeaders(headersValue) {
        this.setState({ headers: headersValue })
        console.log(headersValue)
    }

    render() {
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal size="lg" show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add Test</Modal.Title>
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
                            <Combobox
                                data={this.props.paths}
                                filter={false}
                                onChange={value => this.pathValue(value)}
                            />
                            <KeyValue
                                hideLabels={true}
                                rows={[{
                                    keyItem: '',
                                    valueItem: ''
                                }]}
                                customAddButtonRenderer={handleAddNew => (
                                    <div className="key-value-add-new" style={{marginTop: "5px"}}>
                                        <div onClick={handleAddNew}>
                                            <img style={{ marginBottom: "5px" }} width="30" height="30" src={addIcon} alt="LogoBin" /> Add new meta data
                                        </div>
                                    </div>
                                )}
                                onChange={rows => this.updateHeaders(rows)}
                            />
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