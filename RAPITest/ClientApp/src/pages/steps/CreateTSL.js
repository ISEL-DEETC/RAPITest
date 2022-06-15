import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { Row, Col, Accordion,Table } from 'react-bootstrap'
import './UploadFile.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import ModalCompWorkflow from '../../components/ModalCompWorkflow.js'
import ModalCompTest from '../../components/ModalCompTest.js'
import ModalCompStressTest from '../../components/ModalCompStressTest.js'
import ModalComp from '../../components/ModalComp.js'
import SimpleModalComp from '../../components/SimpleModalComp.js'
import workflowIcon from '../../assets/share.png'
import testIcon from '../../assets/test.png'
import editIcon from '../../assets/pencil.png'
import deleteIcon from '../../assets/bin.png'
import stressIcon from '../../assets/analysis.png'
import searchIcon from '../../assets/search.png'
import backIcon from '../../assets/back.png'
import continueIcon from '../../assets/continue.png'
import './CreateTSL.css';

const YAML = require('json-to-pretty-yaml');

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
            showRemoveStressTestModal: false,
            currentAddTestWorkflow: "",
            testToRemove: null,
            showBodyModal: false,
            showBodyData: null,
            paths: [],
            servers: [],
            schemas: [],
            schemasValues: [],
            editTest: false,
            editStressTest: false,
            defaultStressTestValues: {
                defaultCount: 1,
                defaultThreads: 1,
                defaultDelay:0
            },
            defaultTestValues: {
                defaultServer: "",
                defaultPath: "",
                defaultMethod: "Get",
                defaultHeaders: [{
                    keyItem: '',
                    valueItem: ''
                }],
                defaultCode: 200,
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
        this.editStressTest = this.editStressTest.bind(this)
        this.showRemoveStressTest = this.showRemoveStressTest.bind(this)
        this.RemoveStressTest = this.RemoveStressTest.bind(this)
        this.disableRemoveStressTestModal = this.disableRemoveStressTestModal.bind(this)
        this.showFinalizeButton = this.showFinalizeButton.bind(this)
        this.finalize = this.finalize.bind(this)
        this.showBody = this.showBody.bind(this)
        this.disableShowBodyModal = this.disableShowBodyModal.bind(this)
    }

    async componentDidMount() {
        let aux = {
            defaultServer: this.props.servers[0],
            defaultPath: this.props.paths[0],
            defaultMethod: "Get",
            defaultHeaders: [{
                keyItem: '',
                valueItem: ''
            }],
            defaultCode: 200,
            defaultSchema: ""
        }
        this.setState({ defaultTestValues: aux, paths: this.props.paths, servers: this.props.servers, schemas: this.props.schemas, schemasValues: this.props.schemasValues})
    }

    // ---------------------------------------

    // --- WORKFLOW METHODS --- 

    renderWorkflows() {
        return (
            <Accordion defaultActiveKey="0">
                {this.state.workflows.map((item, index) => {
                    return <Accordion.Item key={item.WorkflowID} eventKey={item.WorkflowID}>
                        <Accordion.Header><img style={{ marginRight: "15px" }} width="50" height="50" src={workflowIcon} alt="Logo" />{item.WorkflowID}</Accordion.Header>
                        <Accordion.Body>
                            {this.renderStressTest(item.Stress, index, item)}
                            {item.Tests.map((test, testindex) => {
                                return this.renderTest(test, testindex, item)
                            })}
                            <div style={{ marginTop: "20px" }}>
                                <AwesomeButton className="buttonAdd" style={{ marginRight: "20px" }} type="primary" onPress={() => this.addTest(item)}><img style={{ marginRight: "15px" }} width="50" height="50" src={testIcon} alt="Logo" />Add Test</AwesomeButton>
                                <AwesomeButton className="buttonAdd" disabled={item.Stress !== null || item.Tests.length === 0} type="primary" onPress={() => this.addStressTest(item)}><img style={{ marginRight: "15px" }} width="50" height="50" src={stressIcon} alt="Logo" />Add Stress Test</AwesomeButton>
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

    // Add workflow

    addWorkflow() {
        this.setState({ showWorkflowModal: true })
    }

    createWorkflow(WorkflowID) {
        let workflow = {
            WorkflowID: WorkflowID,
            Stress: null,
            Tests: []
        }
        this.setState({ workflows: this.state.workflows.concat([workflow]), showWorkflowModal: false })
    }

    disableWorkflowModal() { this.setState({ showWorkflowModal: false }) }

    // Remove Workflow

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


    // -------------------------------------------------------------


    // --- TEST METHODS ---

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
                                        {this.renderHeaders(test.Headers)}
                                        {test.Body !== "" &&
                                            <tr>
                                            {test.Body.length <= 50 ? <td>{test.Body}</td> : <td>{test.Body.substring(0, 50) + '...'}<img className="seeMoreBody" onClick={() => this.showBody(test.Body)} style={{ marginLeft: "15px" }} width="25" height="25" src={searchIcon} alt="Logo" /></td>}
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
                                <div style={{ marginTop: "55px", textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="primary" onPress={() => this.editTest(test, workflow)}><img width="50" height="50" src={editIcon} alt="Logo" /></AwesomeButton>
                                </div>
                                <div style={{ marginTop: "20px", textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={() => this.showRemoveTest(test, workflow)}><img width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
                                </div>
                            </Col>
                        </Row>
                    </Accordion.Body>
                </Accordion.Item >
            </Accordion>
        )
    }

    renderHeaders(header) {
        if (header.length !== 0) {
            if (header.length === 1 && header[0].keyItem === "" && header[0].valueItem === "") {
                return
            }
            return (
                <tr>
                    <td>
                        {header.map((item, index) => {
                            return <p key={index}>{item.keyItem + ":" + item.valueItem}</p>
                        })}
                    </td>
                </tr>
            )
        }
        return
    }

    showBody(body) {
        this.setState({ showBodyModal: true, showBodyData: body })
    }

    disableShowBodyModal() {
        this.setState({ showBodyModal:false })
    }

    // Add Test

    addTest(workflow) {
        this.setState({ showTestModal: true, currentAddTestWorkflow: workflow })
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
            defaultCode: 200,
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
        this.setState({ defaultTestValues: auxdefault, workflows: aux, showTestModal: false, editTest: false })
    }

    disableTestModal() {
        let aux = {
            defaultServer: this.state.servers[0],
            defaultPath: this.state.paths[0],
            defaultMethod: "Get",
            defaultHeaders: [{
                keyItem: '',
                valueItem: ''
            }],
            defaultCode: 200,
            defaultSchema: ""
        }

        this.setState({ defaultTestValues: aux, editTest: false, showTestModal: false })
    }

    // Edit Test

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

    // Remove Test

    showRemoveTest(test, workflow) {
        this.setState({ testToRemove: test, workflowToRemove: workflow, showRemoveTestModal: true })
    }

    RemoveTest() {
        let aux = this.state.workflows

        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.workflowToRemove.WorkflowID) {
                item.Tests.forEach((testaux, testindex) => {
                    if (testaux.TestID === this.state.testToRemove.TestID) {
                        item.Tests.splice(testindex, 1)
                    }
                })
            }
        })

        this.setState({ workflows: aux, showRemoveTestModal: false })
    }

    disableRemoveTestModal() {
        this.setState({ showRemoveTestModal: false })
    }


    // -------------------------------------------

    // --- STRESS TEST METHODS

    renderStressTest(stress, index, workflow) {
        if(stress === null) return
        return (
            <Accordion key={index} defaultActiveKey={index}>
                <Accordion.Item key={index} eventKey={index}>
                    <Accordion.Header><img style={{ marginRight: "15px" }} width="40" height="40" src={stressIcon} alt="LogoTest" />Stress Test</Accordion.Header>
                    <Accordion.Body>
                        <Row>
                            <Col>
                                <h4>Data</h4>
                                <Table striped bordered hover>
                                    <tbody>
                                        <tr>
                                            <td>Count: {stress.Count}</td>
                                        </tr>
                                        <tr>
                                            <td>Threads: {stress.Threads}</td>
                                        </tr>
                                        <tr>
                                            <td>Delay: {stress.Delay}</td>
                                        </tr>
                                    </tbody>
                                </Table>
                            </Col>
                            <Col>
                                <div style={{ marginTop: "55px", textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="primary" onPress={() => this.editStressTest(stress,workflow)}><img width="50" height="50" src={editIcon} alt="Logo" /></AwesomeButton>
                                </div>
                                <div style={{ marginTop: "20px", textAlign: "center" }}>
                                    <AwesomeButton className="buttonEdit" type="secondary" onPress={() => this.showRemoveStressTest(workflow)}><img width="50" height="50" src={deleteIcon} alt="Logo" /></AwesomeButton>
                                </div>
                            </Col>
                        </Row>
                    </Accordion.Body>
                </Accordion.Item >
            </Accordion>
        )
    }

    // Add Stress Test

    addStressTest(workflow) {
        this.setState({ showStressTestModal: true, currentAddTestWorkflow: workflow })
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
                item.Stress = newStress
            }
        })
        this.setState({ workflows: aux, showStressTestModal: false, currentAddTestWorkflow: null, editStressTest: false  })
    }

    disableStressTestModal() { this.setState({ showStressTestModal: false, currentAddTestWorkflow: null, editStressTest: false,  }) }

    // Edit Stress Test

    editStressTest(stress,workflow) {
        let aux = {
            defaultCount: stress.Count,
            defaultThreads: stress.Threads,
            defaultDelay: stress.Delay
        }
        this.setState({ defaultStressTestValues: aux, showStressTestModal: true, currentAddTestWorkflow: workflow, editStressTest: true })
    }

    // Remove Stress Test

    showRemoveStressTest(workflow) {
        this.setState({  workflowToRemove: workflow, showRemoveStressTestModal: true })
    }

    RemoveStressTest() {
        let aux = this.state.workflows

        aux.forEach((item, index) => {
            if (item.WorkflowID === this.state.workflowToRemove.WorkflowID) {
                item.Stress = null
            }
        })

        this.setState({ workflows: aux, showRemoveStressTestModal: false })
    }

    disableRemoveStressTestModal() {
        this.setState({ showRemoveStressTestModal: false })
    }

    // --- FINALIZE METHODS ---

    showFinalizeButton() {
        let found = true
        if (this.state.workflows.length >= 1) {
            this.state.workflows.forEach((item, index) => {
                if (item.Tests.length >= 1) {
                    found = false;
                }
            })
        }
        return found
    }

    async finalize() {
        let aux = this.state.workflows
        let dictionaryFile = ""

        aux.forEach((item, index) => {

            if (item.Stress === null) {
                delete item.Stress
            }

            item.Tests.forEach((testaux, testindex) => {
                

                testaux.Headers = this.setupHeaders(testaux.Headers)
                if (testaux.Headers.length === 0) {
                    delete testaux.Headers
                }

                testaux.Verifications = this.setupVerifications(testaux.Verifications)
                if (testaux.Body === "") {
                    delete testaux.Body
                }
                else {
                    dictionaryFile += "dictionaryID:" + testaux.TestID + "Body\n" + testaux.Body+"\n\n"
                    testaux.Body = "$ref/dictionary/"+testaux.TestID+"Body"
                }
            })
        })

        let newFile = YAML.stringify(aux);
        var blob = new Blob([newFile], {
            type: 'text/plain'
        });
        let url = window.URL.createObjectURL(blob);
        let a = document.createElement('a');
        a.href = url;
        a.download = 'Created_TSL.yaml';
        a.click();


        var blobDic = new Blob([dictionaryFile], {
            type: 'text/plain'
        });
        let urlDic = window.URL.createObjectURL(blobDic);
        let aDic = document.createElement('a');
        aDic.href = urlDic;
        if (dictionaryFile !== "") {
            aDic.download = 'dictionary.txt';
            aDic.click();
        }

        const file = new File([blob], 'sample.txt')
        const fileDic = new File([blobDic], 'dic.txt')

        this.props.handlerTest([file], fileDic, null)
    }

    setupHeaders(headers) {
        let newHeaders = []

        headers.forEach((item, index) => {
            if (item.keyItem === "") return
            newHeaders.push(item.keyItem+":"+item.valueItem)
        })

        return newHeaders
    }

    setupVerifications(verifications) {
        let newVerifications = []
        if (verifications.Schema === "") {
            delete verifications.Schema
        }
        else {
            verifications.Schema = "$ref/definitions/" + verifications.Schema
        }
        newVerifications.push(verifications)

        return newVerifications
    }

    // --------------------------------------------


    render() {
        return (
            <div>
                {this.renderWorkflows()}
                <div style={{marginTop:"30px"}}>
                    <AwesomeButton className="buttonAdd" type="primary" onPress={this.addWorkflow}><img style={{ marginRight: "15px" }} width="50" height="50" src={workflowIcon} alt="Logo" />Add Workflow</AwesomeButton>
                </div>
                <div style={{ marginTop: "30px" }}>
                    <AwesomeButton className="buttonAdd" disabled={this.showFinalizeButton()} type="primary" onPress={this.finalize}><img style={{ marginRight: "15px" }} width="50" height="50" src={continueIcon} alt="Logo" />Download & Continue</AwesomeButton>
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
                    schemasValues={this.state.schemasValues}
                    currentWorkflows={this.state.workflows}
                    edit={this.state.editTest}
                    previousTest={this.state.currentTest}
                    defaultValues={this.state.defaultTestValues}
                />
                <ModalCompStressTest
                    okButtonFunc={this.createStressTest}
                    cancelButtonFunc={this.disableStressTestModal}
                    visible={this.state.showStressTestModal}
                    edit={this.state.editStressTest}
                    defaultValues={this.state.defaultStressTestValues}
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
                <ModalComp
                    title="Delete Stress Test"
                    body="Are you sure you want to delete this stress test. This will delete everything related to the stress test."
                    okButtonText="Delete"
                    okButtonFunc={this.RemoveStressTest}
                    cancelButtonFunc={this.disableRemoveStressTestModal}
                    visible={this.state.showRemoveStressTestModal}
                />
                <SimpleModalComp
                    title="Body Data"
                    body={this.state.showBodyData}
                    cancelButtonFunc={this.disableShowBodyModal}
                    visible={this.state.showBodyModal}
                />
                <div style={{ marginTop: '200px' }}>
                    <AwesomeButton  type="primary" onPress={this.props.goBackToSelection}><img style={{ marginRight: "15px" }} width="50" height="50" src={backIcon} alt="Logo" />Go Back</AwesomeButton>
                </div>
            </div>
        )
    }
}