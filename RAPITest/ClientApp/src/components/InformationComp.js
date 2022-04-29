import React, { Component } from 'react'
import { Row, Col, Figure,Container } from 'react-bootstrap'

export default class InformationComp extends Component {
    render() {
        let width = this.props.width
        let height = this.props.height
        let alt = this.props.alt
        let src = this.props.src
        let mainInfo = this.props.mainInfo
        let bottomText = this.props.bottomText

        return (
            <Container fluid>
                <Row>
                    <Col style={{ display: 'flex', justifyContent: 'right' }}>
                        <Figure>
                            <Figure.Image
                                width={width}
                                height={height}
                                alt={alt}
                                src={src}
                            />
                            <Figure.Caption>
                            </Figure.Caption>
                        </Figure>
                    </Col>
                    <Col>
                        <Row style={{paddingTop: "10px"}}>
                            <h1 style={{ fontWeight:"bold" }}>{mainInfo}</h1>
                        </Row>
                        <Row>
                            <h4>{bottomText}</h4>
                        </Row>
                    </Col>
                </Row>
            </Container>
        )
    }
}