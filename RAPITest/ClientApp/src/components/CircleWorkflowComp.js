import React, { Component } from 'react'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import './CircleWorkflowComp.css';
import { Row } from 'react-bootstrap'
import Xarrow from "react-xarrows";

const rootStyle = { display: 'flex', margin: '15px 0', };
const rowStyle = { display: 'flex', justifyContent: 'space-between', margin: '0px 15px' }

export default class CircleWorkflowComp extends Component {

    getCircleType(indexTest, indexInner, success) {
        if (indexTest === 0) return "circleMain"

        if (indexInner === 0) return "circleTest"

        if (success) return "circleSuccess"

        return "circleFail"
    }

    render() {
        let workflow = this.props.workflow
        let indexWorkflow = this.props.indexWorkflow
        let clickableFunction = this.props.clickableFunction
        let fromGenerated = this.props.fromGenerated

        return (
            <Row key={indexWorkflow}>
                {workflow.map((o, i) => {
                    return (
                        <div key={i} style={rootStyle}>
                            {o.map((inner, inneri) => {
                                return (
                                    <div key={inner.id} style={rowStyle}>
                                        <button id={inner.id} onClick={() => clickableFunction(inner.id, i, inneri,fromGenerated)} className={this.getCircleType(i, inneri, inner.success)}>{inner.displayName.length > 8 ? inner.displayName.substring(0, 8) + ".." : inner.displayName}</button>
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
                                    <div key={inneri}>
                                        {inner.targetId.map((target, targeti) => {
                                            return (
                                                <Xarrow
                                                    start={inner.id}
                                                    end={target}
                                                    showHead={false}
                                                    key={targeti}
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
        )
    }
}