import React, { Component } from 'react'
import duarteImage from '../assets/Duarte.jpg'
import openIcon from '../assets/openapi.png'
import logoISEL from '../assets/logoISEL.png'
import logoCML from '../assets/logoCML.png'
import tslIcon from '../assets/tslogo.png'
import loopIcon from '../assets/loop.webp'
import {Row, Col } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'

export default class About extends Component {

    static displayName = About.name;


    render() {
        return (
            <div>
                <Row style={{ padding: "50px 150px 0px 75px" }}>
                    <h3>RapiTest</h3>
                    <p></p>
                    <p>RapiTest consists of a WEB API test framework capable of testing security aspects, network latency, and load. The main objective is to validate real APIs used for data ingestion on Plataforma de Gestão de Dados de Lisboa (PGIL).</p>
                    <p>RapiTest was developed as part of "DADOS AO SERVIÇO DE LISBOA" partnership between the Lisbon City council (CML) and Lisbon School of Engineering (ISEL).</p>
                    <p>This project is under MIT licence.</p>
                </Row>
                <Row style={{ maxWidth: "1919px" }}>
                    <Col style={{ padding: "50px 150px 0px 75px" }}>                     
                        <h3>Authors</h3>
                        <p></p>
                        <p>
                            Duarte Felício, 
                            Nuno Datia <a href="https://orcid.org/0000-0003-1600-0227"><Image src="https://orcid.org/assets/vectors/orcid.logo.icon.svg" height="16px" width="16px" roundedCircle/></a>,
                            José Simão <a href="https://orcid.org/0000-0002-6564-593X"><Image src="https://orcid.org/assets/vectors/orcid.logo.icon.svg" height="16px" width="16px" roundedCircle/></a>,
                            Renato Marcelo
                        </p>
                    </Col>
                </Row>
                <Row style={{ maxWidth: "1919px" }}>
                    <Col style={{ padding: "0px 100px 0px 0px" }}>
                        <img width="500" height="333" src={logoISEL} alt="Logo" />
                    </Col>
                    <Col style={{ padding: "40px 100px 0px 0px" }}>
                        <img width="500" height="250" src={logoCML} alt="Logo" />
                    </Col>
                </Row>
            </div>
        )
    }
}