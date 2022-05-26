import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { Container, Row, Col, Accordion } from 'react-bootstrap'
import './UploadFile.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import ModalCompWorkflow from '../../components/ModalCompWorkflow.js'
import ModalCompTest from '../../components/ModalCompTest.js'
import ModalCompStressTest from '../../components/ModalCompStressTest.js'
import workflowIcon from '../../assets/share.png'
import authService from '../api-authorization/AuthorizeService';

export class CreateTSL extends Component {

    constructor() {

        super()

        this.state = {
            acceptTSL: null,
            acceptDIC: null,
            acceptDLL: null,
            workflows: [],
            showWorkflowModal: false,
            showTestModal: false,
            showStressTestModal: false,
            paths: [],
            servers: [],
            schemas: []
        }

        this.addWorkflow = this.addWorkflow.bind(this)
        this.addTest = this.addTest.bind(this)
        this.addStressTest = this.addStressTest.bind(this)
        this.renderWorkflows = this.renderWorkflows.bind(this)
        this.createWorkflow = this.createWorkflow.bind(this)
        this.createTest = this.createTest.bind(this)
        this.createStressTest = this.createStressTest.bind(this)
        this.disableWorkflowModal = this.disableWorkflowModal.bind(this)
        this.disableTestModal = this.disableTestModal.bind(this)
        this.disableStressTestModal = this.disableStressTestModal.bind(this)
    }

    async componentDidMount() {
        let data = new FormData();
        data.append('apiSpecification', this.props.apiSpecification);
        const token = await authService.getAccessToken();
        fetch(`SetupTest/GetSpecificationDetails`, {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
            body: data
        }).then(res => {
            res.json().then(response => {
                this.setState({ paths: response.Paths, servers: response.Servers, schemas: response.Schemas})
            })
        })
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
                                <AwesomeButton style={{ marginRight: "20px" }} size="large" type="primary" onPress={this.addTest}>Add Test</AwesomeButton>
                                <AwesomeButton size="large" type="primary" onPress={this.addStressTest}>Add Stress Test</AwesomeButton>
                            </div>
                        </Accordion.Body>
                    </Accordion.Item >
                })}
            </Accordion>
        )
    }


    addWorkflow() {
        this.setState({ showWorkflowModal: true })
    }

    addTest() {
        this.setState({ showTestModal:true })
    }

    addStressTest() {
        this.setState({ showStressTestModal:true })
    }

    createWorkflow(WorkflowID) {
        let workflow = {
            WorkflowID: WorkflowID,
            Tests: [],
            StressTest: null
        }
        this.setState({ workflows: this.state.workflows.concat([workflow]), showWorkflowModal: false })
    }

    createTest() {
        console.log("add test")
    }

    createStressTest() {
        console.log("add stress test")
    }

    disableWorkflowModal() { this.setState({ showWorkflowModal: false }) }

    disableTestModal() { this.setState({ showTestModal: false }) }

    disableStressTestModal() { this.setState({ showStressTestModal: false }) }

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
                <ModalCompTest
                    okButtonFunc={this.createTest}
                    cancelButtonFunc={this.disableTestModal}
                    visible={this.state.showTestModal}
                    servers={this.state.servers}
                    paths={this.state.paths}
                    schemas={this.state.schemas}
                />
                <ModalCompStressTest
                    okButtonFunc={this.createStressTest}
                    cancelButtonFunc={this.disableStressTestModal}
                    visible={this.state.showStressTestModal}
                    
                />
            </div>
        )
    }
}