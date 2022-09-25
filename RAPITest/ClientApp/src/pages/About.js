import React, { Component } from 'react'
import duarteImage from '../assets/Duarte.jpg'
import openIcon from '../assets/openapi.png'
import tslIcon from '../assets/tslogo.png'
import loopIcon from '../assets/loop.webp'
import {Row, Col, Figure } from 'react-bootstrap'

export default class About extends Component {

    static displayName = About.name;


    render() {
        return (
            <div>
                <Row style={{  maxWidth: "1919px" }}>
                    <Col style={{ padding: "100px 100px 100px 100px" }}>
                        <img width="300" height="300" src={openIcon} alt="Logo" />
                    </Col>
                    <Col style={{ padding: "100px 150px 100px 75px" }}>
                        <h3>OpenAPI</h3>
                        <p></p>
                        <p>In order to test your desired API, you need to supply it's description.</p>
                        <p>For this purpose we take advantage of the OpenAPI initiative which defines a standard, language-agnostic interface to RESTful APIs.</p>
                        <p>This allows both humans and computers to discover and understand the capabilities of the service without access to source code, documentation, or through network traffic inspection.</p>     
                    </Col>
                </Row>
                <Row style={{ maxWidth: "1919px" }}>
                    <Col style={{ padding: "120px 75px 100px 100px" }}>
                        <h3>Test Specific Language</h3>
                        <p></p>
                        <p>After uploading the OpenAPI Specification you can customize tests with TSL files.</p>
                        <p>These yaml files define specific tests which would be impossible to generate with just the specification.</p>
                        <p>They allow for the creation of workflows, stress tests, native and custom verifications and much more!</p>
                    </Col>
                    <Col style={{ padding: "80px 100px 100px 100px" }}>
                        <img width="300" height="300" src={tslIcon} alt="Logo" />
                    </Col>
                </Row>
                <Row style={{ maxWidth: "1919px" }}>
                    <Col style={{ padding: "100px 100px 100px 100px" }}>
                        <img width="300" height="300" src={loopIcon} alt="Logo" />
                    </Col>
                    <Col style={{ padding: "100px 150px 100px 75px" }}>
                        <h3>Test Continuously</h3>
                        <p></p>
                        <p>Continuous testing is also supported!</p>
                        <p>After defining a time interval your tests are run at each interval which allows continuous monitoring of the API.</p>
                        <p>It is also useful for APIs in development to make sure nothing breaks with each update!</p>                    
                    </Col>
                </Row>
                <Row style={{ maxWidth: "1919px" }}>
                    <Col style={{ padding: "150px 150px 100px 75px" }}>                     
                        <h3>About me</h3>
                        <p></p>
                        <p>This App was developed over the course of one year and it served as my thesis for the masters degree in Computed Engineering for ISEL.</p>
                        <p>I learned alot from making this App and I really hope you like it!</p>
                        <p>You can find the source code for this app in my <a href="https://bitbucket.org/cmlcoi/rapite/src/master/" rel="noopener noreferrer" target="_blank">bitbucket</a>.</p>
                    </Col>
                    <Col style={{ padding: "100px 100px 100px 100px" }}>
                        <Figure>
                            <Figure.Image
                                width={300}
                                height={300}
                                alt="300x300"
                                src={duarteImage}
                            />
                            <Figure.Caption>
                                Duarte Felicio
                            </Figure.Caption>
                        </Figure>
                    </Col>   
                </Row>
            </div>
        )
    }
}