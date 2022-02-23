import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import CardComp from '../components/CardComp'
import statsIcon from '../assets/stats.png'
import uploadIcon from '../assets/upload.png'
import loginIcon from '../assets/login.png'
import {Table,  Row, Col, Figure} from 'react-bootstrap'
import './Home.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";

export class Home extends Component {

    static displayName = Home.name;

    constructor(props) {
        super(props)
        this.state = {
            Auth: false,
            currentSetupTests: 0,
            latestActions: [],
            lastLogin: "",
            render: false
        }
        this.renderLastReports = this.renderLastReports.bind(this)
    }

    //check if user is autenticated, if not render notAuth vs render Auth
    async componentDidMount() {
        let token = await authService.getAccessToken()
        if (token !== null) {
            //fetch para saber os dados do user
            fetch(`Home/GetUserDetails`, {
                method: 'GET',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }).then(res => {
                if (res.status !== 204) {
                    res.json().then(details => {
                        let lastLoginString = details.lastLogin.split('T')
                        let finalString = lastLoginString[0] + ' ' + lastLoginString[1].split('.')[0]

                        details.latestActions.forEach(api => {
                            let auxString = ""
                            let auxfinalString = ""
                            if (api.reportDate !== "0001-01-01T00:00:00") {
                                auxString = api.reportDate.split('T')
                                auxfinalString = auxString[0] + ' ' + auxString[1].split('.')[0]
                                api.reportDate = auxfinalString
                            }
                            if (api.nextTest !== "0001-01-01T00:00:00") {
                                auxString = api.nextTest.split('T')
                                auxfinalString = auxString[0] + ' ' + auxString[1].split('.')[0]
                                api.nextTest = auxfinalString
                            }
                        })

                        this.setState({
                            Auth: true,
                            currentSetupTests: details.setupApiCount,
                            latestActions: details.latestActions,
                            lastLogin: finalString,
                            render: true
                        })
                    })
                }
                else {
                    this.setState({ render: true })
                }
            })
        }
        else {
            this.setState({
                render: true
            })
        }

    }

    renderLastReports(latestReports) {
        return latestReports.map(report => {
            if (report.reportDate === "0001-01-01T00:00:00") return null
            return <button type="button" key={report.apiId} className="link-button" onClick={() => this.props.history.push('/monitorTests/report/' + report.apiId)}>The latest test for {report.title} completed on {report.reportDate}<br></br></button>
        })
    }

    //render page for a logged in user
    renderAuth() {
        return (
            <div>
                <Row style={{ textAlign:"center" }}>
                    <h1 className="row justify-content-md-center" style={{ width: "100%" }}>Welcome back!</h1>
                    <h4 className="row justify-content-md-center" style={{ marginTop: 25, width: "100%" }}>Here is some general data about you:</h4>
                </Row>
                <Row style={{paddingTop: "50px"}}>
                    <Col>
                        <CardComp
                            title='Recently Completed Tests'
                            body={this.renderLastReports(this.state.latestActions)}
                        />
                    </Col>
                    <Col>                    
                        <CardComp
                            title='Configured APIs'
                            body={this.state.currentSetupTests}
                        />                       
                        <div style={{ paddingTop: "47px" }}></div>
                        <CardComp
                            title='Previous Login'
                            body={this.state.lastLogin}
                        />
                    </Col>
                </Row>
                <Row style={{ padding: "50px 100px 0px 100px" }}>
                    <Table striped bordered hover>
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>API Name</th>
                                <th>Next Test</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.latestActions.map((report,index) => {
                                return <tr key={index}><td>{index}</td><td>{report.title}</td><td>{report.nextTest === "0001-01-01T00:00:00" ? "-" : report.nextTest}</td></tr>
                            })}
                        </tbody>
                    </Table>
                </Row>
            </div>
        )
    }

    //render page for a not logged in user
    renderNotAuth() {
        return (
            <div>
                <Row style={{ textAlign: "center" }}>
                    <h1>RAPITest Helps You</h1>
                    <h1 style={{ fontWeight: "bold" }}>Validate API's Continuously</h1>
                    <h3>With 3 simple steps</h3>
                </Row>
                <Row style={{ textAlign: "center", paddingTop: "100px" }}>
                    <Col>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={loginIcon}
                            />
                            <Figure.Caption>
                                Register or Login
                            </Figure.Caption>
                        </Figure>
                    </Col>
                    <Col style={{ paddingTop: "100px" }}>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={uploadIcon}
                            />
                            <Figure.Caption>
                                Setup your test configuration
                            </Figure.Caption>
                        </Figure>
                    </Col>
                    <Col>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={180}
                                alt="171x180"
                                src={statsIcon}
                            />
                            <Figure.Caption>
                                Analyse the results
                            </Figure.Caption>
                        </Figure>
                    </Col>
                </Row>
                <Row style={{ padding: "50px 550px 0px 550px" }}>
                    <AwesomeButton type="primary" onPress={() => this.props.history.push('setupTest') }>Get Started!</AwesomeButton>
                </Row >
            </div>
        );
    }

    render() {
        if (this.state.render) {
            return (
                <div>
                    {this.state.Auth ? this.renderAuth() : this.renderNotAuth()}
                </div>
            )
        }
        return <div></div>
    }
}

