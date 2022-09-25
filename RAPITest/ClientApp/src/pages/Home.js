import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import CardComp from '../components/CardComp'
import statsIcon from '../assets/stats.webp'
import uploadIcon from '../assets/upload.webp'
import loginIcon from '../assets/login.webp'
import { Table, Row, Col, Figure, Badge, ListGroup } from 'react-bootstrap'
import './Home.css';
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import testIcon from '../assets/test.png'
import calendarIcon from '../assets/calendar.png'
import clockIcon from '../assets/clock.png'

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
                        let lastLoginString = details.LastLogin.split('T')
                        let finalString = lastLoginString[0] + ' ' + lastLoginString[1].split('.')[0]

                        details.LatestActions.forEach(api => {
                            let auxString = ""
                            let auxfinalString = ""
                            if (api.ReportDate !== "0001-01-01T00:00:00") {
                                auxString = api.ReportDate.split('T')
                                auxfinalString = auxString[0] + ' ' + auxString[1].split('.')[0]
                                api.ReportDate = auxfinalString
                            }
                            if (api.NextTest !== "0001-01-01T00:00:00") {
                                auxString = api.NextTest.split('T')
                                auxfinalString = auxString[0] + ' ' + auxString[1].split('.')[0]
                                api.NextTest = auxfinalString
                            }
                        })

                        this.setState({
                            Auth: true,
                            currentSetupTests: details.SetupApiCount,
                            latestActions: details.LatestActions,
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
        return (
            <ListGroup>
                {latestReports.map(report => {
                    if (report.ReportDate !== "0001-01-01T00:00:00") {
                        return (
                            <ListGroup.Item key={report.ApiId}>
                                {report.Errors === -1 ? <div className="link-button" onClick={() => this.props.history.push('/monitorTests')}><img style={{ marginRight: "10px" }} width="20" height="20" src={testIcon} alt="Logo" />{report.Title} on {report.ReportDate} <div style={{ float: 'right' }}><Badge bg="danger">Validation Error</Badge></div></div> : <div className="link-button" onClick={() => this.props.history.push('/monitorTests/report/' + report.ApiId)}><img style={{ marginRight: "10px" }} width="20" height="20" src={testIcon} alt="Logo" />{report.Title} on {report.ReportDate} <div style={{ float: 'right' }}><Badge bg="danger">{report.Errors} Errors</Badge>{' '}<Badge bg="warning">{report.Warnings} Warnings</Badge></div></div>}
                                
                            </ListGroup.Item>
                        )
                    }
                    return <div key={report.ApiId}></div>
                })}
            </ListGroup>
        )
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
                            icon={clockIcon}
                        />
                    </Col>
                    <Col>                    
                        <CardComp
                            title='Configured Tests'
                            body={this.state.currentSetupTests}
                            icon={testIcon}
                        />                       
                        <div style={{ paddingTop: "47px" }}></div>
                        <CardComp
                            title='Previous Login'
                            body={this.state.lastLogin}
                            icon={calendarIcon}
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
                                return <tr key={index}><td>{index}</td><td>{report.Title}</td><td>{report.NextTest === "0001-01-01T00:00:00" ? "-" : report.NextTest}</td></tr>
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
                    <h1>RapiTest Helps You</h1>
                    <h1 style={{ fontWeight: "bold" }}>Validate APIs Continuously</h1>
                    <h3>With 3 simple steps</h3>
                </Row>
                <Row style={{ textAlign: "center", paddingTop: "100px" }}>
                    <Col>
                        <Figure>
                            <Figure.Image
                                width={171}
                                height={171}
                                alt="171x171"
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
                                height={171}
                                alt="171x171"
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
                                height={171}
                                alt="171x171"
                                src={statsIcon}
                            />
                            <Figure.Caption>
                                Analyse the results
                            </Figure.Caption>
                        </Figure>
                    </Col>
                </Row>
                <Row className="justify-content-center" style={{ paddingTop: "50px" }}>
                    <AwesomeButton style={{ width: "200px" }} type="primary" onPress={() => this.props.history.push('setupTest') }>Get Started!</AwesomeButton>
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

