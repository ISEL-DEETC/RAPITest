import React, { Component } from 'react'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';
import { ListGroup, Row, Col } from 'react-bootstrap'
import './ListGroupComp.css';

export default class ListGroupComp extends Component {

    render() {
        let title = this.props.title
        let files = this.props.files
        let symbol = this.props.symbol
        let toShow = this.props.toShow
        let removeSymbol = this.props.removeSymbol
        let removeFunction = this.props.removeFunction

        return (
            <div>
                <h4>{files.length === 0 ? <div></div> : title}</h4>
                <ListGroup as="ol">
                    {
                        files.map((f, i) => <ListGroup.Item as="li" key={i}>
                            <Row>
                                <Col sm={2}>
                                    <img style={{ marginRight: "15px" }} width="35" height="35" src={symbol} alt="Logo" />
                                </Col>
                                <Col sm={9}>
                                    {toShow(f)}
                                </Col>
                                <Col sm={1}>
                                    <div className="removeFileIcon" onClick={() => removeFunction(f)}>
                                        <img width="35" height="35" src={removeSymbol} alt="Logo" />
                                    </div>
                                </Col>
                            </Row>
                        </ListGroup.Item>)
                    }
                </ListGroup>
            </div>
        )
    }
}