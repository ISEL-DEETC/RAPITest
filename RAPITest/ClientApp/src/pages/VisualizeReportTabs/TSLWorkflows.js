import React, { Component} from 'react';
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import { Row, Badge } from 'react-bootstrap'
import './TSLWorkflows.css';
import Xarrow from "react-xarrows";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { scaleOrdinal } from 'd3-scale';
import { schemeCategory10 } from 'd3-scale-chromatic';

const barcolors = scaleOrdinal(schemeCategory10).range();

const rootStyle = { display: 'flex', margin: '15px 0', };
const rowStyle = { display: 'flex', justifyContent: 'space-between', margin: '0px 15px' }

export default class TSLWorkflows extends Component {

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

    getCircleType(indexTest, indexInner, success) {
        if (indexTest === 0) return "circleMain"

        if (indexInner === 0) return "circleTest"

        if (success) return "circleSuccess"

        return "circleFail"
    }

    render() {
        let fullWorkflows = this.props.fullWorkflows
        let clickableFunction = this.props.clickableFunction
        let stressTestData = this.props.stressTestData
        let stressTestColumns = this.props.stressTestColumns

        return (
            
            <div>
                {fullWorkflows.map((workflow, indexWorkflow) =>
                    <div>
                        <h3 style={{ paddingTop: "20px", paddingBottom: "20px", fontWeight:"bold" }}>{workflow[0][0].displayName}</h3>
                        <Row key={indexWorkflow}>
                            {workflow.map((o, i) => {
                                return (
                                    <div key={i} style={rootStyle}>
                                        {o.map((inner, inneri) => {
                                            return (
                                                <div key={inner.id} style={rowStyle}>
                                                    <button id={inner.id} onClick={() => clickableFunction(inner.id,i,inneri)} className={this.getCircleType(i, inneri, inner.success)}>{inner.displayName.length > 8 ? inner.displayName.substring(0, 8) + ".." : inner.displayName}</button>
                                                </div>
                                            )
                                        })}
                                    </div>
                                )
                            })}

                            {workflow.map((o, i) => {
                                return (
                                    <div key={i}>
                                        {o.map((inner, inneri) => {
                                            return (
                                                <div>
                                                    {inner.targetId.map((target, targeti) => {
                                                        return (
                                                            <Xarrow
                                                                start={inner.id}
                                                                end={target}
                                                                showHead={false}
                                                            />
                                                        )
                                                    })}
                                                </div>
                                            )
                                        })}
                                    </div>
                                )
                            })}
                        </Row>
                        <Row style={{width:'100%'}}>
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
                                {stressTestColumns[indexWorkflow].map((o, i) => <Line type="monotone" dataKey={o} stroke={barcolors[i]} />)}
                            </LineChart>
                        </Row>
                    </div>
                )}
            </div>
 
        )
        
    }
}

