import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import Overview from './VisualizeReportTabs/Overview'
import GeneratedTests from './VisualizeReportTabs/GeneratedTests'
import TSLWorkflows from './VisualizeReportTabs/TSLWorkflows'
import { Tabs, Tab, Row, Col } from 'react-bootstrap'

export class VisualizeReport extends Component {

    static displayName = VisualizeReport.name;

    constructor(props) {
        super(props)
        this.state = {
            apiTitle: "Report",
            allReportsOriginal: null,
            allReportsUIFriendly: null,
            errors: 0,
            warnings: 0,
            workflows: null,
            date: null,
            generatedTests: null,
            missingTests: null
        }

        this.setupReport = this.setupReport.bind(this)
    }

    async componentDidMount() {
        const token = await authService.getAccessToken();
        let apiId = this.props.match.params.apiId
        
        fetch(`MonitorTest/ReturnReport?apiId=${apiId}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(resp => {
                resp.report = JSON.parse(resp.report)
                this.setupReport(resp)
            })

    }

    setupReport(report) {
        let newDates = [];

        report.allReportDates.forEach((element, index) => {
            newDates.push(this.showTime(element))
        });

        report.report.date = this.showTime(report.report.date)

        this.setState({
            apiTitle: report.apiName,
            allReportsOriginal: report.allReportDates,
            allReportsUIFriendly: newDates,
            errors: report.report.Errors,
            warnings: report.report.Warnings,
            workflows: report.report.WorkflowResults,
            date: report.report.date,
            generatedTests: report.report.GeneratedTests,
            missingTests: report.report.MissingTests
        })
    }

    showTime(value) {
        if (value === null) return 0
        let time = value.split('T')
        let date = time[0]
        let hours = time[1].split('.')[0]

        return date + " " + hours
    }

    showRender() {
        console.log(this.state)
        return (
            <div>
                <Row>
                    <Col>
                        <h1>{this.state.apiTitle}</h1>
                    </Col>
                    <Col>
                        <h1>{this.state.date}</h1>
                    </Col>
                </Row>
                <Row style={{ paddingTop:"10px" }}>
                    <Tabs defaultActiveKey="overview" id="uncontrolled-tab-example" className="mb-3">
                        <Tab eventKey="overview" title="Overview">
                            <Overview
                    
                            />
                        </Tab>
                        <Tab eventKey="tslworkflows" title="TSL Workflows">
                            <TSLWorkflows
                                report={this.state.report}
                                tests={this.state.Tests}
                            />
                        </Tab>
                        <Tab eventKey="generatedtests" title="Generated Tests">
                            <GeneratedTests
                    
                            />
                        </Tab>
                    </Tabs>
                </Row>
            </div>
        )
    }

    render() {
        return (
            <div>
                {this.state.report !== null && this.showRender()}
            </div>
        )
    }
}