import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { Container, Row, Col, Accordion } from 'react-bootstrap'
import './UploadFile.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import ModalCompWorkflow from '../../components/ModalCompWorkflow.js'
import workflowIcon from '../../assets/share.png'

export class CreateTSL extends Component {

    constructor() {

        super()

        this.state = {
            acceptTSL: null,
            acceptDIC: null,
            acceptDLL: null,
            workflows: [],
            showWorkflowModal: false,
        }

        this.addWorkflow = this.addWorkflow.bind(this)
        this.renderWorkflows = this.renderWorkflows.bind(this)
        this.createWorkflow = this.createWorkflow.bind(this)
        this.disableWorkflowModal = this.disableWorkflowModal.bind(this)
    }

    renderStressTest(stress) {

    }

    renderTest(test, testindex) {
        /*return (
            <Accordion.Item key={test.TestID + testindex} eventKey={test.TestID}>
                <Accordion.Header>{test.TestID}</Accordion.Header>
                <Accordion.Body>
                    <ListGroup as="ol">
                        {test.TestResults.map((testresult, testresultindex) => {
                            return <ListGroup.Item key={test.TestID + testresult.TestName + testresult.Success + testresult.Description} as="li" className="d-flex justify-content-between align-items-start" >
                                <div className="ms-2 me-auto">
                                    <div className="fw-bold" style={{ textAlign: "left" }}>{testresult.TestName}</div>
                                    <div style={{ textAlign: "left" }}>  {testresult.Description}</div>
                                </div>
                                {this.printBadge(testresult)}
                            </ListGroup.Item>
                        })}
                    </ListGroup>
                </Accordion.Body>
            </Accordion.Item >
        )*/
    }

    renderWorkflows() {
        return (
            <Accordion defaultActiveKey="0">
                {this.state.workflows.map((item, index) => {
                    return <Accordion.Item key={item.WorkflowID} eventKey={item.WorkflowID}>
                        <Accordion.Header><img style={{ marginRight: "15px" }} width="50" height="50" src={workflowIcon} alt="Logo" />{item.WorkflowID}</Accordion.Header>
                        <Accordion.Body>
                            {this.renderStressTest(item.StressTest)}
                            {item.Tests.map((test, testindex) => {
                                return this.renderTest(test, testindex)
                            })}
                            <div>
                                <AwesomeButton style={{ marginRight:"20px" }} size="large" type="primary" onPress={this.addWorkflow}>Add Test</AwesomeButton>
                                <AwesomeButton size="large" type="primary" onPress={this.addWorkflow}>Add Stress Test</AwesomeButton>
                            </div>
                        </Accordion.Body>
                    </Accordion.Item >
                })}
            </Accordion>
        )
    }


    addWorkflow() {
        this.setState({ showWorkflowModal:true })
    }

    createWorkflow(WorkflowID) {
        let workflow = {
            WorkflowID: WorkflowID,
            Tests: [],
            StressTest: null
        }
        this.setState({ workflows: this.state.workflows.concat([workflow]), showWorkflowModal: false })
    }

    disableWorkflowModal() { this.setState({ showWorkflowModal: false }) }

    render() {
        return (
            <div>
                {this.renderWorkflows()}
                <div style={{paddingTop:"30px"}}>
                    <AwesomeButton size="large" type="primary" onPress={this.addWorkflow}>Add Workflow</AwesomeButton>
                </div>
                <ModalCompWorkflow
                    okButtonFunc={this.createWorkflow}
                    cancelButtonFunc={this.disableWorkflowModal}
                    visible={this.state.showWorkflowModal}
                    currentWorkflows={this.state.workflows}
                />
            </div>
        )
    }
}