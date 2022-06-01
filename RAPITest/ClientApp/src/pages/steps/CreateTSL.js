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
import ModalComp from '../../components/ModalComp.js'
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
            showRemoveTestModal: false,
            showTestModal: false,
            showStressTestModal: false,
            workflowToRemove: null,
            showRemoveWorkflowModal: false,
            currentAddTestWorkflow: "",
            testToRemove: null,
            paths: [],
            servers: [],
            schemas: [],
            editTest: false,
            defaultTestValues: {
                defaultServer: "",
                defaultPath: "",
                defaultMethod: "Get",
                defaultHeaders: [{
                    keyItem: '',
                    valueItem: ''
                }],
                defaultCode: "200",
                defaultSchema: ""
            },
            currentTest: null
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
        this.showRemoveTest = this.showRemoveTest.bind(this)
        this.RemoveTest = this.RemoveTest.bind(this)
        this.disableRemoveTestModal = this.disableRemoveTestModal.bind(this)
        this.showRemoveWorkflow = this.showRemoveWorkflow.bind(this)
        this.RemoveWorkflow = this.RemoveWorkflow.bind(this)
        this.disableRemoveWorkflowModal = this.disableRemoveWorkflowModal.bind(this)
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
                let aux = {
                    defaultServer: response.Servers[0],
                    defaultPath: response.Paths[0],
                    defaultMethod: "Get",
                    defaultHeaders: [{
                        keyItem: '',
                        valueItem: ''
                    }],
                    defaultCode: "200",
                    defaultSchema: ""
                }
                this.setState({ defaultTestValues:aux, paths: response.Paths, servers: response.Servers, schemas: response.Schemas})
            })
        })
    }

    renderStressTest(stress) {

    }

    renderTest(test, testindex, workflow) {
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
                                        {test.Headers.length !== 0 &&
                                            <tr>
                                                <td>
                                                    {test.Headers.map((item, index) => {
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
                                    <AwesomeButton className="buttonEdit"  type="primary" onPress={() => this.editTest(test,workflow)}><img  width="50" height="50" src={editIcon} alt="Logo" /></AwesomeButton>
                                </div>
                                <div style={{ marginTop: "20px",  textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={() => this.showRemoveTest(test,workflow)}><img  width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
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
                                return this.renderTest(test, testindex, item)
                            })}
                            <div style={{marginTop:"20px"}}>
                                <AwesomeButton style={{ marginRight: "20px" }} size="large" type="primary" onPress={() => this.addTest(item)}>Add Test</AwesomeButton>
                                <AwesomeButton size="large" type="primary" onPress={() => this.addStressTest(item)}>Add Stress Test</AwesomeButton>
                                <div style={{ textAlign: "right" }} >
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={() => this.showRemoveWorkflow(item)}><img width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
                                </div>
                            </div>
                        </Accordion.Body>
                    </Accordion.Item >
                })}
            </Accordion>
        )
    }

    editTest(test, workflow) {
        let aux = {
            defaultServer: test.Server,
            defaultPath: test.Path,
            defaultMethod: test.Method,
            defaultHeaders: test.Headers,
            defaultCode: test.Verifications.Code,
            defaultSchema: test.Verifications.Schema
        }
        this.setState({ defaultTestValues: aux, showTestModal: true, currentAddTestWorkflow: workflow, editTest: true, currentTest: test })
    }

    addWorkflow() {
        this.setState({ showWorkflowModal: true })
    }

    addTest(workflow) {
        this.setState({ showTestModal: true, currentAddTestWorkflow: workflow })
    }

    addStressTest(workflow) {
        this.setState({ showStressTestModal: true, currentAddTestWorkflow: workflow })
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
        let auxdefault = {
            defaultServer: this.state.servers[0],
            defaultPath: this.state.paths[0],
            defaultMethod: "Get",
            defaultHeaders: [{
                keyItem: '',
                valueItem: ''
            }],
            defaultCode: "200",
            defaultSchema: ""
        }
        let aux = this.state.workflows
        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.currentAddTestWorkflow.WorkflowID) {
                if (!this.state.editTest) {
                    item.Tests.push(test)
                }
                else {
                    item.Tests.forEach((testaux, testindex) => {
                        console.log(testaux)
                        if (testaux.TestID === this.state.currentTest.TestID) {
                            testaux.TestID = test.TestID
                            testaux.Server = test.Server
                            testaux.Path = test.Path
                            testaux.Method = test.Method
                            testaux.Headers = test.Headers
                            testaux.Body = test.Body
                            testaux.Verifications = test.Verifications
                        }
                    })
                }
            }
        })
        console.log(aux)
        this.setState({ defaultTestValues: auxdefault, workflows: aux, showTestModal: false, editTest: false })
    }

    createStressTest(formCount, formThreads, formDelay) {
        let aux = this.state.workflows
        let newStress = {
            Count: formCount,
            Threads: formThreads,
            Delay: formDelay
        }
        aux.forEach((item, index) => {
            if (this.state.currentAddTestWorkflow.WorkflowID === item.WorkflowID) {
                item.StressTest = newStress
            }
        })
        this.setState({ workflows: aux, showStressTestModal: false, currentAddTestWorkflow: null })
    }

    disableWorkflowModal() { this.setState({ showWorkflowModal: false }) }

    disableTestModal() {
        let aux = {
            defaultServer: this.state.servers[0],
            defaultPath: this.state.paths[0],
            defaultMethod: "Get",
            defaultHeaders: [{
                keyItem: '',
                valueItem: ''
            }],
            defaultCode: "200",
            defaultSchema: ""
        }

        this.setState({ defaultTestValues:aux, editTest: false, showTestModal: false })
    }

    disableStressTestModal() { this.setState({ showStressTestModal: false, currentAddTestWorkflow: null}) }

    showRemoveTest(test,workflow) {
        this.setState({ testToRemove: test, workflowToRemove: workflow, showRemoveTestModal: true })
    }

    RemoveTest() {
        let aux = this.state.workflows

        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.workflowToRemove.WorkflowID) {
                item.Tests.forEach((testaux, testindex) => {
                    if (testaux.TestID === this.state.testToRemove.TestID) {
                        item.Tests.splice(testindex,1)
                    }
                })
            }
        })

        this.setState({ workflows: aux, showRemoveTestModal: false })
    }

    disableRemoveTestModal() {
        this.setState({ showRemoveTestModal: false })
    }

    showRemoveWorkflow(workflow) {
        this.setState({ workflowToRemove: workflow, showRemoveWorkflowModal: true })
    }

    RemoveWorkflow() {
        let aux = this.state.workflows

        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.workflowToRemove.WorkflowID) {
                aux.splice(index, 1)
            }
        })

        this.setState({ workflows: aux, showRemoveWorkflowModal: false })
    }

    disableRemoveWorkflowModal() {
        this.setState({ showRemoveWorkflowModal: false })
    }

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
                    edit={this.state.editTest}
                    previousTest={this.state.currentTest}
                    defaultValues={this.state.defaultTestValues}
                />
                <ModalCompStressTest
                    okButtonFunc={this.createStressTest}
                    cancelButtonFunc={this.disableStressTestModal}
                    visible={this.state.showStressTestModal}
                    
                />
                <ModalComp
                    title="Delete Test"
                    body="Are you sure you want to delete this test. This will delete everything related to the test."
                    okButtonText="Delete"
                    okButtonFunc={this.RemoveTest}
                    cancelButtonFunc={this.disableRemoveTestModal}
                    visible={this.state.showRemoveTestModal}
                />
                <ModalComp
                    title="Delete Workflow"
                    body="Are you sure you want to delete this workflow. This will delete everything related to the workflow including its tests."
                    okButtonText="Delete"
                    okButtonFunc={this.RemoveWorkflow}
                    cancelButtonFunc={this.disableRemoveWorkflowModal}
                    visible={this.state.showRemoveWorkflowModal}
                />
            </div>
        )
    }
}