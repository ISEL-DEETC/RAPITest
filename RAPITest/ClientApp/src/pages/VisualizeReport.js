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
            missingTests: null,
            barChartData: null,
            pieChartData: null,
            totalCompletionTime: 0
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
        console.log(report)
        let newDates = [];

        report.allReportDates.forEach((element, index) => {
            newDates.push(this.showTime(element))
        });

        report.report.date = this.showTime(report.report.date)


        let piedata = [];
        piedata.push({ name: 'Total Errors', value: report.report.Errors })

        let bardata = [];

        let totalsuccesses = 0;
        let totalCompletionTime = 0;

        report.report.WorkflowResults.forEach((workflow, workflowindex) => {

            let workflowId = workflow.WorkflowID
            let workflowTotalTime = 0

            workflow.Tests.forEach((test, testindex) => {


                test.TestResults.forEach((testresult, testresultindex) => {
                    if (testresult.Success) totalsuccesses++;
                })

                totalCompletionTime += test.RequestMetadata.ResponseTime
                workflowTotalTime += test.RequestMetadata.ResponseTime

                if (test.StressTimes != null) {
                    test.StressTimes.forEach((time, testresultindex) => {
                        totalCompletionTime += time;
                        workflowTotalTime += time
                    })
                }

            })

            bardata.push({ name: workflowId, Total_Time: workflowTotalTime })
        })

        let generatedTestsCompletionTime = 0

        report.report.GeneratedTests.forEach((generatedTest, workflowindex) => {
            generatedTest.TestResults.forEach((testresult, testresultindex) => {
                if (testresult.Success) totalsuccesses++;
            })
            generatedTestsCompletionTime += generatedTest.RequestMetadata.ResponseTime
            totalCompletionTime += generatedTest.RequestMetadata.ResponseTime
        })

        bardata.push({ name: 'GeneratedTests', Total_Time: generatedTestsCompletionTime })
        piedata.push({ name: 'Total Successes', value: totalsuccesses })
        

        this.setState({
            apiTitle: report.apiName,
            allReportsOriginal: report.allReportDates,
            allReportsUIFriendly: newDates,
            errors: report.report.Errors,
            warnings: report.report.Warnings,
            workflows: report.report.WorkflowResults,
            date: report.report.date,
            generatedTests: report.report.GeneratedTests,
            missingTests: report.report.MissingTests,
            pieChartData: piedata,
            barChartData: bardata,
            totalCompletionTime: totalCompletionTime
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
                                errors={this.state.errors}
                                warnings={this.state.warnings}
                                workflows={this.state.workflows}
                                generatedTests={this.state.generatedTests}
                                totalCompletionTime={this.state.totalCompletionTime}
                                pieChartData={this.state.pieChartData}
                                barChartData={this.state.barChartData}
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
                {this.state.date !== null && this.showRender()}
            </div>
        )
    }
}