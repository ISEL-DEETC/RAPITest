import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import "react-awesome-button/dist/styles.css";
import { Row, Col, Table } from 'react-bootstrap'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import authService from '../api-authorization/AuthorizeService'
import generatedIcon from '../../assets/gear.png'

const YAML = require('json-to-pretty-yaml');

export default class MissingTests extends Component {

    getMethod(method) {
        switch (method) {
            case 0:
                return "Get"
            case 1:
                return "Put"
            case 2:
                return "Post"
            case 3:
                return "Delete"
            default:
                break;
        }
    }

    async generateTSLFile() {
        const token = await authService.getAccessToken();
        let apiId = this.props.apiId
        let apiTitle = this.props.apiTitle

        fetch(`MonitorTest/GenerateMissingTestsTSL?apiId=${apiId}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(resp => {
                resp = YAML.stringify(resp);
                var blob = new Blob([resp], {
                    type: 'text/plain'
                });
                let url = window.URL.createObjectURL(blob);
                let a = document.createElement('a');
                a.href = url;
                a.download = apiTitle + '_MissingTests_TSL.yaml';
                a.click();
            })
    }

    render() {
        let missingTests = this.props.missingTests

        return (
            <div>
                <Row style={{ paddingTop: "20px", paddingBottom: "20px" }}>
                    <Col>
                        <h3 style={{fontWeight: "bold" }}>Missing Tests</h3>
                    </Col>
                    <Col>
                        <AwesomeButton className="buttonAdd" type="primary" onPress={() => this.generateTSLFile()}><img style={{ marginRight: "15px" }} width="50" height="50" src={generatedIcon} alt="Logo" />Generate TSL</AwesomeButton>
                    </Col>
                </Row>
                <Row>
                    <Table striped bordered hover>
                        <thead>
                            <tr>
                                <th>Tests</th>
                                <th>Method</th>
                                <th>URI</th>
                                <th>Headers</th>
                                <th>Code</th>
                            </tr>
                        </thead>
                        <tbody>
                            {missingTests.map((test, i) => {

                                return (
                                    <tr key={i}>
                                        <td>{i}</td>
                                        <td>{this.getMethod(test.Method)}</td>
                                        <td>{test.Server + test.Path}</td>
                                        <td>{JSON.stringify(test.Headers)}</td>
                                        <td>{test.TestID.substr(test.TestID.length-3)}</td>
                                    </tr>
                                )

                            })}
                        </tbody>
                    </Table>
                </Row>
            </div>
        )
    }
}