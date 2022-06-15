import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import Overview from './VisualizeReportTabs/Overview'
import GeneratedTests from './VisualizeReportTabs/GeneratedTests'
import TSLWorkflows from './VisualizeReportTabs/TSLWorkflows'
import MissingTests from './VisualizeReportTabs/MissingTests'
import { Row, Col, Table, DropdownButton, Dropdown, Figure  } from 'react-bootstrap'
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import SlidingPane from "react-sliding-pane";
import "react-sliding-pane/dist/react-sliding-pane.css";
import HttpRequestInfoComp from '../components/HttpRequestInfoComp'
import calendarIcon from '../assets/calendar.png'
import reportIcon from '../assets/statsSmall.png'
import ReactReadMoreReadLess from "react-read-more-read-less";

const average = arr => arr.reduce((a, b) => a + b, 0) / arr.length;

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
            totalCompletionTime: 0,
            fullWorkflows: null,
            openSidePanel: false,
            sidePanelInfo: {},
            stressTestData: null,
            stressTestColumns: null,
            stressTestMetadata: null,
            fullGeneratedTests: null
        }

        this.setupReport = this.setupReport.bind(this)
        this.showSidePanelDetails = this.showSidePanelDetails.bind(this)
        this.showSidePanel = this.showSidePanel.bind(this)
        this.loadPreviousReport = this.loadPreviousReport.bind(this)
    }

    async componentDidMount() {
        const token = await authService.getAccessToken();
        let apiId = this.props.match.params.apiId
        
        fetch(`MonitorTest/ReturnReport?apiId=${apiId}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(resp => {
                resp.Report = JSON.parse(resp.Report)
                this.setupReport(resp)
            })

    }

    setupReport(report) {
        let newDates = [];

        report.AllReportDates.forEach((element, index) => {
            newDates.push(this.showTime(element))
        });


        report.Report.date = this.showTime(report.Report.date)


        let piedata = [];
        piedata.push({ name: 'Total Errors', value: report.Report.Errors })

        let bardata = [];

        let totalsuccesses = 0;
        let totalCompletionTime = 0;

        let fullWorkflows = []

        let stressTestData = []
        let stressTestColumns = []
        let stressTestMetadata = []

        report.Report.WorkflowResults.forEach((workflow, workflowindex) => {

            let thisWorkflow = []

            let workflowStressData = []
            let workflowStressColumns = []
            let workflowStressMetadata = []

            let workflowId = workflow.WorkflowID
            let workflowTotalTime = 0

            let currentWorkflow = []
            currentWorkflow.push({ displayName: workflowId, id: workflowId, targetId: [] })
            thisWorkflow.push(currentWorkflow)

            workflow.Tests.forEach((test, testindex) => {

                let testStressMetadata = []

                workflowStressColumns.push(test.TestID)
                testStressMetadata.push(test.TestID)

                let currentTest = []
                currentTest.push({ displayName: test.TestID, id: test.TestID, targetId: [] })

                test.TestResults.forEach((testresult, testresultindex) => {
                    currentTest.push({ displayName: testresult.TestName, id: test.TestID + testresult.TestName, targetId: [], success: testresult.Success })
                    if (testresult.Success) totalsuccesses++;
                })
                thisWorkflow.push(currentTest)

                totalCompletionTime += test.RequestMetadata.ResponseTime
                workflowTotalTime += test.RequestMetadata.ResponseTime

                if (test.StressTimes != null) {

                    testStressMetadata.push(Math.min(...test.StressTimes))
                    testStressMetadata.push(Math.max(...test.StressTimes))
                    testStressMetadata.push(average(test.StressTimes))
                    testStressMetadata.push(test.StressTimes[test.StressTimes.length - 1])
                    testStressMetadata.push(test.StressTimes.length)

                    test.StressTimes.forEach((time, testresultindex) => {
                        totalCompletionTime += time;
                        workflowTotalTime += time

                        if (workflowStressData[testresultindex] !== undefined) {
                            let aux = workflowStressData[testresultindex]
                            aux[test.TestID] = time
                        }
                        else {
                            let newAux = {}
                            newAux.name = testresultindex
                            newAux[test.TestID] = time
                            workflowStressData.push(newAux)
                        }
                    })
                }

                workflowStressMetadata.push(testStressMetadata)

            })

            stressTestColumns.push(workflowStressColumns)
            stressTestData.push(workflowStressData)
            stressTestMetadata.push(workflowStressMetadata)
            fullWorkflows.push(thisWorkflow)
            bardata.push({ name: workflowId, Total_Time: workflowTotalTime })
        })

        this.setupCircleTreeMultiple(fullWorkflows)

        let generatedTestsCompletionTime = 0

        let fullGeneratedTests = []
        fullGeneratedTests.push([{ displayName: 'Generated', id: 'Generated', targetId: [] }])

        report.Report.GeneratedTests.forEach((generatedTest, workflowindex) => {

            let currentTest = []
            currentTest.push({ displayName: generatedTest.TestID, id: generatedTest.TestID, targetId: [] })

            generatedTest.TestResults.forEach((testresult, testresultindex) => {
                currentTest.push({ displayName: testresult.TestName, id: generatedTest.TestID + testresult.TestName, targetId: [], success: testresult.Success })
                if (testresult.Success) totalsuccesses++;
            })
            generatedTestsCompletionTime += generatedTest.RequestMetadata.ResponseTime
            totalCompletionTime += generatedTest.RequestMetadata.ResponseTime
            fullGeneratedTests.push(currentTest)
        })

        this.setupCircleTree(fullGeneratedTests)

        bardata.push({ name: 'GeneratedTests', Total_Time: generatedTestsCompletionTime })
        piedata.push({ name: 'Total Successes', value: totalsuccesses })
        

        this.setState({
            apiTitle: report.ApiName,
            allReportsOriginal: report.AllReportDates,
            allReportsUIFriendly: newDates,
            errors: report.Report.Errors,
            warnings: report.Report.Warnings,
            workflows: report.Report.WorkflowResults,
            date: report.Report.date,
            generatedTests: report.Report.GeneratedTests,
            missingTests: report.Report.MissingTests,
            pieChartData: piedata,
            barChartData: bardata,
            totalCompletionTime: totalCompletionTime,
            fullWorkflows: fullWorkflows,
            stressTestData: stressTestData,
            stressTestColumns: stressTestColumns,
            stressTestMetadata: stressTestMetadata,
            fullGeneratedTests: fullGeneratedTests
        })
    }

    showSidePanel(id, workflowIndex, testIndex, fromGenerated) {
        let information = {}

        if (fromGenerated) {
            if (workflowIndex === 0) {
                information.workflow = { Tests: this.state.generatedTests }
                information.title = id
                information.subtitle = "Workflow Details"
            }
            else {
                if (testIndex === 0) {
                    this.state.generatedTests.forEach((test, wi) => {
                        if (id.includes(test.TestID)) {
                            information.test = test
                            information.title = id
                            information.subtitle = "Test Details"
                        }
                    })
                }
                else {
                    this.state.generatedTests.forEach((test, wi) => {
                        if (id.includes(test.TestID)) {
                            let target = id.substring(test.TestID.length, id.length)
                            test.TestResults.forEach((result, ri) => {
                                if (target === result.TestName) {
                                    information.verification = result
                                    information.title = result.TestName
                                    information.subtitle = "Verification Details"
                                }
                            })
                        }
                    })
                }
            }
        } else {
            if (workflowIndex === 0) {
                this.state.workflows.forEach((workflow, wi) => {
                    if (id === workflow.WorkflowID) {
                        information.workflow = workflow
                        information.title = id
                        information.subtitle = "Workflow Details"
                    }
                })
            }
            else {
                if (testIndex === 0) {
                    this.state.workflows.forEach((workflow, wi) => {
                        workflow.Tests.forEach((test, ti) => {
                            if (id.includes(test.TestID)) {
                                information.test = test
                                information.title = id
                                information.subtitle = "Test Details"
                            }
                        })
                    })
                }
                else {
                    this.state.workflows.forEach((workflow, wi) => {
                        workflow.Tests.forEach((test, ti) => {
                            if (id.includes(test.TestID)) {
                                let target = id.substring(test.TestID.length, id.length)
                                test.TestResults.forEach((result, ri) => {
                                    if (target === result.TestName) {
                                        information.verification = result
                                        information.title = result.TestName
                                        information.subtitle = "Verification Details"
                                    }
                                })
                            }
                        })
                    })
                }
            }
        }
        

        this.setState({
            openSidePanel: true,
            sidePanelInfo: information
        })
    }

    showSidePanelDetails() {
        if (this.state.sidePanelInfo.workflow !== undefined) {
            return <Table striped bordered hover>
                <thead>
                    <tr>
                        <th>Tests</th>
                        <th># Successes</th>
                        <th># Errors</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.sidePanelInfo.workflow.Tests.map((test, i) => {
                        let nErrors = 0
                        let nSuccesses = 0

                        test.TestResults.forEach((r, i) => {
                            if (r.Success) {
                                nSuccesses++
                            }
                            else {
                                nErrors++
                            }
                        })

                        return (
                            <tr key={i}>
                                <td>{test.TestID}</td>
                                <td>{nSuccesses}</td>
                                <td>{nErrors}</td>
                            </tr>
                        )
                        
                    })}
                </tbody>
            </Table>
        }
        if (this.state.sidePanelInfo.test !== undefined) {
            let myTest = this.state.sidePanelInfo.test

            return <HttpRequestInfoComp
                Method={myTest.RequestMetadata.Method}
                URI={myTest.RequestMetadata.URI}
                requestHeaders={myTest.Headers}
                requestBody={myTest.Body}
                code={myTest.RequestMetadata.ResponseCode}
                responseHeaders={myTest.RequestMetadata.ResponseHeaders}
                responseBody={myTest.RequestMetadata.ResponseBody}
                responseTime={myTest.RequestMetadata.ResponseTime}
            />
        }
        if (this.state.sidePanelInfo.verification !== undefined) {
            return <Table striped bordered hover>
                <tbody>
                    <tr>
                        <td>Success</td>
                        <td>{this.state.sidePanelInfo.verification.Success ? "true" : "false"}</td>
                    </tr>
                    <tr>
                        <td>Description</td>
                        <td>
                            <ReactReadMoreReadLess
                                charLimit={800}
                                readMoreText={"Read more ▼"}
                                readLessText={"Read less ▲"}
                                readMoreClassName="read-more-less--more"
                                readLessClassName="read-more-less--more"
                            >
                                {this.state.sidePanelInfo.verification.Description === null ? "" : this.state.sidePanelInfo.verification.Description}
                            </ReactReadMoreReadLess>
                        </td>
                    </tr>
                </tbody>
            </Table>
        }
    }

    setupCircleTreeMultiple(fullWorkflows) {
        fullWorkflows.forEach((workflow, workflowindex) => {
            this.setupCircleTree(workflow)
        })
    }

    setupCircleTree(workflow) {
        for (var i = 0; i < workflow.length; i++) {
            if (i + 1 !== workflow.length) {
                workflow[i][0].targetId.push(workflow[i + 1][0].id)
            }
            let TestList = workflow[i]
            for (var k = 0; k < TestList.length; k++) {
                if (k + 1 !== TestList.length) {
                    TestList[k].targetId.push(TestList[k + 1].id)
                }
            }
        }
    }

    async loadPreviousReport(date) {
        const token = await authService.getAccessToken();
        let apiId = this.props.match.params.apiId

        fetch(`MonitorTest/ReturnReportSpecific?apiId=${apiId}&date=${date}`, {
            method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
            .then(resp => {
                resp.Report = JSON.parse(resp.Report)
                this.setupReport(resp)
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
        return (
            <div>
                <Row>
                    <Col>    
                        <h1><img style={{ marginRight: "15px" }} width="64" height="64" src={reportIcon} alt="Logo" />{this.state.apiTitle}</h1>
                    </Col>
                    <Col className="d-flex justify-content-end">
                        <Figure style={{paddingRight:"10px"}}>
                            <Figure.Image
                                width={40}
                                height={40}
                                alt={"40x40"}
                                src={calendarIcon}
                            />
                            <Figure.Caption>
                            </Figure.Caption>
                        </Figure>
                        <DropdownButton id="dropdown-item-button" title={this.state.date}>
                            {this.state.allReportsUIFriendly.map((o, i) => {
                                if (o === this.state.date) return <div key={i}></div>
                                return <Dropdown.Item key={i} as="button" onClick={() => this.loadPreviousReport(o)}>{o}</Dropdown.Item>
                            })}
                        </DropdownButton>
                    </Col>
                </Row>
                <Row style={{ paddingTop:"10px" }}>
                    <Tabs id='myTabsID'>
                        <TabList>
                            <Tab>Overview</Tab>
                            <Tab>TSL Workflows</Tab>
                            <Tab>Generated Tests</Tab>
                            <Tab>Missing Tests</Tab>
                        </TabList>
                        <TabPanel>
                            <Overview
                                errors={this.state.errors}
                                warnings={this.state.warnings}
                                workflows={this.state.workflows}
                                generatedTests={this.state.generatedTests}
                                totalCompletionTime={this.state.totalCompletionTime}
                                pieChartData={this.state.pieChartData}
                                barChartData={this.state.barChartData}
                            />
                        </TabPanel>
                        <TabPanel>
                            <TSLWorkflows
                                fullWorkflows={this.state.fullWorkflows}
                                clickableFunction={this.showSidePanel}
                                stressTestData={this.state.stressTestData}
                                stressTestColumns={this.state.stressTestColumns}
                                stressTestMetadata={this.state.stressTestMetadata}
                            />
                        </TabPanel>
                        <TabPanel>
                            <GeneratedTests
                                fullGeneratedTests={this.state.fullGeneratedTests}
                                clickableFunction={this.showSidePanel}
                            />
                        </TabPanel>
                        <TabPanel>
                            <MissingTests
                                missingTests={this.state.missingTests}
                                apiId={this.props.match.params.apiId}
                                apiTitle={this.state.apiTitle}
                            />
                        </TabPanel>
                    </Tabs>
                </Row>
            </div>
        )
    }

    render() {
        return (
            <div>
                {this.state.date !== null && this.showRender()}
                <SlidingPane
                    isOpen={this.state.openSidePanel}
                    title={this.state.sidePanelInfo.title}
                    subtitle={this.state.sidePanelInfo.subtitle}
                    width="40%"
                    onRequestClose={() => {
                        // triggered on "<" on left top click or on outside click
                        this.setState({
                            openSidePanel: false,
                            sidePanelInfo: {},
                        });
                    }}
                >
                    <div>{this.showSidePanelDetails()}</div>
                </SlidingPane>
            </div>
        )
    }
}

/**/