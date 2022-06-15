import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import { Row, Col, Form } from 'react-bootstrap'
import Dropzone from '../../components/Dropzone'
import './UploadFile.css';
import { warningMessage, dangerMessage } from '../../components/AlertComp'
import authService from '../api-authorization/AuthorizeService';
import Loader from 'react-loader-spinner'
import { AwesomeButton } from "react-awesome-button";
import backIcon from '../../assets/back.png'
import uploadIcon from '../../assets/uploadSmall.png'

const delay = ms => new Promise(res => setTimeout(res, ms));

export class UploadApiSpecification extends Component {

    constructor() {

        super()

        this.state = {
            showInput: true,
            showDanger: false,
            showWarning: false,
            warningMessage: ""
        }

        this.onDrop = this.onDrop.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
        this.closeDanger = this.closeDanger.bind(this)
        this.uploadURL = this.uploadURL.bind(this)
    }

    //callback for dropzone
    onDrop(accept, reject) {
        if (reject.length !== 0 || accept.length > 1) {
            this.setState({ showWarning: true, warningMessage: "Please upload only one yaml or json file" })
        }
        else {
            this.setState({ showInput: false }, async () => {

                let data = new FormData();
                data.append('apiSpecification', accept[0]);
                data.append('title', this.props.apiTitle);
                const token = await authService.getAccessToken();
                fetch(`SetupTest/GetSpecificationDetails`, {
                    method: 'POST',
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    body: data
                }).then(async (res) => {
                    if (!res.ok) {
                        await delay(2000);
                        this.setState({ showDanger: true, showInput: true })
                    }
                    else {
                        await delay(2500);
                        res.json().then(response => {
                            this.props.handlerAPI(response.Paths, response.Servers, response.Schemas, response.SchemasValues)
                        })
                    }
                })
            })
        }
    }

    uploadURL() {
        let formString = document.getElementById("formURL").value
        if (formString.length === 0) {
            this.setState({ showWarning: true, warningMessage: "Please fill out the required form" })
            return
        }

        this.setState({ showInput: false }, async () => {
            let data = new FormData();
            data.append('apiSpecification', formString);
            data.append('title', this.props.apiTitle);
            const token = await authService.getAccessToken();
            fetch(`SetupTest/GetSpecificationDetailsURL`, {
                method: 'POST',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                body: data
            }).then(async (res) => {
                if (!res.ok) {
                    await delay(2000);
                    this.setState({ showDanger: true, showInput: true })
                }
                else {
                    await delay(2500);
                    res.json().then(response => {
                        this.props.handlerAPI(response.Paths, response.Servers, response.Schemas, response.SchemasValues)
                    })
                }
            })
        })
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    closeDanger() {
        this.setState({ showDanger: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                {this.state.showDanger ? dangerMessage("Error while validating OpenAPI Specification, please upload a valid specification", this.closeDanger) : <div></div>}
                {this.state.showInput && <div>
                    <Row>
                        <div className="root-dropzone">
                            <Dropzone
                                accept=".yaml, .json"
                                onDrop={this.onDrop}
                                history={this.props.history}
                                text={
                                    <div align="center">
                                        <p>Drop your API specification here, or click to select it.</p>
                                        <p>Only .yaml or .json files will be accepted</p>
                                    </div>}
                                />
                        </div>
                    </Row>
                    <Row style={{marginTop:'30px'}}>
                        <Col sm={10}>
                            <Form>
                                <Form.Group className="mb-3" controlId="formURL">
                                    <Form.Label>OpenAPI Specification URL</Form.Label>
                                    <Form.Control placeholder="Enter URL" />
                                    <Form.Text className="text-muted">
                                        The URL of the OpenAPI Specification you want to test
                                    </Form.Text>
                                </Form.Group>
                            </Form>
                        </Col>
                        <Col sm={2}>
                            <AwesomeButton className="buttonAdd" style={{ marginTop: '20px' }} type="primary" onPress={this.uploadURL}><img style={{ marginRight: "15px" }} width="50" height="50" src={uploadIcon} alt="Logo" />Upload</AwesomeButton>
                        </Col>
                    </Row>
                    <div style={{ marginTop: '200px' }}>
                        <AwesomeButton type="primary" onPress={this.props.goBack}><img style={{ marginRight: "15px" }} width="50" height="50" src={backIcon} alt="Logo" />Go Back</AwesomeButton>
                    </div>
                </div>}
                {!this.state.showInput && <div>
                    <Row>
                        <Col style={{marginTop:'15px'}}>
                            <h4>Please wait while the OpenAPI Specification is processed</h4>
                        </Col>
                        <Col>
                            <Loader type="Grid" color="#00BFFF" height={55} width={55} />
                        </Col>
                    </Row>
                </div>}
                
             </div>
             
        )
    }
}