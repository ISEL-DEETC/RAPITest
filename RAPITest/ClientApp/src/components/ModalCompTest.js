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

const statusMessages = [
    { id: '200', name:'200' },
    { id: '201', name:'201' },
    { id: '202', name: '202' },
    { id: '203', name: '203' },
    { id: '204', name: '204' },
    { id: '205', name: '205' },
    { id: '206', name: '206' },
    { id: '207', name: '207' },
    { id: '208', name: '208' },
    { id: '226', name: '226' },
    { id: '300', name: '300' },
    { id: '301', name: '301' },
    { id: '302', name: '302' },
    { id: '303', name: '303' },
    { id: '304', name: '304' },
    { id: '305', name: '305' },
    { id: '306', name: '306' },
    { id: '307', name: '307' },
    { id: '308', name: '308' },
    { id: '400', name: '400' },
    { id: '401', name: '401' },
    { id: '402', name: '402' },
    { id: '403', name: '403' },
    { id: '404', name: '404' },
    { id: '405', name: '405' },
    { id: '406', name: '406' },
    { id: '407', name: '407' },
    { id: '408', name: '408' },
    { id: '409', name: '409' },
    { id: '410', name: '410' },
    { id: '411', name: '411' },
    { id: '412', name: '412' },
    { id: '413', name: '413' },
    { id: '414', name: '414' },
    { id: '415', name: '415' },
    { id: '416', name: '416' },
    { id: '417', name: '417' },
    { id: '418', name: '418'},
    { id: '420', name: '420' },
    { id: '422', name: '422' },
    { id: '423', name: '423' },
    { id: '424', name: '424' },
    { id: '425', name: '425' },
    { id: '426', name: '426' },
    { id: '428', name: '428' },
    { id: '429', name: '429' },
    { id: '431', name: '431' },
    { id: '444', name: '444' },
    { id: '449', name: '449' },
    { id: '450', name: '450' },
    { id: '451', name: '451' },
    { id: '499', name: '499' },
    { id: '500', name: '500' },
    { id: '501', name: '501' },
    { id: '502', name: '502' },
    { id: '503', name: '503' },
    { id: '504', name: '504' },
    { id: '505', name: '505' },
    { id: '506', name: '506' },
    { id: '507', name: '507' },
    { id: '508', name: '508' },
    { id: '509', name: '509' },
    { id: '510', name: '510' },
    { id: '511', name: '511' },
    { id: '598', name: '598' },
    { id: '599', name: '599' },
];

export default class ModalCompTest extends React.Component {

    constructor() {

        super()

        this.state = {
            title: "Add Test",
            okButton: "Add",
            name: "",
            showWarning: false,
            warningMessage: "",
            selectedPath: "",
            selectedServer: "",
            selectedSchema: "",
            selectedMethod: "Get",
            selectedCode: 200,
            paths: [],
            servers: [],
            schemas: [],
            edit: false,
            previousTest: null,
            headers: [{
                keyItem: '',
                valueItem: ''
            }]
        }

        this.finalizeCallback = this.finalizeCallback.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
        this.pathValue = this.pathValue.bind(this)
        this.updateHeaders = this.updateHeaders.bind(this)
        this.serverValue = this.serverValue.bind(this)
        this.schemaValue = this.schemaValue.bind(this)
        this.methodValue = this.methodValue.bind(this)
        this.codeValue = this.codeValue.bind(this)

    }

    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
            if (!this.props.edit) {
                this.setState({ okButton:"Add", showWarning: false, headers: [{keyItem: '',valueItem: ''}], edit: false, title: "Add Test", previousTest: null, selectedPath: this.props.paths[0], selectedServer: this.props.servers[0], selectedMethod: "Get", selectedCode: "200", selectedSchema:"", paths: this.props.paths, servers: this.props.servers, schemas: this.props.schemas})
            }
            else {
                this.setState({ okButton:"Edit", showWarning: false, edit: true, title: "Edit Test", headers: this.props.previousTest.Headers, previousTest: this.props.previousTest, selectedPath: this.props.previousTest.Path, selectedServer: this.props.previousTest.Server, selectedMethod: this.props.previousTest.Method, selectedCode: this.props.previousTest.Verifications.Code, selectedSchema: this.props.previousTest.Verifications.Schema, paths: this.props.paths, servers: this.props.servers, schemas: this.props.schemas })
            }
        }
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    finalizeCallback() {

        let test = {
            TestID: "",
            Server: "",
            Path: "",
            Method: "",
            Headers: [],
            Body: {
                id: "",
                data: ""
            },
            Verifications: {
                Code: 0,
                Schema: ""
            }
        }

        let formString = document.getElementById("formTestId").value
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
                item.Tests.forEach((test, indextest) => {
                    if (test.TestID === formString) {
                        if (this.state.edit && formString === this.state.previousTest.TestID) return
                        this.setState({ showWarning: true, warningMessage: "ID already used, must be unique" })
                        found = true
                    }
                }) 
            })

            if (!found) {
                if (this.state.selectedPath.includes("{")) {
                    this.setState({ showWarning: true, warningMessage: "Path not valid, please change variable path {..} to concrete value" })
                }
                else {
                    test.TestID = formString
                    test.Server = this.state.selectedServer
                    test.Path = this.state.selectedPath
                    test.Method = this.state.selectedMethod
                    test.Headers = this.state.headers
                    test.Body.id = formString + "Body"
                    test.Body.data = document.getElementById("formBody").value
                    test.Verifications.Code = this.state.selectedCode
                    test.Verifications.Schema = this.state.selectedSchema

                    this.props.okButtonFunc(test)
                }
            }
        }
    }

    pathValue(selectedPaths) {
        this.setState({ selectedPath: selectedPaths })
    }

    serverValue(selectedServers) {
        this.setState({ selectedServer: selectedServers })
    }

    schemaValue(selectedSchemas) {
        this.setState({ selectedSchema: selectedSchemas })
    }

    codeValue(selectedCodes) {
        this.setState({ selectedCode: selectedCodes.id })
    }

    methodValue(e) {
        this.setState({ selectedMethod: e.target.value });
    }

    updateHeaders(headersValue) {
        this.setState({ headers: headersValue })
    }

    render() {
        let cancelButtonFunc = this.props.cancelButtonFunc
        let visible = this.props.visible

        return (
            <div>
                <Modal size="lg" show={visible} onHide={cancelButtonFunc}>
                    <Modal.Header closeButton>
                        <Modal.Title>{this.state.title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                        <Form>
                            <Form.Group className="mb-3" controlId="formTestId">
                                <Form.Label>Test ID</Form.Label>
                                {this.state.edit ? <Form.Control defaultValue={this.state.previousTest.TestID} /> : <Form.Control placeholder="Enter ID" /> }
                                <Form.Text className="text-muted">
                                    The ID of the added test, must be unique across all tests
                                </Form.Text>
                            </Form.Group>
                        </Form>
                        <div>
                            Server
                            <Combobox
                                data={this.state.servers}
                                filter={false}
                                onChange={value => this.serverValue(value)}
                                defaultValue={this.props.defaultValues.defaultServer}
                                />
                        </div>
                        <div style={{marginTop:"10px"}}>
                            Path
                            <Combobox
                                data={this.state.paths}
                                filter={false}
                                onChange={value => this.pathValue(value)}
                                defaultValue={this.props.defaultValues.defaultPath}
                            />
                        </div>
                        <div style={{ marginTop: "20px" }}>
                            Method
                            <Form.Select aria-label="Default select example" value={this.state.selectedMethod} onChange={this.methodValue} >
                                <option value="Get">Get</option>
                                <option value="Post">Post</option>
                                <option value="Put">Put</option>
                                <option value="Delete">Delete</option>
                            </Form.Select>
                        </div>
                        <div style={{ marginTop: "15px" }}>
                            Headers, Key/Value
                            <KeyValue
                                hideLabels={true}
                                rows={this.props.defaultValues.defaultHeaders}
                                customAddButtonRenderer={handleAddNew => (
                                    <div className="key-value-add-new" style={{marginTop: "5px"}}>
                                        <div onClick={handleAddNew}>
                                            <img style={{ marginBottom: "5px" }} width="30" height="30" src={addIcon} alt="LogoBin" /> Add Header
                                        </div>
                                    </div>
                                )}
                                onChange={rows => this.updateHeaders(rows)}
                                />
                        </div>
                        <Form style={{ marginTop: "15px" }}>
                            <Form.Group className="mb-3" controlId="formBody">
                                <Form.Label>Body</Form.Label>
                                {this.state.edit ? <Form.Control defaultValue={this.state.previousTest.Body.data} /> : <Form.Control placeholder="Body Data" />}
                            </Form.Group>
                        </Form>
                        <div style={{ marginTop: "20px" }}>
                            <h4>Verifications</h4>
                            <div>Code</div>
                            <Combobox
                                data={statusMessages}
                                dataKey='id'
                                textField='name'
                                defaultValue={this.props.defaultValues.defaultCode}
                                onChange={value => this.codeValue(value)}
                            />
                            <div>Schema</div>
                            <Combobox
                                data={this.state.schemas}
                                filter={false}
                                defaultValue={this.props.defaultValues.defaultSchema}
                                onChange={value => this.schemaValue(value)}
                            />
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <AwesomeButton type="primary" onPress={this.finalizeCallback}>{this.state.okButton}</AwesomeButton>
                        <AwesomeButton type="secondary" onPress={cancelButtonFunc}>Cancel</AwesomeButton>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}