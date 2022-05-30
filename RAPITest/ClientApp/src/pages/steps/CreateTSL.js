import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { Container, Row, Col, Accordion,Table } from 'react-bootstrap'
import './UploadFile.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import ModalCompWorkflow from '../../components/ModalCompWorkflow.js'
import ModalCompTest from '../../components/ModalCompTest.js'
import ModalCompStressTest from '../../components/ModalCompStressTest.js'
import workflowIcon from '../../assets/share.png'
import testIcon from '../../assets/test.png'
import editIcon from '../../assets/pencil.png'
import deleteIcon from '../../assets/bin.png'
import authService from '../api-authorization/AuthorizeService';
import './CreateTSL.css';

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
            currentAddTestWorkflow: "",
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
        return (
            <Accordion key={testindex} defaultActiveKey={test.TestID}>
                <Accordion.Item key={test.TestID + testindex} eventKey={test.TestID}>
                    <Accordion.Header><img style={{ marginRight: "15px" }} width="40" height="40" src={testIcon} alt="LogoTest" />{test.TestID}</Accordion.Header>
                    <Accordion.Body>
                        <Row>
                            <Col>
                                <h4>Request</h4>
                                <Table striped bordered hover>
                                    <tbody>
                                        <tr>
                                            <td>{test.Method}</td>
                                        </tr>
                                        <tr>
                                            <td>{test.Server}{test.Path}</td>
                                        </tr>
                                        {test.Header.length !== 0 &&
                                            <tr>
                                                <td>
                                                    {test.Header.map((item, index) => {
                                                        return <p key={index}>{item.keyItem + "  " + item.valueItem}</p>
                                                    })}
                                                </td>
                                            </tr>
                                        }
                                        {test.Body.data !== "" &&
                                            <tr>
                                                <td>{test.Body.data}</td>
                                            </tr>
                                        }
                                    </tbody>
                                </Table>
                                <h4>Verifications</h4>
                                <Table striped bordered hover>
                                    <tbody>
                                        <tr>
                                            <td>Code: {test.Verifications.Code}</td>
                                        </tr>
                                        {test.Verifications.Schema !== "" &&
                                            <tr>
                                                <td>Schema: {test.Verifications.Schema}</td>
                                            </tr>
                                        }
                                    </tbody>
                                </Table>
                            </Col>
                            <Col>
                                <div style={{ marginTop:"55px", textAlign:"center" }}>
                                    <AwesomeButton className="buttonEdit"  type="primary" onPress={this.addStressTest}><img  width="50" height="50" src={editIcon} alt="Logo" /></AwesomeButton>
                                </div>
                                <div style={{ marginTop: "20px",  textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={this.addStressTest}><img  width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
                                </div>
                            </Col>
                        </Row>
                    </Accordion.Body>
                </Accordion.Item >
            </Accordion>
        )
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
                            <div style={{marginTop:"20px"}}>
                                <AwesomeButton style={{ marginRight: "20px" }} size="large" type="primary" onPress={() => this.addTest(item)}>Add Test</AwesomeButton>
                                <AwesomeButton size="large" type="primary" onPress={this.addStressTest}>Add Stress Test</AwesomeButton>
                                <div style={{ textAlign: "right" }} >
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={this.addStressTest}><img width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
                                </div>
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

    addTest(workflow) {
        this.setState({ showTestModal: true, currentAddTestWorkflow: workflow })
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

    createTest(test) {
        let aux = this.state.workflows

        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.currentAddTestWorkflow.WorkflowID) {
                item.Tests.push(test)
            }
        })

        this.setState({ workflows:aux,  showTestModal: false })
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
                <div style={{marginTop:"30px"}}>
                    <AwesomeButton className="buttonAdd" type="primary" onPress={this.addWorkflow}><img style={{ marginRight: "15px" }} width="50" height="50" src={workflowIcon} alt="Logo" />Add Workflow</AwesomeButton>
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
                    currentWorkflows={this.state.workflows}
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