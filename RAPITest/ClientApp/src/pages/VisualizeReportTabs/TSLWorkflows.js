import React, { Component} from 'react';
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css';
import { Row, Badge } from 'react-bootstrap'
import './TSLWorkflows.css';
import Xarrow from "react-xarrows";

const rootStyle = { display: 'flex', margin: '25px 0', };
const rowStyle = { display: 'flex', justifyContent: 'space-between', margin: '0px 25px' }

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

        if (success) return "circleSucess"

        return "circleFail"
    }

    render() {
        let fullWorkflows = this.props.fullWorkflows
     
        return (
            
            <div>
                {fullWorkflows.map((workflow, indexWorkflow) =>
                    <Row key={indexWorkflow}>
                        {workflow.map((o, i) => {
                            return (
                                <div key={i} style={rootStyle}>
                                    {o.map((inner, inneri) => {
                                        return (
                                            <div key={inner.id} style={rowStyle}>
                                                <button id={inner.id} className={this.getCircleType(i, inneri, inner.Success)}>{inner.displayName}</button>
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
                )}
            </div>
 
        )
        
    }
}

