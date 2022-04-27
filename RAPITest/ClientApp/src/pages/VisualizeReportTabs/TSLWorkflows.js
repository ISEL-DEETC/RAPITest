import React, { Component } from 'react';
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import { Card, Accordion, Container, ListGroup, Badge } from 'react-bootstrap'
import AccordionItemListGroup from '../../components/AccordionItemListGroup'

export default class TSLWorkflows extends Component {

    showTime(value) {
        if (value === null) return 0
        let time = value.split('T')
        let date = time[0]
        let hours = time[1].split('.')[0]

        return date + " " + hours
    }

    printBadge(testResult) {
        if (testResult.Success) {
            return <Badge bg="success" pill>
                Success
            </Badge>
        }
        return <Badge bg="danger" pill>
            Error
        </Badge>
    }

    printErrors(testresult) {
        if (!testresult.Success)
            return <ListGroup.Item key={testresult.WorkflowID + testresult.TestID + testresult.TestName} as="li" className="d-flex justify-content-between align-items-start" >
                <div className="ms-2 me-auto">
                    <div className="fw-bold" style={{ textAlign: "left" }}>{testresult.TestID + " - " + testresult.TestName}</div>
                    <div style={{ textAlign: "left" }}>  {testresult.Description}</div>
                </div>
                <Badge bg="danger" pill>
                    Error
                </Badge>
            </ListGroup.Item>
        return <div key={testresult.WorkflowID + testresult.TestID + testresult.TestName}></div>
    }

    printWarnings(warning) {
        return <ListGroup.Item key={warning.Server + warning.Endpoint + warning.Method + warning.Consumes + warning.Produces + warning.ResponseCode} as="li" className="d-flex justify-content-between align-items-start" >
            <div className="ms-2 me-auto">
                <div className="fw-bold" style={{ textAlign: "left" }}>{warning.Method + " " + warning.Server + warning.Endpoint}</div>
                <div style={{ textAlign: "left" }}>Consumes:  {warning.Consumes}</div>
                <div style={{ textAlign: "left" }}>Produces:  {warning.Produces}</div>
                <div style={{ textAlign: "left" }}>Code:  {warning.ResponseCode}</div>
            </div>
            <Badge bg="warning" pill>
                Warning
            </Badge>
        </ListGroup.Item>
    }

    renderTest(test, testindex) {
        return (
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
        )
    }

    renderWorkflow(workflow, workflowindex) {
        return (
            <Accordion.Item key={workflow.WorkflowID + workflowindex} eventKey={workflow.WorkflowID}>
                <Accordion.Header>{workflow.WorkflowID}</Accordion.Header>
                <Accordion.Body>
                    {workflow.Tests.map((test, testindex) => {
                        return this.renderTest(test, testindex)
                    })}
                </Accordion.Body>
            </Accordion.Item >
        )
    }

    render() {
        return (<div></div>)
        let report = this.props.report
        let Tests = this.props.tests

        return (
            < Container >
                <Card className="text-center">
                    <Card.Header>Report Results</Card.Header>
                    <Card.Body>
                        <Accordion defaultActiveKey={['0']} alwaysOpen>
                            <AccordionItemListGroup
                                title={"Errors: " + report.Errors}
                                eventKey="0"
                                listItems={Tests}
                                printFunction={this.printErrors}
                            />
                            <AccordionItemListGroup
                                title={"Warnings: " + report.MissingTests.length}
                                eventKey="1"
                                listItems={report.MissingTests}
                                printFunction={this.printWarnings}
                            />
                            <Accordion.Item eventKey="workflows">
                                <Accordion.Header>Workflows</Accordion.Header>
                                <Accordion.Body>
                                    {report.WorkflowResults.map((workflow, workflowindex) => {
                                        return this.renderWorkflow(workflow, workflowindex)
                                    })}
                                </Accordion.Body>
                            </Accordion.Item>
                        </Accordion>
                    </Card.Body>
                    <Card.Footer className="text-muted">{this.showTime(report.date)}</Card.Footer>
                </Card>
            </Container >
        )
        
    }
}