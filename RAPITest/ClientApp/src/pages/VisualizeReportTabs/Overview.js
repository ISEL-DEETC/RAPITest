import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import "react-awesome-button/dist/styles.css";
import errorIcon from '../../assets/warning.png'
import warningIcon from '../../assets/bell.png'
import workflowIcon from '../../assets/share.png'
import generatedIcon from '../../assets/gear.png'
import timeIcon from '../../assets/hourglass.png'
import { Row, Col} from 'react-bootstrap'
import InformationComp from '../../components/InformationComp.js'
import PieChartComp from '../../components/PieChartComp.js'
import { BarChart, Bar, Cell, XAxis, YAxis, CartesianGrid, Tooltip } from 'recharts';
import { scaleOrdinal } from 'd3-scale';
import { schemeCategory10 } from 'd3-scale-chromatic';

const COLORS = ['#ff5d5d', '#10CD29'];

const barcolors = scaleOrdinal(schemeCategory10).range();

export default class Overview extends Component {

    render() {
        let errors = this.props.errors
        let warnings = this.props.warnings
        let workflows = this.props.workflows
        let generatedTests = this.props.generatedTests
        let totalCompletionTime = this.props.totalCompletionTime
        let pieChartData = this.props.pieChartData
        let barChartData = this.props.barChartData

        console.log(barChartData)

        return (
            <div>
                <Row style={{marginTop:"30px"}}>
                    <Col>
                        <InformationComp
                            width={80}
                            height={80}
                            alt={"80x80"}
                            src={errorIcon}
                            mainInfo={errors}
                            bottomText={"Errors"}
                        />
                    </Col>
                    <Col>
                        <InformationComp
                            width={80}
                            height={80}
                            alt={"80x80"}
                            src={warningIcon}
                            mainInfo={warnings}
                            bottomText={"Warnings"}
                        />
                    </Col>
                    <Col>
                        <InformationComp
                            width={80}
                            height={80}
                            alt={"80x80"}
                            src={workflowIcon}
                            mainInfo={workflows.length}
                            bottomText={"Workflows"}
                        />
                    </Col>
                </Row>
                <Row style={{ padding:"40px 200px 0px 200px"}}>
                    <Col>
                        <InformationComp
                            width={80}
                            height={80}
                            alt={"80x80"}
                            src={generatedIcon}
                            mainInfo={generatedTests.length}
                            bottomText={"Generated Tests"}
                        />
                    </Col>
                    <Col>
                        <InformationComp
                            width={80}
                            height={80}
                            alt={"80x80"}
                            src={timeIcon}
                            mainInfo={Math.round(totalCompletionTime / 1000) + " Sec"}
                            bottomText={"Completion Time"}
                        />
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <PieChartComp
                            width={400}
                            height={300}
                            data={pieChartData}
                            cx="70%"
                            cy="60%"
                            outerRadius={100}
                            dataKey="value"
                            colors={COLORS}
                        />
                    </Col>
                    <Col>
                        <BarChart
                            width={500}
                            height={300}
                            margin={{ top: 80, right: 20, left: 20, bottom: 5, }}
                            data={barChartData}
                        >
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="name" />
                            <YAxis />
                            <Tooltip />
                            <Bar
                                dataKey="Total_Time"
                                fill="#00a0fc"
                                stroke="#000000"
                                strokeWidth={1}
                            >
                                {
                                    barChartData.map((entry, index) => (
                                        <Cell key={`cell-${index}`} fill={barcolors[index % 20]} />
                                    ))
                                }
                            </Bar>
                        </BarChart>
                    </Col>
                </Row>
            </div>
        )
    }
}