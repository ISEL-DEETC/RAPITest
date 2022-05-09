import React, { Component} from 'react';
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import { Row, Table } from 'react-bootstrap'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';
import { scaleOrdinal } from 'd3-scale';
import { schemeCategory10 } from 'd3-scale-chromatic';
import CircleWorkflowComp from '../../components/CircleWorkflowComp.js'

const barcolors = scaleOrdinal(schemeCategory10).range();

export default class TSLWorkflows extends Component {

    renderStressTest(stressTestMetadata, indexWorkflow, stressTestData, stressTestColumns) {
        return (
            <div>
                <h3 style={{ paddingTop: "20px", paddingBottom: "20px" }}>Stress Test</h3>
                <Table striped bordered hover>
                    <thead>
                        <tr>
                            <th>Tests</th>
                            <th>Min (ms)</th>
                            <th>Max (ms)</th>
                            <th>Average (ms)</th>
                            <th>Last (ms)</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
                        {stressTestMetadata[indexWorkflow].map((test, i) => {
                            return (
                                <tr key={i}>
                                    <td>{test[0]}</td>
                                    <td>{test[1]}</td>
                                    <td>{test[2]}</td>
                                    <td>{test[3]}</td>
                                    <td>{test[4]}</td>
                                    <td>{test[5]}</td>
                                </tr>
                            )

                        })}
                    </tbody>
                </Table>
                <Row style={{ width: '100%', paddingTop: "20px" }}>
                    <LineChart
                        width={document.getElementById('myTabsID').clientWidth}
                        height={300}
                        data={stressTestData[indexWorkflow]}
                        margin={{
                            top: 5,
                            right: 30,
                            left: 20,
                            bottom: 5,
                        }}
                    >
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="name" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        {stressTestColumns[indexWorkflow].map((o, i) => <Line type="monotone" key={i} dataKey={o} stroke={barcolors[i]} />)}
                    </LineChart>
                </Row>
            </div>
        )
    }

    render() {
        let fullWorkflows = this.props.fullWorkflows
        let clickableFunction = this.props.clickableFunction
        let stressTestData = this.props.stressTestData
        let stressTestColumns = this.props.stressTestColumns
        let stressTestMetadata = this.props.stressTestMetadata

        return (
            
            <div>
                {fullWorkflows.map((workflow, indexWorkflow) =>
                    <div key={indexWorkflow}>
                        <h3 style={{ paddingTop: "20px", paddingBottom: "20px", fontWeight:"bold" }}>{workflow[0][0].displayName}</h3>
                        <CircleWorkflowComp
                            workflow={workflow}
                            indexWorkflow={indexWorkflow}
                            clickableFunction={clickableFunction}
                            fromGenerated={false}
                        />
                        {stressTestData[indexWorkflow].length !== 0 && this.renderStressTest(stressTestMetadata, indexWorkflow, stressTestData, stressTestColumns)}
                        
                    </div>
                )}
            </div>
 
        )
        
    }
}

